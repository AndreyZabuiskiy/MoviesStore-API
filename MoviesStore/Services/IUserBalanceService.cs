public interface IUserBalanceService
{
    public Task<CurrentUserBalanceDto> GetBalanceByIdAsync (int id);
}