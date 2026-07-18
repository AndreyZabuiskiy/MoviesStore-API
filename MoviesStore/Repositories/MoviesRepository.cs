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
            SELECT m.movie_id, m.title, m.release_date, m.duration_minutes, m.price,
                d.director_id, d.first_name, d.last_name
            FROM movies m
            INNER JOIN directors d
            ON m.director_id = d.director_id
        ", connection);

        await using var reader = await command.ExecuteReaderAsync();

        var movieIdOrdinal = reader.GetOrdinal("movie_id");
        var titleOrdinal = reader.GetOrdinal("title");
        var releaseDateOrdinal = reader.GetOrdinal("release_date");
        var durationMinutesOrdinal = reader.GetOrdinal("duration_minutes");
        var priceOrdinal = reader.GetOrdinal("price");
        var directorIdOrdinal = reader.GetOrdinal("director_id");
        var firstNameOrdinal = reader.GetOrdinal("first_name");
        var lastNameOrdinal = reader.GetOrdinal("last_name");

        while (await reader.ReadAsync())
        {
            movies.Add(new Movie()
            {
                MovieId = reader.GetInt32(movieIdOrdinal),
                Title = reader.GetString(titleOrdinal),
                ReleaseDate = reader.GetFieldValue<DateOnly>(releaseDateOrdinal),
                DurationMinutes = reader.GetInt32(durationMinutesOrdinal),
                Price = reader.GetDecimal(priceOrdinal),
                Director = new Director
                {
                    DirectorId = reader.GetInt32(directorIdOrdinal),
                    FirstName = reader.GetString(firstNameOrdinal),
                    LastName = reader.GetString(lastNameOrdinal)
                }
            });
        }

        return movies;
    }
}
