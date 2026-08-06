using Npgsql;

public class DirectorsRepository : IDirectorsRepository
{
    private readonly string _connectionString;

    public DirectorsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
    }

    public async Task<Director> GetDirectorById(int id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT director_id, first_name, last_name, date_of_birth, country
            FROM directors
            WHERE director_id=@user_id;
        ", connection);

        command.Parameters.AddWithValue("@user_id", NpgsqlTypes.NpgsqlDbType.Integer, id);

        await using var reader = await command.ExecuteReaderAsync();

        if(await reader.ReadAsync())
        {
            return new Director
            {
                DirectorId = reader.GetInt32(reader.GetOrdinal("director_id")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                DateOfBirth = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("date_of_birth")),
                Country = reader.GetString(reader.GetOrdinal("country"))
            };
        }

        return null;
    }

    public async Task<List<Director>> GetDirectors()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(@"
            SELECT director_id, first_name, last_name
            FROM directors;
        ", connection);

        await using var reader = await command.ExecuteReaderAsync();
        
        var directorIdOrdinal = reader.GetOrdinal("director_id");
        var directorFirstNameOrdinal = reader.GetOrdinal("first_name");
        var directorLastNameOrdinal = reader.GetOrdinal("last_name");

        var directors = new List<Director>();

        while(await reader.ReadAsync())
        {
            directors.Add(new Director()
            {
                DirectorId = reader.GetInt32(directorIdOrdinal),
                FirstName = reader.GetString(directorLastNameOrdinal),
                LastName = reader.GetString(directorLastNameOrdinal)
            });
        };

        return directors;
    }
}
