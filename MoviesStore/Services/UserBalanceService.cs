public class UserBalanceService : IUserBalanceService
{
    private readonly IUserBalanceRepository _balanceRepository;

    public UserBalanceService(IUserBalanceRepository userBalanceRepository)
    {
        _balanceRepository = userBalanceRepository;
    }

    public async Task<CurrentUserBalanceDto> GetBalanceByIdAsync (int id)
    {
        var user = await _balanceRepository.GetBalanceAsync(id);

        if (user == null)
            throw new UserNotFoundException(id);

        return new CurrentUserBalanceDto
        {
            UserId = id,
            Balance = user.Balance
        };
    }
}