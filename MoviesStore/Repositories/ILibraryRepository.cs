public interface ILibraryRepository
{
    public Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync (int userId, string visibility, string sort);
    public Task<bool> SetMovieVisibilityAsync(int userId, int movieId, bool isHidden);
    public Task<bool> SetSortLibrarySettingsAsync(int userId, string sortType);
    public Task<string> GetSortTypeLibrarySettingsAsync(int userId);
    public Task<bool> SetVisibleLibraryMovieAsync(int userId, string visibleType);
    public Task<string> GetVisibleLibraryMovieAsync(int userId);
}