using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<LibraryResponseDto>> GetLibrary()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        var movies = await _libraryService.GetLibraryByUserIdAsync(userId);
        return Ok(movies);
    }
}
