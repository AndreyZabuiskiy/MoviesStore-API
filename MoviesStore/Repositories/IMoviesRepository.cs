public interface IMoviesRepository
{
    public Task<List<Movie>> GetAllAsync();
    public Task<Movie> GetByIdAsync(int id);
}