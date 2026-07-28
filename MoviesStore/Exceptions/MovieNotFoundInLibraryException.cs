public class MovieNotFoundInLibraryException : Exception
{
    public MovieNotFoundInLibraryException(int id) : base($"Movie with id: {id}, not found in your library.") {}
}