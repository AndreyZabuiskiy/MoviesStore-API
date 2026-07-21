using System.Security.Claims;
using System.Threading.Tasks;
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
    public async Task<ActionResult> GetBalance()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        var balance = await _balanceService.GetBalanceByIdAsync(userId);
        return Ok(balance);
    }
}
