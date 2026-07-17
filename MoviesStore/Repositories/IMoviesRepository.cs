public interface IMoviesRepository
{
    public Task<List<Movie>> GetAllAsync();
}