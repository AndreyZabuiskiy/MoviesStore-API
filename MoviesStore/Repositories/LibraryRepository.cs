using Npgsql;

public class LibraryRepository : ILibraryRepository
{
    private readonly string _connectionString;
    Dictionary<string, string> visibilityTypesDictionary = new Dictionary<string, string>()
    {
        { VisibleTypeLibraryMovie.VISIBLE, "WHERE ulm.user_id = @user_id AND ulm.is_hidden = false" },
        { VisibleTypeLibraryMovie.HIDDEN, "WHERE ulm.user_id = @user_id AND ulm.is_hidden = true" },
        { VisibleTypeLibraryMovie.ALL, "WHERE ulm.user_id = @user_id"}
    };
    Dictionary<string, string> sortTypesDictionary = new Dictionary<string, string>()
    {
        { SortTypeLibraryMovie.TITLE, "ORDER BY m.title" },
        { SortTypeLibraryMovie.DURATION, "ORDER BY m.duration_minutes DESC" },
        { SortTypeLibraryMovie.ADDED_AT, "ORDER BY ulm.added_at DESC"},
        { SortTypeLibraryMovie.PRICE_DESC, "ORDER BY m.price DESC"},
        { SortTypeLibraryMovie.PRICE_ASC, "ORDER BY m.price ASC"},
        { SortTypeLibraryMovie.IMDB_RATIONG, "ORDER BY m.imdb_rating DESC"}
    };

    public LibraryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<List<LibraryMovieReadModel>> GetLibraryMoviesAsync(int userId, string visibility, string sort)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@$"
            SELECT ulm.movie_id, m.title, m.duration_minutes, m.imdb_rating, m.price, ulm.is_hidden, ulm.added_at
            FROM user_library_movies ulm
            INNER JOIN movies m
            ON ulm.movie_id = m.movie_id
            {
                (visibilityTypesDictionary.TryGetValue(visibility, out string visibilityQuery)
                ? visibilityQuery
                : "WHERE ulm.user_id = @user_id")
            }
            {
                (sortTypesDictionary.TryGetValue(sort, out string sortQuery)
                ? sortQuery
                : "ORDER BY m.title")
            };
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

    public async Task<string> GetSortTypeLibrarySettingsAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT sort_type
            FROM user_library_settings
            WHERE user_id = @user_id
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        var result = (string)await command.ExecuteScalarAsync();

        return result;
    }

    public async Task<string> GetVisibleLibraryMovieAsync(int userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT visible_type
            FROM user_library_settings
            WHERE user_id = @user_id;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);

        var result = (string)await command.ExecuteScalarAsync();
        return result;
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

    public async Task<bool> SetSortLibrarySettingsAsync(int userId, string sortType)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            UPDATE user_library_settings
            SET sort_type = @sort_type
            WHERE user_id = @user_id
            RETURNING sort_type;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, userId);
        command.Parameters.AddWithValue("@sort_type", NpgsqlTypes.NpgsqlDbType.Text, sortType);

        var rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> SetVisibleLibraryMovieAsync(int userId, string visibleType)
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
}
