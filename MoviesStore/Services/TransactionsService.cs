public class TransactionsService : ITransactionsService
{
    private readonly ITransactionsRepository _transactionRepository;

    public TransactionsService(ITransactionsRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }


    public async Task<int> CreateTransactionAsync(UserTransaction transaction)
    {
        if(transaction.TransactionType == TransactionType.TopUp)
        {
            transaction.BalanceAfter = transaction.BalanceBefore + transaction.Amount;
        } else if (transaction.TransactionType == TransactionType.Purchase || transaction.TransactionType == TransactionType.Refund)
        {
            transaction.BalanceAfter = transaction.BalanceBefore - transaction.Amount;
        }
        
        var transactionId = await _transactionRepository.CreateTransactionAsync(transaction);
        return transactionId;
    }
}