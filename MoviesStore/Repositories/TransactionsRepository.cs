using Npgsql;

public class TransactionsRepository : ITransactionsRepository
{
    private readonly string _connectionString;

    public TransactionsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<UserTransaction> AddTransactionAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        UserTransaction transaction)
    {
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
            RETURNING transaction_id, transaction_type_id, transaction_at, user_id, amount, balance_before, balance_after;
        ", connection, sqlTransaction);

        command.Parameters.AddWithValue("@transaction_type_id", NpgsqlTypes.NpgsqlDbType.Integer, (int)transaction.TransactionType);
        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, transaction.UserId);
        command.Parameters.AddWithValue("@amount", NpgsqlTypes.NpgsqlDbType.Numeric, transaction.Amount);
        command.Parameters.AddWithValue("@balance_before", NpgsqlTypes.NpgsqlDbType.Numeric, transaction.BalanceBefore);
        command.Parameters.AddWithValue("@balance_after", NpgsqlTypes.NpgsqlDbType.Numeric, transaction.BalanceAfter);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new UserTransaction
            {
                TransactionId = reader.GetInt32(reader.GetOrdinal("transaction_id")),
                TransactionType = (TransactionType)reader.GetInt32(reader.GetOrdinal("transaction_type_id")),
                TransactionAt = reader.GetFieldValue<DateTime>(reader.GetOrdinal("transaction_at")),
                Amount = reader.GetDecimal(reader.GetOrdinal("amount")),
                BalanceBefore = reader.GetDecimal(reader.GetOrdinal("balance_before")),
                BalanceAfter = reader.GetDecimal(reader.GetOrdinal("balance_after"))
            };
        }

        return null;
    }

    public async Task<List<UserTransaction>> GetTransactionsAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT transaction_id, transaction_type_id, transaction_at,
                user_id, amount, balance_before, balance_after
            FROM user_transactions
            WHERE user_id = @user_id;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        await using var reader = await command.ExecuteReaderAsync();

        var transactionIdOrdinal = reader.GetOrdinal("transaction_id");
        var transactionTypeOrdinal = reader.GetOrdinal("transaction_type_id");
        var transactionAtOrdinal = reader.GetOrdinal("transaction_at");
        var userIdOrdinal = reader.GetOrdinal("user_id");
        var amountOrdinal = reader.GetOrdinal("amount");
        var balanceBeforeOrdinal = reader.GetOrdinal("balance_before");
        var balanceAfterOrdinal = reader.GetOrdinal("balance_after");

        var transactions = new List<UserTransaction>();

        while(await reader.ReadAsync())
        {
            transactions.Add(new UserTransaction
            {
                TransactionId = reader.GetInt32(transactionIdOrdinal),
                TransactionType = (TransactionType)reader.GetInt32(transactionTypeOrdinal),
                TransactionAt = reader.GetFieldValue<DateTime>(transactionAtOrdinal),
                UserId = reader.GetInt32(userIdOrdinal),
                Amount = reader.GetDecimal(amountOrdinal),
                BalanceBefore = reader.GetDecimal(balanceBeforeOrdinal),
                BalanceAfter = reader.GetDecimal(balanceAfterOrdinal)
            });
        }

        return transactions;
    }
}
