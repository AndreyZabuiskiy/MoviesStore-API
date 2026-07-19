public class MovieDetails
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

    public int DirectorId { get; set; }
    public string DirectorFirstName { get; set; }
    public string DirectorLastName { get; set; }
}