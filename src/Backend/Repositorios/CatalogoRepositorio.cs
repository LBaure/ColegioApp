using Core.Constantes;
using Core.Models;
using Core.Models.Seguridad;
using Core.Repositorios;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorios
{
    public class CatalogoRepositorio : ICatalogoRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;
        public CatalogoRepositorio(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;            
        }

        public async Task<ResultadoHttpModelo> ObtenerOpcionesMenu()
        {
            using var connection = await _connectionProvider.OpenAsync();
            var catalogo = await connection.QueryAsync<CatalogoOpcionMenuModelo>(sqlOpcionesMenu);
            connection.Close();

            return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
            {
                Mensaje = "La información se ha guardado exitosamente",
                Titulo = "Registro de Roles",
                Resultado = catalogo
            };
        }

        public async Task<ResultadoHttpModelo> ObtenerRoles()
        {
            using var connection = await _connectionProvider.OpenAsync();
            var catalogo = await connection.QueryAsync<CatalogoModelo>(sqlRoles);
            connection.Close();

            return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
            {
                Mensaje = "La información se ha guardado exitosamente",
                Titulo = "Registro de Roles",
                Resultado = catalogo
            };
        }

        private const string sqlOpcionesMenu =
           "SELECT \n" +
           "   ID_OPCION_MENU IdOpcionMenu, \n" +
           "   NOMBRE Nombre, \n" +
           "   DESCRIPCION Descripcion, \n" +
           "   URL Path \n" +
           "FROM AD_OPCIONES_MENU \n" +
           "WHERE ACTIVO = 1 \n" +
           "ORDER BY NOMBRE ASC";

        private const string sqlRoles =
           "SELECT \n" +
           "   ID_ROL Id, \n" +
           "   NOMBRE Nombre, \n" +
           "   DESCRIPCION Descripcion \n" +
           "FROM AD_ROLES \n" +
           "WHERE ACTIVO = 1 \n" +
           "ORDER BY NOMBRE ASC";
    }
}
