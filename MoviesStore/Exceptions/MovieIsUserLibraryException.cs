public class MovieIsUserLibraryException : Exception
{
    public MovieIsUserLibraryException(int movieId) : base($"Movie is in the user's library") {}
}