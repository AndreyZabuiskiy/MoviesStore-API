using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMoviesRepository _moviesRepository;

    public MoviesController(IMoviesRepository moviesRepository)
    {
        _moviesRepository = moviesRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetHello()
    {
        var movies = await _moviesRepository.GetAllAsync();
        ;
        return Ok(movies);
    }
}