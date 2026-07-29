public interface ILibraryService
{
    public Task<LibraryResponseDto> GetLibraryByUserIdAsync(int id, string visibilityQuery, string sortQuery);
    public Task SetMovieVisibilityAsync(int userId, int movieId, bool isHidden);
}