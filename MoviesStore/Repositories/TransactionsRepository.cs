using Npgsql;

public class TransactionsRepository : ITransactionsRepository
{
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
}
