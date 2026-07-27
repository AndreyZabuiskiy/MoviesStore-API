public class LibraryService : ILibraryService
{
    private readonly ILibraryRepository _libraryRepository;

    public LibraryService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<LibraryResponseDto> GetLibraryByUserIdAsync(int id)
    {
        var moviesModel = await _libraryRepository.GetLibraryMoviesAsync(id);
        var libraryMoviesDto = new List<LibraryMovieDto>();

        foreach(var movie in moviesModel)
        {
            libraryMoviesDto.Add(new LibraryMovieDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                AddedAt = movie.AddedAt
            });
        }

        return new LibraryResponseDto
        {
            Movies = libraryMoviesDto
        };
    }
}
