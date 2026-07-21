public interface IUserBalanceRepository
{
    public Task<User> GetBalanceAsync(int id);
}