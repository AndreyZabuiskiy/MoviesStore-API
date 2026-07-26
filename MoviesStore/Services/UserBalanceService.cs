using Npgsql;

public class UserBalanceService : IUserBalanceService
{
    private readonly IUserBalanceRepository _balanceRepository;
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly string _connectionString;
    const decimal MAX_TOP_UP_AMOUNT = 1_000_000m;

    public UserBalanceService(
        IUserBalanceRepository userBalanceRepository,
        ITransactionsRepository transactionsRepository,
        IConfiguration configuration)
    {
        _balanceRepository = userBalanceRepository;
        _transactionsRepository = transactionsRepository;
        _connectionString = configuration.GetConnectionString("Postgres");
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
        if (amount <= 0 || amount > MAX_TOP_UP_AMOUNT)
            throw new InvalidTopUpAmountException(amount);

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var sqlTransaction = await connection.BeginTransactionAsync();

        try
        {
            var balance = await GetBalanceForTransactionAsync(connection, sqlTransaction, id);

            var transaction = await _transactionsRepository.AddTransactionAsync(
                connection,
                sqlTransaction,
                new UserTransaction
                {
                    TransactionType = TransactionType.TopUp,
                    UserId = id,
                    BalanceBefore = balance,
                    BalanceAfter = balance + amount,
                    Amount = amount
                }
            );

            await _balanceRepository.IncreaseBalanceAsync(connection, sqlTransaction, id, amount);

            var topUpDto = new TopUpBalanceResponseDto
            {
                TransactionId = transaction.TransactionId,
                UserId = id,
                Amount = amount,
                BalanceBefore = transaction.BalanceBefore,
                BalanceAfter = transaction.BalanceAfter
            };

            await sqlTransaction.CommitAsync();
            return topUpDto;
        }
        catch
        {
            await sqlTransaction.RollbackAsync();
            throw;
        }
    }

    private async Task<decimal> GetBalanceForTransactionAsync (
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        int id)
    {
        var balance = await _balanceRepository.GetBalanceForUpdateAsync(connection, sqlTransaction, id);

        if (balance is null)
            throw new UserNotFoundException(id);

        return balance.Value;
    }
}
