using Core.Excepciones;
using Core.Models.Seguridad;
using Core.Servicios.Seguridad;
using Exceptionless;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Seguridad
{
    [Produces("application/json")]
    [Route("api/seguridad/[controller]")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioServicio _usuarioServicio;

        public UsuariosController(IUsuarioServicio usuarioServicio)
        {
            _usuarioServicio = usuarioServicio;
        }
        [Authorize(policy:"Cookie")]
        [HttpGet("{nitUsuario?}")]
        public async Task<IActionResult> Get([FromRoute] string? nitUsuario)
        {
            try
            {
                var result = await _usuarioServicio.ObtenerUsuarios(nitUsuario);
                return Ok(result);  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(policy: "Cookie")]
        [HttpPost("config-usuario")]
        public async Task<IActionResult> Get([FromBody] DobleFactorAutenticacionModelo dobleFactorAutenticacion)
        {
            try
            {
                var listaUsuarios = await _usuarioServicio.ObtenerConfiguracionUsuario(dobleFactorAutenticacion);
                return Ok(listaUsuarios);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CredencialesUsuarioModelo credencialesUsuario)
        {
            try
            {
                var response = await _usuarioServicio.Login(credencialesUsuario);
                return Ok(response);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] CredencialesUsuarioModelo credencialesUsuario)
        {
            try
            {
                var response = await _usuarioServicio.Logout(credencialesUsuario);
                HttpContext.SignOutAsync().Wait();
                return Ok(response);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertarUsuario([FromBody] UsuarioAdministracionModelo usuarioAdministracion)
        {
            try
            {
                var resultado = await _usuarioServicio.InsertarUsuario(usuarioAdministracion);
                return Ok(resultado);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }
        [Authorize(policy: "Jwt")]
        [HttpPut]
        public async Task<IActionResult> ModificarUsuario([FromBody] UsuarioAdministracionModelo usuarioAdministracion)
        {
            try
            {
                var resultado = await _usuarioServicio.ModificarUsuario(usuarioAdministracion);
                return Ok(resultado);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarUsuario([FromBody] UsuarioAdministracionModelo usuarioAdministracionModelo)
        {
            try
            {
                var resultado = await _usuarioServicio.EliminarUsuario(usuarioAdministracionModelo);
                return Ok(resultado);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpGet("Roles/{NitUsuario}")]
        public async Task<IActionResult> ObtenerRolesUsuario([FromRoute] string NitUsuario)
        {
            try
            {
                var result = await _usuarioServicio.ObtenerRolesUsuario(NitUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(policy: "Jwt")]
        [HttpPost("Roles")]
        public async Task<IActionResult> InsertarRolesUsuario([FromBody] UsuarioRolesModelo usuarioRolesModelo)
        {
            try
            {
                var resultado = await _usuarioServicio.InsertarRolesUsuario(usuarioRolesModelo);
                return Ok(resultado);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpGet("BitacoraRoles/{NitUsuario}")]
        public async Task<IActionResult> ObtenerBitacoraRolesUsuario([FromRoute] string NitUsuario)
        {
            try
            {
                var result = await _usuarioServicio.ObtenerBitacoraRolesUsuario(NitUsuario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("MiPerfil")]
        public async Task<IActionResult> ActualizarMiPerfil([FromBody] MiUsuarioModelo miUsuario)
        {
            try
            {
                var resultado = await _usuarioServicio.ActualizarMiPerfil(miUsuario);
                return Ok(resultado);
            }
            catch (ResponseException re)
            {
                return StatusCode(re.codigo, new { re.mensaje, re.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

    }


}
