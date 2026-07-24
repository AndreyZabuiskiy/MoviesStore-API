public class UserBalanceService : IUserBalanceService
{
    private readonly IUserBalanceRepository _balanceRepository;
    private readonly ITransactionsService _transactionsService;

    public UserBalanceService(IUserBalanceRepository userBalanceRepository, ITransactionsService transactionsService)
    {
        _balanceRepository = userBalanceRepository;
        _transactionsService = transactionsService;
    }

    public async Task<CurrentUserBalanceDto> GetBalanceByIdAsync (int id)
    {
        var userBalance = await _balanceRepository.GetBalanceAsync(id);

        if (userBalance == null)
            throw new UserNotFoundException(id);

        return new CurrentUserBalanceDto
        {
            UserId = id,
            Balance = userBalance.Balance
        };
    }

    public async Task<TopUpBalanceResponseDto> TopUpBalanceAsync(int id, decimal amount)
    {
        var userBalance = await GetBalanceByIdAsync(id);

        var transactionId = await _transactionsService.CreateTransactionAsync(new UserTransaction
        {
            TransactionType = TransactionType.TopUp,
            UserId = userBalance.UserId,
            BalanceBefore = userBalance.Balance,
            Amount = amount
        });

        await _balanceRepository.TopUpBalanceAsync(id, amount);

        var topUpDto = new TopUpBalanceResponseDto
        {
            TransactionId = transactionId,
            UserId = id,
            Amount = amount,
            BalanceBefore = userBalance.Balance,
            BalanceAfter = userBalance.Balance + amount
        };

        return topUpDto;
    }
}