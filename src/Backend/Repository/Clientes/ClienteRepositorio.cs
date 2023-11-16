using Core.Constants;
using Core.Exceptions;
using Core.Models;
using Core.Repositories;
using Dapper;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Clientes
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;
        public ClienteRepositorio(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<ResultadoHttpModelo> AgregarCliente(AgregarClienteModelo clienteModelo)
        {
            using var connection = await _connectionProvider.OpenAsync();

            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@ParamNitCliente", clienteModelo.Cliente?.NitCliente);
                parametros.Add("@ParamNombres", clienteModelo.Cliente?.Nombres);
                parametros.Add("@ParamApellidos", clienteModelo.Cliente?.Apellidos);
                parametros.Add("@ParamDireccion", clienteModelo.Cliente?.Direccion);
                parametros.Add("@ParamTelefono", clienteModelo.Cliente?.Telefono);
                parametros.Add("@ParamCorreoElectronico", clienteModelo.Cliente?.CorreoElectronico);
                parametros.Add("@ParamEstado", clienteModelo.Cliente?.Estado == true ? 1 : 0 );   


                var sqlInsertClient =
                "INSERT INTO clientes \n" +
                "( \n" +
                "    NitCliente, \n" +
                "    Nombres, \n" +
                "    Apellidos, \n" +
                "    Direccion, \n" +
                "    Telefono, \n" +
                "    CorreoElectronico, \n" +
                "    Estado \n" +
                ") \n" +
                "VALUES \n" +
                "( \n" +
                "    @ParamNitCliente, \n" +
                "    @ParamNombres, \n" +
                "    @ParamApellidos, \n" +
                "    @ParamDireccion, \n" +
                "    @ParamTelefono, \n" +
                "    @ParamCorreoElectronico, \n" +
                "    @ParamEstado \n" +
                ") ";


                if (clienteModelo.Vehiculo != null)
                {
                    Console.WriteLine("hay que insertar vehiculo");
                }

                var filasAfectadas = await connection.ExecuteAsync(sqlInsertClient, parametros);

                if (filasAfectadas > 0)
                {
                    var listadoClientes = await ObtenerTodosClientes();
                    return new ResultadoHttpModelo(
                       EstadoSolicitudHttp.success,
                       "Cliente registrado correctamente",
                       "Clientes",
                       resultado: listadoClientes.Resultado
                   );
                }
                else
                {
                    throw new ResponseException("Ocurrio un error al insertar el cliente, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                }


               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }




        public async Task<ResultadoHttpModelo> ObtenerTodosClientes()
        {
            using var connection = await _connectionProvider.OpenAsync();
            try
            {
                var sqlSelectClient = "select * from clientes";

                var cliente = await connection.QueryAsync<ClienteModelo>(sqlSelectClient);
                return new ResultadoHttpModelo(
                    EstadoSolicitudHttp.success,
                    "Información obtenidad exitosamente.",
                    "Clientes",
                    cliente
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}
