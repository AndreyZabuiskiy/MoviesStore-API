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

    public async Task<Movie> GetByIdAsync(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand(@"
            SELECT m.movie_id, m.title, m.release_date, m.duration_minutes, m.budget, m.box_office, m.age_rating, m.imdb_rating, m.price,
                d.director_id, d.first_name, d.last_name
            FROM movies m
            INNER JOIN directors d
            ON d.director_id = m.director_id
            WHERE movie_id = @id;
            ", connection);

        command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Integer, id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Movie
            {
                MovieId = reader.GetInt32(reader.GetOrdinal("movie_id")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                ReleaseDate = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("release_date")),
                Budget = reader.GetInt32(reader.GetOrdinal("budget")),
                BoxOffice = reader.GetInt32(reader.GetOrdinal("box_office")),
                AgeRating = reader.GetString(reader.GetOrdinal("age_rating")),
                ImdbRating = reader.GetDouble(reader.GetOrdinal("imdb_rating")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Director = new Director
                {
                    DirectorId = reader.GetInt32(reader.GetOrdinal("director_id")),
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name"))
                }
            };
        }

        return null;
    }
}
