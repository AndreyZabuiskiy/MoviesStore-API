public class UserBalanceService : IUserBalanceService
{
    private readonly IUserBalanceRepository _balanceRepository;
    private readonly ITransactionsService _transactionsService;

    public UserBalanceService(IUserBalanceRepository userBalanceRepository, ITransactionsService transactionsService)
    {
        _balanceRepository = userBalanceRepository;
        _transactionsService = transactionsService;
    }

    public async Task<decimal> GetBalanceByIdAsync (int id)
    {
        var balance = await _balanceRepository.GetBalanceAsync(id);

        if (balance is null)
            throw new UserNotFoundException(id);

        return balance.Value;
    }

    public async Task<TopUpBalanceResponseDto> TopUpBalanceAsync(int id, decimal amount)
    {
        var balance = await GetBalanceByIdAsync(id);

        var transactionId = await _transactionsService.CreateTransactionAsync(new UserTransaction
        {
            TransactionType = TransactionType.TopUp,
            UserId = id,
            BalanceBefore = balance,
            Amount = amount
        });

        await _balanceRepository.TopUpBalanceAsync(id, amount);

        var topUpDto = new TopUpBalanceResponseDto
        {
            TransactionId = transactionId,
            UserId = id,
            Amount = amount,
            BalanceBefore = balance,
            BalanceAfter = balance + amount
        };

        return topUpDto;
    }
}
