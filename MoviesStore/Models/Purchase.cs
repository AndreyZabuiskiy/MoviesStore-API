public class Purchase
{
    public int PurchaseId { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public int TransactionId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public decimal PricePaid { get; set; }
}