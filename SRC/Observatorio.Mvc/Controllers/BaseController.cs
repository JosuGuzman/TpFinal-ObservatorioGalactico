using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Observatorio.Mvc.Controllers;

public class BaseController : Controller
{
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    protected string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value;
    }

    protected string GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value;
    }

    protected string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }

    protected bool IsAdmin()
    {
        return GetCurrentUserRole() == "Admin";
    }

    protected bool IsAstronomer()
    {
        var role = GetCurrentUserRole();
        return role == "Astronomer" || role == "Admin";
    }

    protected bool IsResearcher()
    {
        var role = GetCurrentUserRole();
        return role == "Researcher" || role == "Astronomer" || role == "Admin";
    }

    protected void AddSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    protected void AddErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    protected void AddWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    protected IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}