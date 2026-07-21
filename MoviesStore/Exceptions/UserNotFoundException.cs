public class UserNotFoundException : Exception
{
    public UserNotFoundException(int id) : base($"User with {id} not found.") {}
}