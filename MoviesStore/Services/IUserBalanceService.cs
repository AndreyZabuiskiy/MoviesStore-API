public interface IUserBalanceService
{
    public Task<decimal> GetBalanceByIdAsync (int id);
    public Task<TopUpBalanceResponseDto> TopUpBalanceAsync (int id, decimal amount);
}
