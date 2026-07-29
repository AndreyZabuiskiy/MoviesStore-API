public class LibraryService : ILibraryService
{    
    private readonly ILibraryRepository _libraryRepository;

    public LibraryService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<LibraryResponseDto> GetLibraryByUserIdAsync(int id, string visibilityQuery, string sortQuery)
    {
        var sortType = SortTypeLibraryMovie.SortTypesList.FirstOrDefault(s => s == sortQuery);
        if (sortType != null)
        {
            await _libraryRepository.SetSortLibrarySettingsAsync(id, sortType);
        } 
        else
        {
            sortType = await _libraryRepository.GetSortTypeLibrarySettingsAsync(id);
        }

        var visibleType = VisibleTypeLibraryMovie.VisibleTypesList.FirstOrDefault(v => v == visibilityQuery);
        if (visibleType != null)
        {
            await _libraryRepository.SetVisibleLibraryMovieAsync(id, visibleType);
        } 
        else
        {
            visibleType = await _libraryRepository.GetVisibleLibraryMovieAsync(id);
        }

        var moviesModel = await _libraryRepository.GetLibraryMoviesAsync(id, visibleType, sortType);

        var libraryMoviesDto = new List<LibraryMovieDto>();
        foreach(var movie in moviesModel)
        {
            libraryMoviesDto.Add(new LibraryMovieDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                DurationMinutes = movie.DurationMinutes,
                ImdbRating = movie.ImdbRating,
                Price = movie.Price,
                IsHidden = movie.IsHidden,
                AddedAt = movie.AddedAt
            });
        }

        return new LibraryResponseDto
        {
            SortType = sortType,
            VisibleType = visibleType,
            Movies = libraryMoviesDto
        };
    }

    public async Task SetMovieVisibilityAsync(int userId, int movieId, bool isHidden)
    {
        var isUpdateSuccess = await _libraryRepository.SetMovieVisibilityAsync(userId, movieId, isHidden);

        if(!isUpdateSuccess)
            throw new MovieNotFoundInLibraryException(movieId);
    }
}
