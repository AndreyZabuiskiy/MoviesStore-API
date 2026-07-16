using Npgsql;

public class MoviesRepository : IMoviesRepository
{
    private readonly string _connectionString;

    public MoviesRepository (IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        var movies = new List<Movie>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT movie_id, title
            FROM movies
        ", connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            movies.Add(new Movie()
            {
                MovieId = reader.GetInt32(reader.GetOrdinal("movie_id")),
                Title = reader.GetString(reader.GetOrdinal("title"))
            });
        }

        return movies;
    }
}
