using Core.Exceptions;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("TallerApi/[controller]")]
    public class ClienteController : Controller
    {
        private readonly IClienteServicio _clienteServicio;
        public ClienteController(IClienteServicio clienteServicio)
        {
            _clienteServicio = clienteServicio;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosClientes()
        {
            try
            {
                var resultado = await _clienteServicio.ObtenerTodosClientes();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCliente([FromBody] AgregarClienteModelo clienteModelo)
        {
            try
            {
                var resultado = await _clienteServicio.AgregarCliente(clienteModelo);
                return Ok(resultado);
            }
            catch (ResponseException rex)
            {
                return StatusCode(rex.codigo, new { rex.mensaje, rex.estado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
