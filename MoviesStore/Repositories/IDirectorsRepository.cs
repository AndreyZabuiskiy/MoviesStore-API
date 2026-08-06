public interface IDirectorsRepository
{
    public Task<List<Director>> GetDirectors();
    public Task<Director> GetDirectorById(int id);
}
