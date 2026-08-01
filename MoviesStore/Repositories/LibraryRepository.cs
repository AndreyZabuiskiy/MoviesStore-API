using Npgsql;

public class LibraryRepository : ILibraryRepository
{
    private readonly string _connectionString;

    public LibraryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<bool> AddMovieToLibraryAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        int userId,
        int movieId)
    {
        await using var command = new NpgsqlCommand(@"
            INSERT INTO user_library_movies (user_id, movie_id)
            VALUES (@user_id, @movie_id);
        ", connection, sqlTransaction);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        command.Parameters.AddWithValue("@movie_id", NpgsqlTypes.NpgsqlDbType.Integer, movieId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> CreateUserLibraryAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction sqlTransaction,
        int userId)
    {
        await using var command = new NpgsqlCommand(@"
            INSERT INTO user_library_settings (user_id)
            VALUES (@user_id);
        ", connection, sqlTransaction);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync(int userId, string visibilityType, string sortType)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@$"
            SELECT ulm.movie_id, m.title, m.duration_minutes, m.imdb_rating, m.price, ulm.is_hidden, ulm.added_at
            FROM user_library_movies ulm
            INNER JOIN movies m
            ON ulm.movie_id = m.movie_id
            { GetVisibleTypeQuery(visibilityType) }
            { GetSortTypeQuery(sortType) };
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        await using var reader = await command.ExecuteReaderAsync();

        var movieIdOrdinal = reader.GetOrdinal("movie_id");
        var titleOrdinal = reader.GetOrdinal("title");
        var durationMinutesOrdinal = reader.GetOrdinal("duration_minutes");
        var imdbRatingOrdinal = reader.GetOrdinal("imdb_rating");
        var priceOrdinal = reader.GetOrdinal("price");
        var isHiddenOrdinal = reader.GetOrdinal("is_hidden");
        var addedAtOrdinal = reader.GetOrdinal("added_at");

        var movies = new List<LibraryMovieReadModel>();

        while(await reader.ReadAsync())
        {
            movies.Add(new LibraryMovieReadModel
            {
                MovieId = reader.GetInt32(movieIdOrdinal),
                Title = reader.GetString(titleOrdinal),
                DurationMinutes = reader.GetInt32(durationMinutesOrdinal),
                ImdbRating = reader.GetDouble(imdbRatingOrdinal),
                Price = reader.GetDecimal(priceOrdinal),
                IsHidden = reader.GetBoolean(isHiddenOrdinal),
                AddedAt = reader.GetFieldValue<DateTime>(addedAtOrdinal)
            });
        }

        return movies;
    }

    public async Task<string?> GetSortTypeLibrarySettingsAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT sort_type
            FROM user_library_settings
            WHERE user_id = @user_id
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        var result = await command.ExecuteScalarAsync();
        return result as string;
    }

    public async Task<string?> GetVisibleTypeLibrarySettingsAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT visible_type
            FROM user_library_settings
            WHERE user_id = @user_id;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        var result = await command.ExecuteScalarAsync();
        return result as string;
    }

    public async Task<bool> IsMovieInUserLibrary(int userId, int movieId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT EXISTS
            (
                SELECT 1
                FROM user_library_movies
                WHERE user_id = @user_id 
                    AND movie_id = @movie_id
            );
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        command.Parameters.AddWithValue("@movie_id", NpgsqlTypes.NpgsqlDbType.Integer, movieId);

        return (bool)await command.ExecuteScalarAsync();
    }

    public async Task<bool> SetMovieVisibilityAsync(int userId, int movieId, bool isHidden)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            UPDATE user_library_movies
            SET is_hidden = @is_hidden
            WHERE user_id = @user_id
                AND movie_id = @movie_id
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        command.Parameters.AddWithValue("@movie_id", NpgsqlTypes.NpgsqlDbType.Integer, movieId);
        command.Parameters.AddWithValue("@is_hidden", NpgsqlTypes.NpgsqlDbType.Boolean, isHidden);

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> SetSortTypeLibrarySettingsAsync(int userId, string sortType)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            UPDATE user_library_settings
            SET sort_type = @sort_type
            WHERE user_id = @user_id;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        command.Parameters.AddWithValue("@sort_type", NpgsqlTypes.NpgsqlDbType.Text, sortType);

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> SetVisibleTypeLibrarySettingsAsync(int userId, string visibleType)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            UPDATE user_library_settings
            SET visible_type = @visible_type
            WHERE user_id = @user_id;
        ", connection);

        command.Parameters.AddWithValue("@visible_type", NpgsqlTypes.NpgsqlDbType.Text, visibleType);
        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        var rows = await command.ExecuteNonQueryAsync();

        return rows > 0;
    }

    private string GetSortTypeQuery(string sortType)
    {
        switch (sortType)
        {
            case SortTypeLibraryMovies.TITLE:
                return "ORDER BY m.title";

            case SortTypeLibraryMovies.DURATION:
                return "ORDER BY m.duration_minutes DESC";

            case SortTypeLibraryMovies.ADDED_AT:
                return "ORDER BY ulm.added_at DESC";

            case SortTypeLibraryMovies.PRICE_DESC:
                return "ORDER BY m.price DESC";

            case SortTypeLibraryMovies.PRICE_ASC:
                return "ORDER BY m.price ASC";

            case SortTypeLibraryMovies.IMDB_RATIONG:
                return "ORDER BY m.imdb_rating DESC"; 
            
            default:
                return "ORDER BY m.title";
        }
    }

    private string GetVisibleTypeQuery(string visibleType)
    {
        switch (visibleType)
        {
            case VisibleTypeLibraryMovies.VISIBLE:
                return "WHERE ulm.user_id = @user_id AND ulm.is_hidden = false";

            case VisibleTypeLibraryMovies.HIDDEN:
                return "WHERE ulm.user_id = @user_id AND ulm.is_hidden = true";

            case VisibleTypeLibraryMovies.ALL:
                return "WHERE ulm.user_id = @user_id";
            
            default:
                return "WHERE ulm.user_id = @user_id AND ulm.is_hidden = false";
        }
    }
}
