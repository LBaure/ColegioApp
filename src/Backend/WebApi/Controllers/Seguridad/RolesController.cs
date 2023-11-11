using Core.Excepciones;
using Core.Models.Seguridad;
using Core.Servicios.Seguridad;
using Exceptionless;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Seguridad
{
    //[Authorize(policy: "Jwt")]
    [Produces("application/json")]
    [Route("api/seguridad/[controller]")]
    public class RolesController : Controller
    {
        private readonly IRolServicio _rolServicio;
        public RolesController(IRolServicio rolServicio)
        {
            _rolServicio = rolServicio;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRoles()
        {
            try
            {
                var listaRoles = await _rolServicio.ObtenerRoles();
                return Ok(listaRoles);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertarRol([FromBody] RolModelo reqRol)
        {
            try
            {
                var response = await _rolServicio.InsertarRol(reqRol);
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
        public async Task<IActionResult> ModificarRol([FromBody] RolModelo reqRol)
        {
            try
            {
                var response = await _rolServicio.ModificarRol(reqRol);
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
        [HttpDelete("{idRol}")]
        public async Task<IActionResult> EliminarRol([FromRoute] int idRol)
        {
            try
            {
                var response = await _rolServicio.EliminarRol(idRol);
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

        [HttpGet("opciones-menu/{idRol}")]
        public async Task<IActionResult> ObtenerOpcionesMenuPorRol([FromRoute] int idRol)
        {
            try
            {
                var listaRoles = await _rolServicio.ObtenerOpcionesMenuPorRol(idRol);
                return Ok(listaRoles);
            }
            catch (Exception ex)
            {
                ex.ToExceptionless();
                return StatusCode(500);
            }
        }

        [HttpPost("opciones-menu")]
        public async Task<IActionResult> InsertarOpcionesMenuPorRol([FromBody] RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            try
            {
                var listaRoles = await _rolServicio.InsertarOpcionesMenuPorRol(rolOpcionMenuModelo);
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

        [HttpPut("opciones-menu")]
        public async Task<IActionResult> ModificarOpcionesMenuPorRol([FromBody] RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            try
            {
                var listaRoles = await _rolServicio.ModificarOpcionesMenuPorRol(rolOpcionMenuModelo);
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


        [HttpDelete("opciones-menu")]
        public async Task<IActionResult> EliminarOpcionMenuPorRol([FromBody] RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            try
            {
                var response = await _rolServicio.EliminarOpcionMenuPorRol(rolOpcionMenuModelo);
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
