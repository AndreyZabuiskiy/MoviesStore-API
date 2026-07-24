public class TopUpBalanceResponseDto
{
    public int TransactionId { get; set; }
    public int UserId { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public decimal Amount { get; set; }
}