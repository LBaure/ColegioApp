using Core.Models;
using Core.Models.Seguridad;
using Core.Models.Sso;
using Core.Servicios;
using Core.Servicios.Seguridad;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : Controller
    {
        public readonly IOpcionMenuServicio _opcionMenuServicio;
        public readonly IConfiguration _configuration;
        public readonly IUsuarioActualServicio _usuarioActual;
        private readonly ConfiguracionSsoModelo _configuracionSsoModelo;    

        public LoginController(IOpcionMenuServicio opcionMenuServicio, IConfiguration configuration, IUsuarioActualServicio usuario, ConfiguracionSsoModelo configuracion)
        {
            _opcionMenuServicio = opcionMenuServicio;
            _configuration = configuration;
            _usuarioActual = usuario;
            _configuracionSsoModelo = configuracion;
        }
         

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var algo = _usuarioActual.Get();    
                Console.WriteLine("Usuario" + algo?.Nombre);
                var listaRoles = await _opcionMenuServicio.Get();
                return Ok(listaRoles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public IActionResult Autenticar(UsuarioModelo usuarioModelo)
        {
            string stringToken = GenerateToken(usuarioModelo);
            return Ok(new { Token = stringToken });

        }

        private string GenerateToken(UsuarioModelo usuarioModelo)
        {
            var urllogin = _configuracionSsoModelo.LoginUrl;
            // obtener datos del appconfig
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, usuarioModelo.Nombre ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, usuarioModelo.Correo ?? ""),
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
