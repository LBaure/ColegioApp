using Core.Exceptions;
using Core.Models.Security;
using Core.Services.Security;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Security
{
    [ApiController]
    [Route("TallerApi/Security/[controller]")]
    public class MenuPermissionsController : Controller
    {
        private readonly IMenuPermissionService _permissionService;
        public MenuPermissionsController(IMenuPermissionService menuPermissionService)
        {
            _permissionService = menuPermissionService;            
        }

        [HttpGet]
        public async Task<IActionResult> GetListMenuPermissions()
        {
            try
            {
                var lista = await _permissionService.GetListMenuPermissions();
                return Ok(lista);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertMenuPermissions([FromBody] ListMenuPermissionModel model)
        {
            try
            {
                var response = await _permissionService.InsertMenuPermissions(model);
                return Ok(response);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMenuPermissions([FromBody] ListMenuPermissionModel permissionModel)
        {
            try
            {
                var response = await _permissionService.UpdateMenuPermissions(permissionModel);
                return Ok(response);

            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuPermissions([FromRoute] int id)
        {
            try
            {
                var listaRoles = await _permissionService.DeleteMenuPermissions(id);
                return Ok(listaRoles);

            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
