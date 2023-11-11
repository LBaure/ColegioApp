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
    public class DashboardController : Controller
    {
        private readonly IDashboardServicio _dashboardServicio;

        public DashboardController(IDashboardServicio dashboardServicio)
        {
            _dashboardServicio = dashboardServicio;
        }

        
        [HttpPost("ObtenerDatosDashboard")]
        public async Task<IActionResult> ObtenerDatosDashboard([FromBody] ExpedientesPorTipoModelo Filtros)
        {
            try
            {
                var lista = await _dashboardServicio.ObtenerDatosDashboard(Filtros);
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

        [HttpPost("ExpedientesPorTipo")]
        public async Task<IActionResult> ObtenerExpedientesPorTipo([FromBody] ExpedientesPorTipoModelo Filtros)
        {
            try
            {
                var lista = await _dashboardServicio.ObtenerExpedientesPorTipo(Filtros);
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

        [HttpPost("EncabezadoExpediente")]
        public async Task<IActionResult> ObtenerEncabezadoExpediente([FromBody] FiltrosEncabezadoModelo filtrosEncabezado)
        {
            try
            {
                var lista = await _dashboardServicio.ObtenerEncabezadoExpediente(filtrosEncabezado);
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

        [HttpGet("ObtenerFasesPorTipoExpediente/{IdTipoExpediente}")]
        public async Task<IActionResult> ObtenerFasesPorTipoExpediente([FromRoute] string IdTipoExpediente)
        {
            try
            {
                var lista = await _dashboardServicio.ObtenerFasesPorTipoExpediente(IdTipoExpediente);
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
