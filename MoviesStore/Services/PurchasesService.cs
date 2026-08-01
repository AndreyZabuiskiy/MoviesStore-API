using Npgsql;

public class PurchasesService : IPurchasesService
{
    private readonly string _connectionString;
    private readonly IMoviesRepository _moviesRepository;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IUserBalanceRepository _balanceRepository;
    private readonly IPurchasesRepository _purchasesRepository;
    private readonly ITransactionsRepository _transactionsRepository;

    public PurchasesService(
        IMoviesRepository moviesRepository,
        ILibraryRepository libraryRepository,
        IUserBalanceRepository balanceRepository,
        IPurchasesRepository purchasesRepository,
        ITransactionsRepository transactionsRepository,
        IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
        _moviesRepository = moviesRepository;
        _libraryRepository = libraryRepository;
        _balanceRepository = balanceRepository;
        _purchasesRepository = purchasesRepository;
        _transactionsRepository = transactionsRepository;
    }

    public async Task<Purchase> AddPurchaseAsync(int userId, int movieId)
    {
        var movie = await _moviesRepository.GetByIdAsync(movieId);

        if(movie is null)
            throw new MovieNotFoundException(movieId);

        if(await _libraryRepository.IsMovieInUserLibrary(userId, movieId))
            throw new MovieIsUserLibraryException(movieId);
        
        var balance = await _balanceRepository.GetBalanceAsync(userId);

        if (movie.Price > balance)
            throw new NotEnoughMoneyException();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var sqlTransaction = await connection.BeginTransactionAsync();

        try
        {
            var transaction = await _transactionsRepository.AddTransactionAsync(connection, sqlTransaction, new UserTransaction
            {
                UserId = userId,
                TransactionType = TransactionType.Purchase,
                Amount = movie.Price,
                BalanceBefore = balance.Value,
                BalanceAfter = balance.Value - movie.Price
            });

            await _balanceRepository.DecreaseBalanceAsync(connection, sqlTransaction, userId, movie.Price);

            var purchase = await _purchasesRepository.AddPurchaseAsync(connection, sqlTransaction, new Purchase
            {
                UserId = userId,
                MovieId = movie.MovieId,
                PricePaid = movie.Price,
                TransactionId = transaction.TransactionId
            });

            await _libraryRepository.AddMovieToLibraryAsync(connection, sqlTransaction, userId, movie.MovieId);
            await sqlTransaction.CommitAsync();
            return purchase;
        }
        catch
        {
            await sqlTransaction.RollbackAsync();
            throw;
        }
    }
}
