public class UserTransactionDto
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime TransactionAt { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
}