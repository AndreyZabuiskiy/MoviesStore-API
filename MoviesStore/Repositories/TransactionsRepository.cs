using Npgsql;

public class TransactionsRepository : ITransactionsRepository
{
    private readonly string _connectionString;

    public TransactionsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<int> CreateTransactionAsync(UserTransaction transaction)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            INSERT INTO user_transactions
            (
                transaction_type_id,
                user_id,
                amount,
                balance_before,
                balance_after
            )
            VALUES
            (
                @transaction_type_id,
                @user_id,
                @amount,
                @balance_before,
                @balance_after
            )
            RETURNING transaction_id;
        ", connection);

        command.Parameters.AddWithValue("@transaction_type_id", NpgsqlTypes.NpgsqlDbType.Integer, (int)transaction.TransactionType);
        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, transaction.UserId);
        command.Parameters.AddWithValue("@amount", NpgsqlTypes.NpgsqlDbType.Numeric, transaction.Amount);
        command.Parameters.AddWithValue("@balance_before", NpgsqlTypes.NpgsqlDbType.Numeric, transaction.BalanceBefore);
        command.Parameters.AddWithValue("@balance_after", NpgsqlTypes.NpgsqlDbType.Numeric, transaction.BalanceAfter);

        var transactionId = (int)await command.ExecuteScalarAsync();

        return transactionId;
    }
}
