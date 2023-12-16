using Microsoft.Extensions.Logging;
using NetCoreDemoApi.Common;
using NetCoreDemoApi.Common.Utils;
using NetCoreDemoApi.Model;
using NetCoreDemoApi.Repositories;
using System.Net;

namespace NetCoreDemoApi.Services;

public class ClientService : IClientService
{
    private readonly ILogger<ClientService> _logger;
    private readonly IClientRepository _repository;
    public ClientService(ILogger<ClientService> logger, IClientRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<int> Create(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Email))
            throw new BusinessException("El Email no puede ser vacio");

        if (string.IsNullOrWhiteSpace(client.Name))
            throw new BusinessException("El Nombre no puede ser vacio");

        if (!client.Email.IsEmailValid())
            throw new BusinessException("El correo es Invalido");

        var c = await _repository.GetByEmail(client.Email);
        if (c != null)
            throw new BusinessException("Ya existe un cliente con ese correo");

      

        return await _repository.Create(client);
    }

    public async Task Delete(Client client)
    {
        var c = await _repository.GetById(client.Id);
        if (c == null)
            throw new BusinessException("El cliente no existe", HttpStatusCode.NotFound);

        await _repository.Delete(client);
    }

    public async Task<IEnumerable<Client>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Client> GetById(int id)
    {
        var c = await _repository.GetById(id);
        if (c == null)
            throw new BusinessException("El cliente no existe", HttpStatusCode.NotFound);

        return c;
    }

    public async Task Update(Client client)
    {
        if (string.IsNullOrWhiteSpace(client.Email))
            throw new BusinessException("El Email no puede ser vacio");

        if (string.IsNullOrWhiteSpace(client.Name))
            throw new BusinessException("El Nombre no puede ser vacio");

        if (!client.Email.IsEmailValid())
            throw new BusinessException("El correo es Invalido");

        var c = await _repository.GetById(client.Id);
        if (c == null)
            throw new BusinessException("El cliente no existe", HttpStatusCode.NotFound);

       

        await _repository.Update(client);
    }
}