public interface IAuthService
{
    public Task<string> RegisterAsync(UserAuthDto request);
    public Task<string> LoginAsync(UserAuthDto request);
}