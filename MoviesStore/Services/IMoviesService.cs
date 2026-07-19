public interface IMoviesService
{
    public Task<List<MovieCardDto>> GetAllMoviesAsync();
    public Task<MovieDetailsDto> GetMovieDetailsAsync(int id);
}