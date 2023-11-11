using Core.Servicios;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CatalogosController : Controller
    {
        private readonly ICatalogoServicio _catalogoServicio;
        public CatalogosController(ICatalogoServicio catalogoServicio)
        {
            _catalogoServicio = catalogoServicio;
        }

        [HttpGet]
        public IActionResult Get() 
        {
            return Ok("Ok connected...");
        }

        [HttpGet("OpcionesMenu")]
        public async Task<IActionResult> ObtenerOpcionesMenu()
        {
            try
            {
                var resultado = await _catalogoServicio.ObtenerOpcionesMenu();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                Console.Write("------------------ALERT-ERROR------------------" + ex);
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [Authorize(policy: "Cookie")]
        [HttpGet("Roles")]
        public async Task<IActionResult> ObtenerRoles()
        {
            try
            {
                var resultado = await _catalogoServicio.ObtenerRoles();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }
    }
}
