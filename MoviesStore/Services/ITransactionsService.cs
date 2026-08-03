public interface ITransactionsService
{
    public Task<TransactionsHistoryResponseDto> GetHistore(int userId);
}