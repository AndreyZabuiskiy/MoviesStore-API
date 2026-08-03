public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IPurchasesRepository _purchasesRepository;

    public TransactionsService(
        ITransactionsRepository transactionsRepository,
        IPurchasesRepository purchasesRepository)
    {
        _transactionsRepository = transactionsRepository;
        _purchasesRepository = purchasesRepository;
    }

    public async Task<TransactionsHistoryResponseDto> GetHistore(int userId)
    {
        var transactions = await _transactionsRepository.GetTransactionsAsync(userId);

        var userHistoreList = new TransactionsHistoryResponseDto()
        {
            Transactions = new List<UserHistoryOperationModelDto>()
        };

        foreach(var transaction in transactions)
        {
            var product = await _purchasesRepository.GetHistoryProductByTransactionId(transaction.TransactionId);

            HistoryProductDto? productDto = product is null
            ? null
            : new HistoryProductDto
            {
                MovieId = product.MovieId,
                Title = product.Title
            };

            userHistoreList.Transactions.Add(new UserHistoryOperationModelDto
            {
                TransactionDto = new UserTransactionDto
                {
                    TransactionId = transaction.TransactionId,
                    UserId = transaction.UserId,
                    TransactionType = transaction.TransactionType,
                    TransactionAt = transaction.TransactionAt,
                    Amount = transaction.Amount,
                    BalanceBefore = transaction.BalanceBefore,
                    BalanceAfter = transaction.BalanceAfter
                },
                ProductDto = productDto
            });
        };

        return userHistoreList;
    }
}
