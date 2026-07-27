public interface ILibraryService
{
    public Task<LibraryResponseDto> GetLibraryByUserIdAsync(int id);
}