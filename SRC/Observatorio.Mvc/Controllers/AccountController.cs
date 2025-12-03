using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Account;
using System.Security.Claims;

namespace Observatorio.Mvc.Controllers;

public class AccountController : BaseController
{
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IUserService userService,
        IAuthenticationService authService,
        ILogger<AccountController> logger)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var user = await _userService.AuthenticateAsync(model.Email, model.Password);
            
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Credenciales inválidas o cuenta inactiva");
                return View(model);
            }

            // Crear claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("DisplayName", user.DisplayName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            _logger.LogInformation("Usuario {Email} ha iniciado sesión", user.Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante login");
            ModelState.AddModelError("", "Error durante el inicio de sesión");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // Validar que el email no existe
            if (await _userService.UserExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Este email ya está registrado");
                return View(model);
            }

            // Crear usuario
            var user = await _userService.RegisterAsync(
                model.Email,
                model.UserName,
                model.Password,
                model.Role == "Admin" ? 1 : 2);

            // Autenticar automáticamente
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("DisplayName", user.DisplayName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            AddSuccessMessage("¡Registro exitoso! Bienvenido al Observatorio WatchTower.");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = GetCurrentUserId();
        var user = await _userService.GetByIdAsync(userId);
        
        if (user == null)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        var model = new ProfileViewModel
        {
            UserID = user.UserID,
            UserName = user.UserName,
            Email = user.Email,
            Bio = "", // Agregar campo bio si existe
            ProfileImage = "", // Agregar campo de imagen
            EmailNotifications = true,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var user = await _userService.GetByIdAsync(model.UserID);
            user.UserName = model.UserName;
            
            await _userService.UpdateUserAsync(user);
            
            // Actualizar claim de nombre si cambió
            if (User.Identity.Name != model.UserName)
            {
                await HttpContext.SignOutAsync();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                    new Claim("DisplayName", user.DisplayName)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);
            }

            AddSuccessMessage("Perfil actualizado correctamente");
            return RedirectToAction("Profile");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = GetCurrentUserId();
            await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
            
            AddSuccessMessage("Contraseña cambiada exitosamente");
            return RedirectToAction("Profile");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _userService.ResetPasswordAsync(model.Email);
            AddSuccessMessage("Se han enviado instrucciones a tu email para restablecer la contraseña.");
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        AddSuccessMessage("Has cerrado sesión correctamente.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ApiKey()
    {
        var userId = GetCurrentUserId();
        var user = await _userService.GetByIdAsync(userId);
        
        var model = new ApiKeyViewModel
        {
            ApiKey = user.ApiKey,
            GeneratedAt = user.CreatedAt // Mejorar con fecha específica
        };
        
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateApiKey()
    {
        var userId = GetCurrentUserId();
        await _userService.GenerateApiKeyAsync(userId);
        
        AddSuccessMessage("Nueva API Key generada exitosamente");
        return RedirectToAction("ApiKey");
    }
}