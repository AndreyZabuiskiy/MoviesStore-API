public interface ITransactionsService
{
    public Task<int> CreateTransactionAsync(UserTransaction transaction);
}