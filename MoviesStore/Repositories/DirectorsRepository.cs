using Npgsql;

public class DirectorsRepository : IDirectorsRepository
{
    private readonly string _connectionString;

    public DirectorsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres");
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
