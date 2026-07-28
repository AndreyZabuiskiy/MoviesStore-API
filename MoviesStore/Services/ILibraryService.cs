public interface ILibraryService
{
    public Task<LibraryResponseDto> GetLibraryByUserIdAsync(int id);
    public Task SetMovieVisibilityAsync(int userId, int movieId, bool isHidden);
}