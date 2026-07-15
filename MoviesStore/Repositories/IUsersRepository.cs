public interface IUsersRepository
{
    public Task<bool> IsUserByEmailAsync(string email);
    public Task<User> RegisterAsync(string email, string password);
}