public interface IPasswordService
{
    public string HashPassword(User user, string password);
    public bool IsVerifyHashedPassword(User user, string providerPassword);
}