public class LibraryMovieReadModel
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public int DurationMinutes { get; set; }
    public double ImdbRating { get; set; }
    public decimal Price { get; set; }
    public bool IsHidden { get; set; }
    public DateTime AddedAt { get; set; }
}
