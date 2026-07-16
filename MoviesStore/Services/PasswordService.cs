using Microsoft.AspNetCore.Identity;

public class PasswordService : IPasswordService
{
    public string HashPassword(User user, string password)
    {
        return new PasswordHasher<User>()
            .HashPassword(user,  password);
    }

    public bool IsVerifyHashedPassword(User user, string providerPassword)
    {
        return new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, providerPassword) == PasswordVerificationResult.Success;
    }
}