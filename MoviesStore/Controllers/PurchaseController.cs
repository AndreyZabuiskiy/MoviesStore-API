using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchasesService _purchaseService;

    public PurchaseController(IPurchasesService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddPurchase([FromBody] PurchaseRequestDto request)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        var purchase = await _purchaseService.AddPurchaseAsync(userId, request.MovieId);
        return Ok(purchase);
    }
}
