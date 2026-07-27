public interface ILibraryRepository
{
    public Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync (int userId);
}