using Core.Models;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ClienteServicio : IClienteServicio
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        public ClienteServicio(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<ResultadoHttpModelo> AgregarCliente(AgregarClienteModelo clienteModelo)
        {
            return await _clienteRepositorio.AgregarCliente(clienteModelo);            
        }

        public async Task<ResultadoHttpModelo> ObtenerTodosClientes()
        {
            return await _clienteRepositorio.ObtenerTodosClientes();
        }

    }
}
