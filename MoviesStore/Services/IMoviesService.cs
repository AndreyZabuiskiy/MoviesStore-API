public interface IMoviesService
{
    public Task<List<MovieCardDto>> GetAllMoviesAsync();
}