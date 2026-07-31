using Npgsql;

public class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly string _connectionString;

    public AuthService(
        IUsersRepository usersRepository,
        IJwtService jwtService,
        IPasswordService passwordService,
        ILibraryRepository libraryRepository,
        IConfiguration configuration)
    {
        _usersRepository = usersRepository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _libraryRepository = libraryRepository;
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<string> RegisterAsync(UserAuthDto request)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var sqlTransaction = await connection.BeginTransactionAsync();

        try
        {
            if (await _usersRepository.IsUserByEmailAsync(connection, sqlTransaction, request.Email))
            {
                throw new UserAlreadyExistsException();
            }

            var newUser = await _usersRepository.AddUserAsync(connection, sqlTransaction, new User
            {
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(new User { Email = request.Email }, request.Password)
            });

            await _libraryRepository.CreateUserLibraryAsync(connection, sqlTransaction, newUser.UserId);
            await sqlTransaction.CommitAsync();
            return _jwtService.CreateToken(newUser);
        }
        catch
        {
            await sqlTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<string> LoginAsync(UserAuthDto request)
    {
        var user = await _usersRepository.GetUserByEmailAsync(request.Email);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        if(!_passwordService.IsVerifyHashedPassword(user, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        return _jwtService.CreateToken(user);
    }
}
