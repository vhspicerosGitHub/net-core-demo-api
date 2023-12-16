using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreDemoApi.Common;
using NetCoreDemoApi.Model;
using NetCoreDemoApi.Services;
using NetCoreDemoApi.Web.ViewModel;
using System.Security.Claims;

namespace NetCoreDemoApi.Web.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
    private readonly ILogger<ClientController> _logger;
    private readonly IClientService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientController(ILogger<ClientController> logger, IClientService service, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet()]
    [ProducesResponseType(typeof(IEnumerable<Client>), 200)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var test = User.Identity?.Name;
            return Ok(await _service.GetAll());
        }
        catch (BusinessException e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode((int)e.HttpStatusCode, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await _service.GetById(id));
        }
        catch (BusinessException e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode((int)e.HttpStatusCode, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost()]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> Create(CreateOrUpdateClient ClientRequest)
    {
        try
        {
            var client = new Client() { Name = ClientRequest.Name, Email = ClientRequest.Email };
            return Ok(await _service.Create(client));
        }
        catch (BusinessException e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode((int)e.HttpStatusCode, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateOrUpdateClient ClientRequest)
    {
        try
        {
            var client = new Client() { Id = id, Name = ClientRequest.Name, Email = ClientRequest.Email };
            await _service.Update(client);
            return Ok();
        }
        catch (BusinessException e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode((int)e.HttpStatusCode, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(new Client { Id = id });
            return Ok();
        }
        catch (BusinessException e)
        {
            _logger.LogError(e, e.Message);
            return StatusCode((int)e.HttpStatusCode, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return BadRequest(e.Message);
        }
    }
}