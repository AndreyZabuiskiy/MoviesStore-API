public interface ILibraryRepository
{
    public Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync (int userId, string visibilityType, string sortType);
    public Task<bool> SetMovieVisibilityAsync(int userId, int movieId, bool isHidden);
    public Task<bool> SetSortTypeLibrarySettingsAsync(int userId, string sortType);
    public Task<string> GetSortTypeLibrarySettingsAsync(int userId);
    public Task<bool> SetVisibleTypeLibrarySettingsAsync(int userId, string visibleType);
    public Task<string> GetVisibleTypeLibrarySettingsAsync(int userId);
}