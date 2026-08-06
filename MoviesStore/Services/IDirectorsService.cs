public interface IDirectorsService
{
    public Task<List<DirectorCardDto>> GetDirectors();
    public Task<DirectorDto> GetDirectorById(int id);
}