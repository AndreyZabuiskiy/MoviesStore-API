public interface IUserBalanceService
{
    public Task<CurrentUserBalanceDto> GetBalanceByIdAsync (int id);
    public Task<TopUpBalanceResponseDto> TopUpBalanceAsync (int id, decimal amount);
}
