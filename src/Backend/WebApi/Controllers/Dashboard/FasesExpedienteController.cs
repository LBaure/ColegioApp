using Core.Excepciones;
using Core.Models.Dashboard;
using Core.Servicios.Dashboard;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Dashboard
{

    [Authorize(policy: "Jwt")]
    [Produces("application/json")]
    [Route("api/dashboard/[controller]")]
    public class FasesExpedienteController : Controller
    {
        private readonly ITipoExpedienteServicio _tipoExpedienteServicio;

        public FasesExpedienteController(ITipoExpedienteServicio tipoExpedienteServicio)
        {
            _tipoExpedienteServicio = tipoExpedienteServicio;
        }


        [Authorize(Policy = "Jwt")]
        [HttpGet]
        public async Task<IActionResult> ObtenerTipoExpedientes()
        {
            try
            {
                var lista = await _tipoExpedienteServicio.ObtenerTiposExpedienteUsuario();
                return Ok(lista);
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

        [HttpPost("ObtenerExpedientesWorkFlow")]
        public async Task<IActionResult> ObtenerExpedientesWorkFlow([FromBody] ExpedientesPorTipoModelo filtrosModelo)
        {
            try
            {
                var lista = await _tipoExpedienteServicio.ObtenerExpedientesWorkFlow(filtrosModelo);
                return Ok(lista);
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
