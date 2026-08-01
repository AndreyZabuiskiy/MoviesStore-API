using Npgsql;

public interface IUserBalanceRepository
{
    public Task<decimal?> GetBalanceAsync(int id);
    public Task DecreaseBalanceAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        int userId,
        decimal amount);
    public Task<decimal?> GetBalanceForUpdateAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        int id);
    public Task IncreaseBalanceAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        int id,
        decimal amount);
}
