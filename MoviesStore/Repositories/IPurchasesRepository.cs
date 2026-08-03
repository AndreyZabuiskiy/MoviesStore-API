using Npgsql;

public interface IPurchasesRepository
{
    public Task<Purchase> AddPurchaseAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        Purchase purchase);

    public Task<HistoryProduct> GetHistoryProductByTransactionId(int transactionId);
}