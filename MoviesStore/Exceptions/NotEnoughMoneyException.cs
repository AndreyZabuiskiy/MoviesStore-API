public class NotEnoughMoneyException : Exception
{
    public NotEnoughMoneyException() : base($"Not enough money in the balance.") {}
}