public interface IUserBalanceRepository
{
    public Task<User> GetBalanceAsync(int id);
    public Task<bool> TopUpBalanceAsync(int id, decimal amount);
}