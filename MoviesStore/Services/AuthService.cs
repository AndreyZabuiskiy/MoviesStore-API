using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUsersRepository _usersRepository;

    public AuthService(IConfiguration configuration, IUsersRepository usersRepository)
    {
        _configuration = configuration;
        _usersRepository = usersRepository;
    }

    public async Task<string> RegisterAsync(UserAuthDto request)
    {
        if (await _usersRepository.IsUserByEmailAsync(request.Email))
        {
            return null;
        }

        var user = new User
        {
            Email = request.Email
        };

        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user,  request.Password);

        var newUser = await _usersRepository.RegisterAsync(request.Email, hashedPassword);

        return CreateToken(newUser);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}