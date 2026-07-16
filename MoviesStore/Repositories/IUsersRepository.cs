public interface IUsersRepository
{
    public Task<bool> IsUserByEmailAsync(string email);
    public Task<User> AddUserAsync(User user);
    public Task<User> GetUserByEmailAsync(string email);
}