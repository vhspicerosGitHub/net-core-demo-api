using NetCoreDemoApi.Model;

namespace NetCoreDemoApi.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAll();

        Task<Client?> GetById(int id);

        Task<Client?> GetByEmail(string email);

        Task<int> Create(Client client);

        Task Update(Client client);

        Task Delete(Client client);
    }
}