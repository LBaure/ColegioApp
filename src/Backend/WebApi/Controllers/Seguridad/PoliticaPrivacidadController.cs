using Core.Excepciones;
using Core.Models.Seguridad;
using Core.Servicios.Seguridad;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Seguridad
{
    [Produces("application/json")]
    [Route("api/seguridad/[controller]")]
    public class PoliticaPrivacidadController : Controller
    {
        private readonly IPoliticaPrivacidadServicio _politicaPrivacidadServicio;
        public PoliticaPrivacidadController(IPoliticaPrivacidadServicio politicaPrivacidad)
        {
            _politicaPrivacidadServicio = politicaPrivacidad;
        }



        [Authorize(policy: "Cookie")]
        [HttpGet]
        public async Task<IActionResult> ObtenerPoliticaPrivacidad()
        {
            try
            {
                var response = await _politicaPrivacidadServicio.ObtenerPoliticaPrivacidad();
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [Authorize(policy: "Cookie")]
        [HttpGet("ConfiguracionDobleFactor")]
        public async Task<IActionResult> ObtenerConfiguracionMFA()
        {
            try
            {
                var response = await _politicaPrivacidadServicio.ObtenerConfiguracionMFA();
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [Authorize(policy: "Jwt")]
        [HttpGet("ConfiguracionDobleFactorLogueado")]
        public async Task<IActionResult> ObtenerConfiguracionUsuarioMFA()
        {
            try
            {
                var response = await _politicaPrivacidadServicio.ObtenerConfiguracionMFA();
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }


        [Authorize(policy: "Jwt")]
        [HttpDelete("{valor}")]
        public async Task<IActionResult> EliminarMFA(string valor)
        {
            try
            {
                var response = await _politicaPrivacidadServicio.EliminarMFA(valor);
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [Authorize(policy: "Cookie")]
        [HttpPost]
        public async Task<IActionResult> InsertarPoliticaUsuario([FromBody] PoliticaUsuarioModelo politicaUsuarioModelo)
        {
            try
            {
                var response = await _politicaPrivacidadServicio.InsertarPoliticaUsuario(politicaUsuarioModelo);
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [Authorize(policy: "Jwt")]
        [HttpPost("ActualizarPolitica")]
        public async Task<IActionResult> ActualizarPoliticaUsuario([FromBody] CambioCredencialesModelo cambioCredenciales)
        {
            try
            {
                var response = await _politicaPrivacidadServicio.ActualizarPoliticaUsuario(cambioCredenciales);
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }


    }
}
