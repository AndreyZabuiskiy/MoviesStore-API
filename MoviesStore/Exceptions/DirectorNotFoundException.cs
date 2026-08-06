public class DirectorNotFoundException : Exception
{
    public DirectorNotFoundException(int id) : base($"Director with {id} not found.") {}
}