using Npgsql;
using NpgsqlTypes;

public class UsersRepository(IConfiguration configuration) : IUsersRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("Postgres");

    public async Task<bool> IsUserByEmailAsync(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT EXISTS(
                SELECT 1
                FROM users
                WHERE email = @email
            )
        ", connection);

        command.Parameters.AddWithValue("email", NpgsqlDbType.Text, email);

        return (bool)await command.ExecuteScalarAsync();
    }

    public async Task<User> LoginAsync(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT email, passwd
            FROM users
            WHERE email = @email
        ", connection);

        command.Parameters.AddWithValue("@email", NpgsqlDbType.Text, email);
        
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                Email = reader.GetString(reader.GetOrdinal("email")),
                PasswordHash = reader.GetString(reader.GetOrdinal("passwd"))
            };
        }

        throw new Exception("User was not exists");
    }

    public async Task<User> RegisterAsync(string email, string password)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            INSERT INTO users (email, passwd, user_role, balanse)
            VALUES
            (
                @email,
                @password,
                'user',
                0
            )
            RETURNING user_id, email, user_role, balanse;
        ", connection);

        command.Parameters.AddWithValue("@email", NpgsqlDbType.Text, email);
        command.Parameters.AddWithValue("@password", NpgsqlDbType.Text, password);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Role = reader.GetString(reader.GetOrdinal("user_role")),
                Balance = reader.GetDecimal(reader.GetOrdinal("balanse"))
            };
        }

        throw new Exception("User was not created");
    }
}
