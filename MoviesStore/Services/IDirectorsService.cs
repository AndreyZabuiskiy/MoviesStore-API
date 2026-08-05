public interface IDirectorsService
{
    public Task<List<DirectorCardDto>> GetDirectors();
}