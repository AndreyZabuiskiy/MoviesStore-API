public class InsufficientFundsInBalanceException : Exception
{
    public InsufficientFundsInBalanceException() : base($"Not enough money in the balance.") {}
}