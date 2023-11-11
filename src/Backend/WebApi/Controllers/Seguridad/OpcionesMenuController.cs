using Core.Excepciones;
using Core.Models.Seguridad;
using Core.Servicios.Seguridad;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Seguridad
{
    [Authorize(policy: "Jwt")]
    [Produces("application/json")]
    [Route("api/seguridad/[controller]")]
    public class OpcionesMenuController : Controller
    {
        private readonly IOpcionMenuServicio _opcionMenuServicio;
        public OpcionesMenuController(IOpcionMenuServicio opcionMenuServicio)
        {
            _opcionMenuServicio = opcionMenuServicio;
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerOpcionesMenu()
        {
            try
            {
                var listaRoles = await _opcionMenuServicio.ObtenerOpcionesMenu();
                return Ok(listaRoles);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertarOpcionMenu([FromBody] OpcionMenuModelo reqOpcionMenu)
        {
            try
            {
                var response = await _opcionMenuServicio.InsertarOpcionMenu(reqOpcionMenu);
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

        [HttpPut]
        public async Task<IActionResult> ActualizarOpcionMenu([FromBody] OpcionMenuModelo reqOpcionMenu)
        {
            try
            {
                var response = await _opcionMenuServicio.ActualizarOpcionMenu(reqOpcionMenu);
                return Ok(response);

            }
            catch(ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }


        [HttpDelete("{idOpcionMenu}")]
        public async Task<IActionResult> EliminarOpcionMenu([FromRoute] int idOpcionMenu)
        {
            try
            {
                var listaRoles = await _opcionMenuServicio.EliminarOpcionMenu(idOpcionMenu);
                return Ok(listaRoles);

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
