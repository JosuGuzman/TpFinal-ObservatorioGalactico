namespace Observatorio.Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly ILoggingService _loggingService;

    public UserService(IUserRepository userRepository, ILoggingService loggingService)
    {
        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher();
        _loggingService = loggingService;
    }

    public async Task<User> AuthenticateAsync(string email, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                await _loggingService.LogWarningAsync("Authentication", 
                    $"Failed login attempt for email: {email}", null);
                throw new AuthenticationException(ErrorMessages.INVALID_CREDENTIALS);
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                await _loggingService.LogWarningAsync("Authentication", 
                    $"Invalid password for user: {user.UserID}", user.UserID);
                throw new AuthenticationException(ErrorMessages.INVALID_CREDENTIALS);
            }

            await _userRepository.UpdateLastLoginAsync(user.UserID);
            await _loggingService.LogInfoAsync("Authentication", 
                $"User logged in: {user.Email}", user.UserID);

            return user;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Authentication", 
                $"Error authenticating user: {email}", null, null, ex);
            throw;
        }
    }

    public async Task<User> RegisterAsync(string email, string username, string password, int roleId = 2)
    {
        try
        {
            if (await _userRepository.EmailExistsAsync(email))
                throw new ValidationException(ErrorMessages.EMAIL_EXISTS);

            if (await _userRepository.UsernameExistsAsync(username))
                throw new ValidationException(ErrorMessages.USERNAME_EXISTS);

            if (!_passwordHasher.IsStrongPassword(password))
                throw new ValidationException(ErrorMessages.PASSWORD_TOO_WEAK);

            var passwordHash = _passwordHasher.HashPassword(password);

            var user = new User
            {
                Email = email,
                UserName = username,
                PasswordHash = passwordHash,
                RoleID = roleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            
            await _loggingService.LogInfoAsync("Registration", 
                $"New user registered: {email} (ID: {createdUser.UserID})", createdUser.UserID);

            return createdUser;
        }
        catch (Exception ex)
        {
            await _loggingService.LogErrorAsync("Registration", 
                $"Error registering user: {email}", null, null, ex);
            throw;
        }
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User", id);

        return user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            throw new NotFoundException("User", email);

        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await GetByIdAsync(user.UserID);
        
        // Validar que el email no esté duplicado si se cambia
        if (existingUser.Email != user.Email && await _userRepository.EmailExistsAsync(user.Email))
            throw new ValidationException(ErrorMessages.EMAIL_EXISTS);

        await _userRepository.UpdateAsync(user);
        await _loggingService.LogInfoAsync("UserUpdate", 
            $"User updated: {user.Email} (ID: {user.UserID})", user.UserID);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await GetByIdAsync(id);
        await _userRepository.DeleteAsync(id);
        
        await _loggingService.LogInfoAsync("UserDelete", 
            $"User deleted: {user.Email} (ID: {id})", user.UserID);
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await GetByIdAsync(userId);
        
        if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            throw new AuthenticationException("Current password is incorrect");

        if (!_passwordHasher.IsStrongPassword(newPassword))
            throw new ValidationException(ErrorMessages.PASSWORD_TOO_WEAK);

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        
        await _loggingService.LogInfoAsync("PasswordChange", 
            $"Password changed for user: {user.Email}", user.UserID);
    }

    public async Task ResetPasswordAsync(string email)
    {
        var user = await GetByEmailAsync(email);
        var newPassword = _passwordHasher.GenerateRandomPassword();
        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        
        await _userRepository.UpdateAsync(user);
        
        await _loggingService.LogInfoAsync("PasswordReset", 
            $"Password reset for user: {user.Email}", user.UserID);
        
        // En producción, aquí enviarías el email con la nueva contraseña
    }

    public async Task GenerateApiKeyAsync(int userId)
    {
        var user = await GetByIdAsync(userId);
        user.ApiKey = StringHelpers.GenerateRandomString(64);
        
        await _userRepository.UpdateAsync(user);
        
        await _loggingService.LogInfoAsync("ApiKeyGenerated", 
            $"API key generated for user: {user.Email}", user.UserID);
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        var user = await _userRepository.GetByApiKeyAsync(apiKey);
        return user != null && user.IsActive;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        return await _userRepository.CountAsync();
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Count(u => u.IsActive);
    }
}