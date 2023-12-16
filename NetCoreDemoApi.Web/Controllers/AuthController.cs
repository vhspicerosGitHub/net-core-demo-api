using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NetCoreDemoApi.Common;
using NetCoreDemoApi.Services;
using NetCoreDemoApi.Web.ViewModel;
using SQLitePCL;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace NetCoreDemoApi.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private readonly AuthService _authService;
        public AuthController(AuthService authService, IConfiguration config, ILogger<AuthController> logger)
        {
            _authService = authService;
            _config = config;
            _logger = logger;
        }


        [HttpPost]
        public IActionResult Post([FromBody] LoginRequest request)
        {
            try
            {
                var user = _authService.Login(email: request.Email, password: request.Password); // Throw a exception if not have access
                if (user == null)
                    throw new BusinessException("Password incorrect");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  null,
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                return Ok(token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw e;
            }
        }
    }

}
