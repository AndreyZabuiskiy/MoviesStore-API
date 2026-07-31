using Npgsql;

public interface IUsersRepository
{
    public Task<bool> IsUserByEmailAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        string email);
    public Task<User> AddUserAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        User user);
    public Task<User> GetUserByEmailAsync(string email);
}