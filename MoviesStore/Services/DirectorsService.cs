public class DirectorsService : IDirectorsService
{
    private readonly IDirectorsRepository _directorsRepository;

    public DirectorsService(IDirectorsRepository directorsRepository)
    {
        _directorsRepository = directorsRepository;
    }

    public async Task<List<DirectorCardDto>> GetDirectors()
    {
        var directors = await _directorsRepository.GetDirectors();

        var directorsCardDto = new List<DirectorCardDto>();

        foreach(var director in directors)
        {
            directorsCardDto.Add(new DirectorCardDto
            {
                DirectorId = director.DirectorId,
                FirstName = director.FirstName,
                LastName = director.LastName
            });
        }

        return directorsCardDto;
    }
}
