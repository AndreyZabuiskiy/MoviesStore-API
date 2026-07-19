public interface IMoviesRepository
{
    public Task<List<Movie>> GetAllAsync();
    public Task<Movie> GetMovieDetailsAsync(int id);
}