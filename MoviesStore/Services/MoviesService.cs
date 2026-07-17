public class MoviesService : IMoviesService
{
    private readonly IMoviesRepository _moviesRepository;

    public MoviesService(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    public async Task<List<MovieCardDto>> GetAllMoviesAsync()
    {
        var movies = await _moviesRepository.GetAllAsync();

        var moviesResponse = new List<MovieCardDto>();
        
        foreach(var movie in movies)
        {
            moviesResponse.Add(new MovieCardDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                DirectorId = movie.DirectorId,
                ReleaseDate = movie.ReleaseDate,
                DurationMinutes = movie.DurationMinutes
            });
        }

        return moviesResponse;
    }
}