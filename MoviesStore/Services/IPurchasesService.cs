public interface IPurchasesService
{
    public Task<Purchase> AddPurchaseAsync(int userId, int movieId);
}