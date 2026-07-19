public class MovieNotFoundException : Exception
{
    public MovieNotFoundException(int id) : base($"Movie with {id} not found.") {}
}