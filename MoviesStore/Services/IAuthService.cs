public interface IAuthService
{
    public Task<string> RegisterAsync(UserAuthDto request);
}