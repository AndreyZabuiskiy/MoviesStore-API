public class MovieDetailsDto
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }
    public int Budget { get; set; }
    public int BoxOffice { get; set; }
    public string AgeRating { get; set; }
    public double ImdbRating { get; set; }
    public decimal Price { get; set; }

    public DirectorCardDto Director { get; set; }
}