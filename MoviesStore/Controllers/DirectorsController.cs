using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DirectorsController : ControllerBase
{
    private readonly IDirectorsService _directorsService;

    public DirectorsController(IDirectorsService directorsService)
    {
        _directorsService = directorsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DirectorCardDto>>> GetDirectors()
    {
        var directors = await _directorsService.GetDirectors();
        return directors;
    }
}
