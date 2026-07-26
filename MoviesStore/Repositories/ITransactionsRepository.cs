using Npgsql;

public interface ITransactionsRepository
{
    public Task<UserTransaction> AddTransactionAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        UserTransaction transaction);
}