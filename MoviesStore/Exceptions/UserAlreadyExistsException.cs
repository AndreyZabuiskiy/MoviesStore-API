public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException() : base("User with this email already exists.") { }
}