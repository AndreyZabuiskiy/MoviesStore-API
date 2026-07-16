using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> RegisterAsync([FromBody] UserAuthDto request)
    {
        var token = await _authService.RegisterAsync(request);

        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] UserAuthDto request)
    {
        var token = await _authService.LoginAsync(request);

        return token;
    }
}
