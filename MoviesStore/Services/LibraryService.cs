using Npgsql;

public class LibraryService : ILibraryService
{    
    private readonly ILibraryRepository _libraryRepository;

    public LibraryService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public async Task<LibraryResponseDto> GetLibraryByUserIdAsync(int id, string? visibilityQuery, string? sortQuery)
    {
        var visibleType = await ResolveVisibleTypeAsync(id, visibilityQuery);
        var sortType = await ResolveSortTypeAsync(id, sortQuery);

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

    private async Task<string> ResolveSortTypeAsync(int id, string? sortQuery)
    {
        var sortType = sortQuery is null ? "" : sortQuery;

        if (SortTypeLibraryMovies.IsSortValid(sortQuery))
        {
            await _libraryRepository.SetSortTypeLibrarySettingsAsync(id, sortType);
        }
        else
        {
            sortType = await _libraryRepository.GetSortTypeLibrarySettingsAsync(id);
        }

        return sortType;
    }

    private async Task<string> ResolveVisibleTypeAsync(int id, string? visibleQuery)
    {
        var visibleType = visibleQuery is null ? "" : visibleQuery;

        if (VisibleTypeLibraryMovies.IsVisibleValid(visibleQuery))
        {
            await _libraryRepository.SetVisibleTypeLibrarySettingsAsync(id, visibleType);
        } 
        else
        {
            visibleType = await _libraryRepository.GetVisibleTypeLibrarySettingsAsync(id);
        }

        return visibleType;
    }
}
