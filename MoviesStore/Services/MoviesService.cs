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

        var moviesDto = new List<MovieCardDto>();
        
        foreach(var movie in movies)
        {
            moviesDto.Add(new MovieCardDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                ReleaseDate = movie.ReleaseDate,
                DurationMinutes = movie.DurationMinutes,
                Price = movie.Price,
                DirectorId = movie.Director.DirectorId,
                DirectorFirstName = movie.Director.FirstName,
                DirectorLastName = movie.Director.LastName
            });
        }

        return moviesDto;
    }

    public async Task<MovieDetails> GetMovieDetailsAsync(int id)
    {
        var movie = await _moviesRepository.GetMovieDetailsAsync(id);

        var movieResponse = new MovieDetails
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            ReleaseDate = movie.ReleaseDate,
            Budget = movie.Budget,
            BoxOffice = movie.BoxOffice,
            AgeRating = movie.AgeRating,
            ImdbRating = movie.ImdbRating,
            Price = movie.Price,
            DirectorId = movie.Director.DirectorId,
            DirectorFirstName = movie.Director.FirstName,
            DirectorLastName = movie.Director.LastName
        };

        return movieResponse;
    }
}
