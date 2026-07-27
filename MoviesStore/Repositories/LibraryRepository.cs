
using Npgsql;

public class LibraryRepository : ILibraryRepository
{
    private readonly string _connectionString;

    public LibraryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT m.movie_id, m.title, ulm.added_at
            FROM user_library_movies ulm
            INNER JOIN movies m
            ON ulm.movie_id = m.movie_id
            WHERE ulm.user_id = @user_id
            ORDER BY m.title;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        await using var reader = await command.ExecuteReaderAsync();

        var movieIdOrdinal = reader.GetOrdinal("movie_id");
        var titleOrdinal = reader.GetOrdinal("title");
        var addedAtOrdinal = reader.GetOrdinal("added_at");

        var movies = new List<LibraryMovieReadModel>();

        while(await reader.ReadAsync())
        {
            movies.Add(new LibraryMovieReadModel
            {
                MovieId = reader.GetInt32(movieIdOrdinal),
                Title = reader.GetString(titleOrdinal),
                AddedAt = reader.GetFieldValue<DateTime>(addedAtOrdinal)
            });
        }

        return movies;
    }
}
