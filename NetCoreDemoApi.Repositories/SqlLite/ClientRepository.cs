using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using NetCoreDemoApi.Model;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace NetCoreDemoApi.Repositories.SqlLite
{
    public class ClienteRepository : IClientRepository
    {
        private readonly ILogger<ClienteRepository> _logger;
        private readonly IConfiguration _configuration;

        public ClienteRepository(ILogger<ClienteRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        private SqliteConnection GetConnection()
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            return new SqliteConnection(connString);

        }
        public async Task<int> Create(Client client)
        {
            _logger.LogDebug($"Ejecutando Query => {Queries.Create}");
            return await GetConnection().ExecuteScalarAsync<int>(Queries.Create,
                new { name = client.Name, email = client.Email });
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            _logger.LogDebug($"Ejecutando Query => {Queries.GetAll}");
            return await GetConnection().QueryAsync<Client>(Queries.GetAll);
        }

        public async Task<Client?> GetByEmail(string email)
        {
            return await GetConnection().QueryFirstOrDefaultAsync<Client>(Queries.GetByEmail,
                new { email });
        }

        public async Task<Client?> GetById(int id)
        {
            _logger.LogDebug($"Ejecutando Query => {Queries.GetById}");
            return await GetConnection().QueryFirstOrDefaultAsync<Client>(Queries.GetById, new { id });
        }

        public async Task Delete(Client client)
        {
            _logger.LogDebug($"Ejecutando Query => {Queries.Delete}");
            _ = await GetConnection().ExecuteAsync(Queries.Delete, new { id = client.Id });
        }

        public async Task Update(Client client)
        {
            _logger.LogDebug($"Ejecutando Query => {Queries.Update}");
            _ = await GetConnection().ExecuteAsync(Queries.Update,
                new { id = client.Id, name = client.Name, email = client.Email });
        }
    }
}