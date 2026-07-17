using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesService _moviesService;

    public MoviesController(IMoviesService moviesService)
    {
        _moviesService = moviesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MovieCardDto>>> GetAllMovies()
    {
        var movies = await _moviesService.GetAllMoviesAsync();
        return Ok(movies);
    }
}