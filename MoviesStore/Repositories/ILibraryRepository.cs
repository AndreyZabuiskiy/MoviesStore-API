public interface ILibraryRepository
{
    public Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync (int userId);
    public Task<bool> SetMovieVisibilityAsync(int userId, int movieId, bool isHidden);
}