public class MovieCardDto
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }

    public DirectorCardDto Director { get; set; }
}