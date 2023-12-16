using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreDemoApi.Model;


namespace NetCoreDemoApi.Repositories.SqlLite;
public class UserRepository : IUserRepository
{

    private readonly ILogger<UserRepository> _logger;
    private readonly IConfiguration _configuration;

    public UserRepository(ILogger<UserRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private SqliteConnection GetConnection()
    {
        var connString = _configuration.GetConnectionString("DefaultConnection");
        return new SqliteConnection(connString);

    }

    public async Task<User?> GetByEmail(string email)
    {
        _logger.LogDebug($"Executing Query => {UserQueries.GetByEmail}");
        return await GetConnection().QueryFirstOrDefaultAsync<User>(UserQueries.GetByEmail,
             new { email });
    }

    public async Task<User?> GetById(int id)
    {
        _logger.LogDebug($"Executing Query => {UserQueries.GetByEmail}");
        return await GetConnection().QueryFirstOrDefaultAsync<User>(UserQueries.GetById,
             new { id });
    }
}
