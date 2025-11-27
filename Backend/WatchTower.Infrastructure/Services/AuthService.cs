namespace WatchTower.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        if (!user.IsActive)
            throw new BusinessRuleException(ErrorMessages.UserInactive);

        // Actualizar último login
        await _userRepository.UpdateLastLoginAsync(user.UserId);

        var token = _tokenService.GenerateToken(user);
        
        return new AuthResponse
        {
            Token = token,
            User = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            },
            ExpiresAt = DateTime.UtcNow.AddHours(AppConstants.TokenExpirationHours)
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.UserExistsAsync(request.Username, request.Email))
            throw new DuplicateException("User");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = UserRole.Visitor
        };

        var userId = await _userRepository.CreateAsync(user);
        user.UserId = userId;

        var token = _tokenService.GenerateToken(user);
        
        return new AuthResponse
        {
            Token = token,
            User = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            },
            ExpiresAt = DateTime.UtcNow.AddHours(AppConstants.TokenExpirationHours)
        };
    }

    public async Task<UserResponse?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserResponse
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new NotFoundException("User", userId);

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return false;

        // En una aplicación real, aquí se enviaría un correo con un token de restablecimiento
        // Por ahora, simplemente devolvemos true si el usuario existe
        return true;
    }
}