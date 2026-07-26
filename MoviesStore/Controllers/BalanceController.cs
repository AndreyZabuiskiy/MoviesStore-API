using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class BalanceController : ControllerBase
{
    private readonly IUserBalanceService _balanceService;

    public BalanceController(IUserBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserBalanceDto>> GetBalance()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        var balance = await _balanceService.GetBalanceByIdAsync(userId);
        return Ok(new UserBalanceDto
        {
            Balance = balance
        });
    }

    [Authorize]
    [HttpPost("top-up")]
    public async Task<ActionResult<string>> TopUpBalance([FromBody]TopUpBalanceRequestDto request)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        var balanceDto = await _balanceService.TopUpBalanceAsync(userId, request.Amount);

        return Ok(balanceDto);
    }
}
