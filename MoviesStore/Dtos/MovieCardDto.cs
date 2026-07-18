public class MovieCardDto
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public int DurationMinutes { get; set; }

    public int DirectorId { get; set; }
    public string DirectorFirstName { get; set; }
    public string DirectorLastName { get; set; }
}