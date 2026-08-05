public interface IDirectorsRepository
{
    public Task<List<Director>> GetDirectors();
}
