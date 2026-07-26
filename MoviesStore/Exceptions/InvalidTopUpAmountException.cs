public class InvalidTopUpAmountException : Exception
{
    public InvalidTopUpAmountException(decimal amount) : base($"Invalid top-up amount: {amount}. Amount must be between 1 and 1,000,000. .") {}
}