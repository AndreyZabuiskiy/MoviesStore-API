public interface ITransactionsRepository
{
    public Task<int> CreateTransactionAsync(UserTransaction transaction);
}