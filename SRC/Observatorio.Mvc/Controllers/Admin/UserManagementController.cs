using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Observatorio.Core.Entities.User;
using Observatorio.Core.Interfaces;
using Observatorio.Mvc.Models.Admin;

namespace Observatorio.Mvc.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/[controller]")]
public class UserManagementController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly ILoggingService _loggingService;

    public UserManagementController(
        IUserRepository userRepository,
        IUserService userService,
        ILoggingService loggingService)
    {
        _userRepository = userRepository;
        _userService = userService;
        _loggingService = loggingService;
    }

    [HttpGet]
    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index(string search = "", string role = "", string status = "", 
                                          int page = 1, int pageSize = 20, string sortBy = "CreatedAt", 
                                          bool sortDesc = true)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            
            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => 
                    u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (u.Role?.RoleName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => 
                    (u.Role?.RoleName?.Equals(role, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                    users = users.Where(u => u.IsActive).ToList();
                else if (status == "inactive")
                    users = users.Where(u => !u.IsActive).ToList();
            }

            // Aplicar ordenamiento
            users = sortBy.ToLower() switch
            {
                "username" => sortDesc ? users.OrderByDescending(u => u.UserName).ToList() : users.OrderBy(u => u.UserName).ToList(),
                "email" => sortDesc ? users.OrderByDescending(u => u.Email).ToList() : users.OrderBy(u => u.Email).ToList(),
                "role" => sortDesc ? users.OrderByDescending(u => u.Role?.RoleName).ToList() : users.OrderBy(u => u.Role?.RoleName).ToList(),
                "lastlogin" => sortDesc ? users.OrderByDescending(u => u.LastLogin).ToList() : users.OrderBy(u => u.LastLogin).ToList(),
                _ => sortDesc ? users.OrderByDescending(u => u.CreatedAt).ToList() : users.OrderBy(u => u.CreatedAt).ToList()
            };

            var totalUsers = users.Count;
            var pagedUsers = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserViewModel
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    Email = u.Email,
                    DisplayName = u.DisplayName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    Role = u.Role?.RoleName ?? "User",
                    RoleID = u.RoleID,
                    ApiKey = u.ApiKey
                })
                .ToList();

            var model = new UserManagementViewModel
            {
                Users = pagedUsers,
                Search = search,
                SelectedRole = role,
                SelectedStatus = status,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalUsers,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                SortBy = sortBy,
                SortDescending = sortDesc,
                AvailableRoles = await GetAvailableRoles(),
                AvailableStatuses = new List<string> { "", "active", "inactive" }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserDetailViewModel
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                Role = user.Role?.RoleName ?? "User",
                RoleID = user.RoleID,
                ApiKey = user.ApiKey,
                CanEdit = true // Admin siempre puede editar
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving user details: {id}");
            return View("Error");
        }
    }

    [HttpGet]
    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new EditUserViewModel
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Email = user.Email,
                IsActive = user.IsActive,
                RoleID = user.RoleID,
                AvailableRoles = await GetAvailableRoles()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving user for edit: {id}");
            return View("Error");
        }
    }

    [HttpPost]
    [Route("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = await GetAvailableRoles();
            return View(model);
        }

        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Guardar valores antiguos para el log
            var oldUserName = user.UserName;
            var oldEmail = user.Email;
            var oldIsActive = user.IsActive;
            var oldRoleId = user.RoleID;

            // Actualizar usuario
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.IsActive = model.IsActive;
            user.RoleID = model.RoleID;

            await _userRepository.UpdateAsync(user);

            // Registrar cambios
            var changes = new List<string>();
            if (oldUserName != model.UserName)
                changes.Add($"Username: {oldUserName} -> {model.UserName}");
            if (oldEmail != model.Email)
                changes.Add($"Email: {oldEmail} -> {model.Email}");
            if (oldIsActive != model.IsActive)
                changes.Add($"Status: {(oldIsActive ? "Active" : "Inactive")} -> {(model.IsActive ? "Active" : "Inactive")}");
            if (oldRoleId != model.RoleID)
                changes.Add($"Role ID: {oldRoleId} -> {model.RoleID}");

            if (changes.Any())
            {
                await _loggingService.LogInfoAsync("UserUpdated",
                    $"User {id} updated by admin {GetCurrentUserEmail()}. Changes: {string.Join(", ", changes)}",
                    GetCurrentUserId());
            }

            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableRoles = await GetAvailableRoles();
            _logger.LogError(ex, $"Error updating user: {id}");
            return View(model);
        }
    }

    [HttpPost]
    [Route("ToggleStatus/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _userRepository.UpdateAsync(user);

            var action = user.IsActive ? "activated" : "deactivated";
            await _loggingService.LogInfoAsync("UserStatusChanged",
                $"User {user.Email} {action} by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = $"User {action} successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error toggling user status: {id}");
            TempData["ErrorMessage"] = "Error changing user status.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Route("ChangeRole/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(int id, int roleId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var oldRoleId = user.RoleID;
            user.RoleID = roleId;
            await _userRepository.UpdateAsync(user);

            await _loggingService.LogInfoAsync("UserRoleChanged",
                $"User {user.Email} role changed from {oldRoleId} to {roleId} by admin {GetCurrentUserEmail()}",
                GetCurrentUserId());

            TempData["SuccessMessage"] = "User role changed successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error changing user role: {id}");
            TempData["ErrorMessage"] = "Error changing user role.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Route("ResetPassword/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userService.ResetPasswordAsync(user.Email);

            await _loggingService.LogInfoAsync("PasswordResetByAdmin",
                $"Password reset for user {user.Email} by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = $"Password reset email sent to {user.Email}.";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error resetting password for user: {id}");
            TempData["ErrorMessage"] = "Error resetting password.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Route("GenerateApiKey/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateApiKey(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userService.GenerateApiKeyAsync(id);

            await _loggingService.LogInfoAsync("ApiKeyGeneratedByAdmin",
                $"API key generated for user {user.Email} by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "API key generated successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating API key for user: {id}");
            TempData["ErrorMessage"] = "Error generating API key.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Route("RevokeApiKey/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RevokeApiKey(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.ApiKey = null;
            await _userRepository.UpdateAsync(user);

            await _loggingService.LogInfoAsync("ApiKeyRevokedByAdmin",
                $"API key revoked for user {user.Email} by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "API key revoked successfully!";
            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error revoking API key for user: {id}");
            TempData["ErrorMessage"] = "Error revoking API key.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    [Route("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // No permitir eliminar el propio usuario
            if (user.UserID == GetCurrentUserId())
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToAction("Details", new { id });
            }

            await _userRepository.DeleteAsync(id);

            await _loggingService.LogInfoAsync("UserDeleted",
                $"User {user.Email} deleted by admin {GetCurrentUserEmail()}", GetCurrentUserId());

            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user: {id}");
            TempData["ErrorMessage"] = "Error deleting user.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpGet]
    [Route("BulkActions")]
    public IActionResult BulkActions()
    {
        var model = new BulkActionsViewModel
        {
            AvailableActions = new List<BulkActionViewModel>
            {
                new BulkActionViewModel { Id = "activate", Name = "Activate Users", Description = "Activate selected users" },
                new BulkActionViewModel { Id = "deactivate", Name = "Deactivate Users", Description = "Deactivate selected users" },
                new BulkActionViewModel { Id = "resetpasswords", Name = "Reset Passwords", Description = "Send password reset emails to selected users" },
                new BulkActionViewModel { Id = "generatetickeys", Name = "Generate API Keys", Description = "Generate new API keys for selected users" },
                new BulkActionViewModel { Id = "revokeapikeys", Name = "Revoke API Keys", Description = "Revoke API keys from selected users" }
            }
        };

        return View(model);
    }

    [HttpPost]
    [Route("ProcessBulkAction")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessBulkAction(string action, List<int> userIds)
    {
        if (userIds == null || !userIds.Any())
        {
            TempData["ErrorMessage"] = "No users selected.";
            return RedirectToAction("BulkActions");
        }

        try
        {
            int successCount = 0;
            int errorCount = 0;

            foreach (var userId in userIds)
            {
                try
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null)
                        continue;

                    switch (action)
                    {
                        case "activate":
                            user.IsActive = true;
                            await _userRepository.UpdateAsync(user);
                            break;

                        case "deactivate":
                            // No permitir desactivar el propio usuario
                            if (userId != GetCurrentUserId())
                            {
                                user.IsActive = false;
                                await _userRepository.UpdateAsync(user);
                            }
                            break;

                        case "resetpasswords":
                            await _userService.ResetPasswordAsync(user.Email);
                            break;

                        case "generateapikeys":
                            await _userService.GenerateApiKeyAsync(userId);
                            break;

                        case "revokeapikeys":
                            user.ApiKey = null;
                            await _userRepository.UpdateAsync(user);
                            break;
                    }

                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing bulk action {action} for user {userId}");
                    errorCount++;
                }
            }

            await _loggingService.LogInfoAsync("BulkActionProcessed",
                $"Bulk action '{action}' processed on {successCount} users by admin {GetCurrentUserEmail()}. Success: {successCount}, Errors: {errorCount}",
                GetCurrentUserId());

            TempData["SuccessMessage"] = $"Bulk action completed. Success: {successCount}, Errors: {errorCount}";
            return RedirectToAction("BulkActions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing bulk action: {action}");
            TempData["ErrorMessage"] = "Error processing bulk action.";
            return RedirectToAction("BulkActions");
        }
    }

    [HttpGet]
    [Route("Export")]
    public async Task<IActionResult> Export(string format = "csv")
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            
            if (format == "json")
            {
                var jsonData = users.Select(u => new
                {
                    u.UserID,
                    u.UserName,
                    u.Email,
                    DisplayName = u.DisplayName,
                    u.IsActive,
                    CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastLogin = u.LastLogin?.ToString("yyyy-MM-dd HH:mm:ss"),
                    Role = u.Role?.RoleName,
                    HasApiKey = !string.IsNullOrEmpty(u.ApiKey)
                });

                return File(System.Text.Encoding.UTF8.GetBytes(
                    System.Text.Json.JsonSerializer.Serialize(jsonData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true })),
                    "application/json", $"users-export-{DateTime.UtcNow:yyyyMMdd}.json");
            }
            else
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("UserID,UserName,Email,DisplayName,IsActive,CreatedAt,LastLogin,Role,HasApiKey");
                
                foreach (var user in users)
                {
                    csv.AppendLine($"\"{user.UserID}\",\"{user.UserName}\",\"{user.Email}\",\"{user.DisplayName}\",\"{user.IsActive}\",\"{user.CreatedAt:yyyy-MM-dd HH:mm:ss}\",\"{user.LastLogin?.ToString("yyyy-MM-dd HH:mm:ss")}\",\"{user.Role?.RoleName}\",\"{!string.IsNullOrEmpty(user.ApiKey)}\"");
                }

                return File(System.Text.Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"users-export-{DateTime.UtcNow:yyyyMMdd}.csv");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting users");
            TempData["ErrorMessage"] = "Error exporting users.";
            return RedirectToAction("Index");
        }
    }

    private async Task<List<RoleViewModel>> GetAvailableRoles()
    {
        // En una implementación real, esto vendría de la base de datos
        return new List<RoleViewModel>
        {
            new RoleViewModel { RoleID = 1, RoleName = "Admin" },
            new RoleViewModel { RoleID = 2, RoleName = "Astronomer" },
            new RoleViewModel { RoleID = 3, RoleName = "Researcher" },
            new RoleViewModel { RoleID = 4, RoleName = "User" }
        };
    }
}