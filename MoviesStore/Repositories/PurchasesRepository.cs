using Microsoft.VisualBasic;
using Npgsql;

public class PurchasesRepository : IPurchasesRepository
{
    private readonly string _connectionString;

    public PurchasesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<Purchase> AddPurchaseAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        Purchase purchase)
    {
        await using var command = new NpgsqlCommand(@"
            INSERT INTO purchases (user_id, movie_id, price_paid, transaction_id)
            VALUES
            (@user_id, @movie_id, @price_paid, @transaction_id)
            RETURNING purchase_id, user_id, movie_id, purchased_at, price_paid, transaction_id;
        ", connection, sqlTransaction);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, purchase.UserId);
        command.Parameters.AddWithValue("@movie_id", NpgsqlTypes.NpgsqlDbType.Integer, purchase.MovieId);
        command.Parameters.AddWithValue("@price_paid", NpgsqlTypes.NpgsqlDbType.Numeric, purchase.PricePaid);
        command.Parameters.AddWithValue("@transaction_id", NpgsqlTypes.NpgsqlDbType.Integer, purchase.TransactionId);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Purchase
            {
                PurchaseId = reader.GetInt32(reader.GetOrdinal("purchase_id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                MovieId = reader.GetInt32(reader.GetOrdinal("movie_id")),
                PurchasedAt = reader.GetFieldValue<DateTime>(reader.GetOrdinal("purchased_at")),
                PricePaid = reader.GetDecimal(reader.GetOrdinal("price_paid")),
                TransactionId = reader.GetInt32(reader.GetOrdinal("transaction_id"))
            };
        }

        return null;
    }

    public async Task<HistoryProduct> GetHistoryProductByTransactionId(int transactionId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT p.movie_id, m.title
			FROM purchases p
			INNER JOIN movies m
			ON p.movie_id = m.movie_id
			WHERE transaction_id = @transaction_id;
        ", connection);
        
        command.Parameters.AddWithValue("@transaction_id", NpgsqlTypes.NpgsqlDbType.Integer, transactionId);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new HistoryProduct
            {
                MovieId = reader.GetInt32(reader.GetOrdinal("movie_id")),
                Title = reader.GetString(reader.GetOrdinal("title"))
            };
        }

        return null;
    }
}
