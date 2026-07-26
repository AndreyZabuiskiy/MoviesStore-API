public interface IUserBalanceRepository
{
    public Task<decimal?> GetBalanceAsync(int id);
    public Task<bool> TopUpBalanceAsync(int id, decimal amount);
}