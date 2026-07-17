using Npgsql;

public class MoviesRepository : IMoviesRepository
{
    private readonly string _connectionString;

    public MoviesRepository (IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<List<Movie>> GetAllAsync()
    {
        var movies = new List<Movie>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT movie_id, title, release_date, duration_minutes, price
            FROM movies
        ", connection);

        await using var reader = await command.ExecuteReaderAsync();

        var movieIdOrdinal = reader.GetOrdinal("movie_id");
        var titleOrdinal = reader.GetOrdinal("title");
        var releaseDateOrdinal = reader.GetOrdinal("release_date");
        var durationMinutesOrdinal = reader.GetOrdinal("duration_minutes");
        var priceOrdinal = reader.GetOrdinal("price");

        while (await reader.ReadAsync())
        {
            movies.Add(new Movie()
            {
                MovieId = reader.GetInt32(movieIdOrdinal),
                Title = reader.GetString(titleOrdinal),
                ReleaseDate = reader.GetFieldValue<DateOnly>(releaseDateOrdinal),
                DurationMinutes = reader.GetInt32(durationMinutesOrdinal),
                Price = reader.GetDecimal(priceOrdinal)
            });
        }

        return movies;
    }
}
