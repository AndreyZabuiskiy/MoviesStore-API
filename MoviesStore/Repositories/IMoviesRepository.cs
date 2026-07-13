public interface IMoviesRepository
{
    public Task<IEnumerable<Movie>> GetAllAsync();
}