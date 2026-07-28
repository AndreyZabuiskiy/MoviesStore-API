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
    public async Task<ActionResult<LibraryResponseDto>> GetLibraryAsync()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        var movies = await _libraryService.GetLibraryByUserIdAsync(userId);
        return Ok(movies);
    }

    [Authorize]
    [HttpPatch("movie/visibility/{movieId}")]
    public async Task<ActionResult> SetMovieVisibilityAsync(int movieId, [FromBody]UpdateMovieVisibilityRequestDto request)
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)
        );

        await _libraryService.SetMovieVisibilityAsync(userId, movieId, request.IsHidden);
        return NoContent();;
    }
}
