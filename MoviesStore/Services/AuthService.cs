public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public AuthService(IUsersRepository usersRepository,
        IJwtService jwtService, IPasswordService passwordService)
    {
        _usersRepository = usersRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    public async Task<string> RegisterAsync(UserAuthDto request)
    {
        if (await _usersRepository.IsUserByEmailAsync(request.Email))
        {
            throw new Exception("User with this email already exists.");
        }

        var newUser = await _usersRepository.AddUserAsync(new User
        {
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(new User { Email = request.Email }, request.Password)
        });

        return _jwtService.CreateToken(newUser);
    }

    public async Task<string> LoginAsync(UserAuthDto request)
    {
        var user = await _usersRepository.GetUserByEmailAsync(request.Email);

        if (user is null)
        {
            throw new Exception("Invalid email or password.");
        }

        if(!_passwordService.IsVerifyHashedPassword(user, request.Password))
        {
            throw new Exception("Invalid email or password.");
        }

        return _jwtService.CreateToken(user);
    }
}
