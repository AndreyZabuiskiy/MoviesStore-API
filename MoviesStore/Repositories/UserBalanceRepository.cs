using Npgsql;

public class UserBalanceRepository : IUserBalanceRepository
{
    private readonly string _connectionString;

    public UserBalanceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<decimal?> GetBalanceAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT balance
            FROM users
            WHERE user_id = @id
        ", connection);

        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Integer, id);

        var result = await command.ExecuteScalarAsync();

        if (result == null)
        {
            return null;
        }

        return (decimal)result;
    }

    public async Task<bool> TopUpBalanceAsync(int id, decimal amount)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            UPDATE users 
            SET balance = balance + @amount
            WHERE user_id = @id;
        ", connection);

        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Integer, id);
        command.Parameters.AddWithValue("@amount", NpgsqlTypes.NpgsqlDbType.Numeric, amount);

        var rows = await command.ExecuteNonQueryAsync();
        
        return rows > 0;
    }
}
