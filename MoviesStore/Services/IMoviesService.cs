public interface IMoviesService
{
    public Task<List<MovieCardDto>> GetAllMoviesAsync();
    public Task<MovieDetails> GetMovieDetailsAsync(int id);
}