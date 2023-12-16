using NetCoreDemoApi.Model;

namespace NetCoreDemoApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
}
