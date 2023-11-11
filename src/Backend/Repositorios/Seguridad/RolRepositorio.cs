using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Seguridad;
using Core.Repositorios;
using Core.Repositorios.Seguridad;
using Dapper;
using Exceptionless;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorios.Seguridad
{
    public class RolRepositorio : IRolRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;
        public RolRepositorio(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;   
            
        }
        public async Task<IEnumerable<RolPorUsuarioModelo>> ObtenerRolesPorUsuarioAsync(string nitUsuario)
        {
            using var connection = await _connectionProvider.OpenAsync();
            return await connection.QueryAsync<RolPorUsuarioModelo>(ObtenerRolesPorUsuarioSql, new { NitUsuario = nitUsuario });
        }

        public async Task<ResultadoHttpModelo> ObtenerRoles()
        {
            using var connection = await _connectionProvider.OpenAsync();
            var lista = await connection.QueryAsync<RolModelo>(sqlCmdObtenerRoles);

           return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
            {
                Mensaje = "Se ha descargado lista de resultados exitosamente.",
                Titulo = "Roles",
                Resultado = lista.AsList()
            };
        }

        public async Task<ResultadoHttpModelo> InsertarRol(RolModelo reqRol)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var param = new DynamicParameters();
                param.Add("@IdRol", reqRol.IdRol);
                param.Add("@Nombre", reqRol.Nombre);
                param.Add("@Descripcion", reqRol.Descripcion);
                param.Add("@Activo", reqRol.Activo ? 1 : 0);

                bool existe = await ExisteRol(reqRol, true, connection, transaction);
                if (existe)
                {
                    transaction.Rollback();
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Existe un registro con el mismo nombre, verifique y vuelva a intentarlo.",
                        Titulo = "Registro de Roles"
                    };
                }
                var filasAfectadas = await connection.ExecuteAsync(sqlCmdInsertarRol, param);
                // agregar bitacora

                transaction.Commit();
                var listadoRoles = await this.ObtenerRoles();

                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha registrado exitosamente",
                    Titulo = "Registro de Roles",
                    Resultado = listadoRoles.Resultado
                };

            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                throw new ResponseException("Ha ocurrido un error al eliminar el registro, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private async Task<bool> ExisteRol(RolModelo reqRol, bool esNuevo, IDbConnection _conn, IDbTransaction _trx)
        {
            string sqlSelectExiste = "" +
                "SELECT COUNT(1) \n" +
                "FROM AD_ROLES \n" +
                "WHERE UPPER(NOMBRE) = UPPER(:Nombre) ";

            var param = new DynamicParameters();
            param.Add("@Nombre", reqRol.Nombre);

            if (!esNuevo)
            {
                param.Add("@IdRol", reqRol.IdRol);
                sqlSelectExiste += "\n AND ID_ROL != :IdRol ";
            }

            int existe = await _conn.QueryFirstAsync<int>(sqlSelectExiste, param);
            return existe > 0;
        }

        public async Task<ResultadoHttpModelo> ModificarRol(RolModelo reqRol)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var param = new DynamicParameters();
                param.Add("@IdRol", reqRol.IdRol);
                param.Add("@Nombre", reqRol.Nombre);
                param.Add("@Descripcion", reqRol.Descripcion);
                param.Add("@Activo", reqRol.Activo ? 1 : 0);

                bool existe = await ExisteRol(reqRol, false, connection, transaction);
                if (existe)
                {
                    transaction.Rollback();
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Existe un registro con el mismo nombre, verifique y vuelva a intentarlo.",
                        Titulo = "Registro de Roles"
                    };
                }
                var filasAfectadas = await connection.ExecuteAsync(sqlCmdModificarRol, param);
                // agregar bitacora

                transaction.Commit();
                var listadoRoles = await this.ObtenerRoles();

                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha actualizado exitosamente",
                    Titulo = "Actualización de Roles",
                    Resultado = listadoRoles.Resultado
                };
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                throw;
            }
        }

        public async Task<ResultadoHttpModelo> EliminarRol(int idRol)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var param = new DynamicParameters();
                param.Add("@IdRol", idRol);

                var filasAfectadas = await connection.ExecuteAsync(sqlCmdEliminarRol, param);
                // agregar bitacora

                transaction.Commit();
                var listadoRoles = await this.ObtenerRoles();

                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "Registro eliminado correctamente",
                    Titulo = "Roles",
                    Resultado = listadoRoles.Resultado
                };
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ORA-02292"))
                {
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = $"No se ha podido eliminar el rol debido a que esta siendo usado en otras instancias del sistema.",
                        Titulo = "Roles"
                    };
                }
                else
                {
                    throw new ResponseException($"Ha ocurrido un error al eliminar el rol, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                }
            }
        }

        public async Task<ResultadoHttpModelo> ObtenerOpcionesMenuPorRol(int idRol)
        {
            var param = new DynamicParameters();
            param.Add("@IdRol", idRol);

            using var connection = await _connectionProvider.OpenAsync();
            var lista = await connection.QueryAsync<RolOpcionMenuModelo>(sqlCmdObtenerOpcionMenuRol, param);

            return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
            {
                Mensaje = "Se ha descargado lista de resultados exitosamente.",
                Titulo = "Roles",
                Resultado = lista.AsList()
            };
        }

        public async Task<ResultadoHttpModelo> InsertarOpcionesMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo)
        {

            using var connection = await _connectionProvider.OpenAsync();

            var param = new DynamicParameters();
            param.Add("@PIdRol", rolOpcionMenuModelo.IdRol);
            param.Add("@PIdOpcionMenu", rolOpcionMenuModelo.IdOpcionMenu);
            param.Add("@PConsulta", rolOpcionMenuModelo.Consulta ? 1 : 0);
            param.Add("@PInserta", rolOpcionMenuModelo.Inserta ? 1 : 0);
            param.Add("@PModifica", rolOpcionMenuModelo.Modifica ? 1 : 0);
            param.Add("@PElimina", rolOpcionMenuModelo.Elimina ? 1 : 0);

            try
            {
                string sqlExiste = "SELECT COUNT(1) FROM AD_ROLES_OPCIONES_MENU WHERE ID_ROL  = :PIdRol AND ID_OPCION_MENU = :PIdOpcionMenu";
                var count = await connection.QueryFirstAsync<int>(sqlExiste, param);

                if (count > 0)
                {
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Existe un registro con la misma opción de menú, verifique y vuelva a intentarlo.",
                        Titulo = "Opciones de menú por roles"
                    };
                }
                var filasAfectadas = await connection.ExecuteAsync(sqlCmdInsertarOpcionMenuRol, param);
                // bitacora
                var lista = await this.ObtenerOpcionesMenuPorRol(rolOpcionMenuModelo.IdRol);

                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha ingresado exitosamente",
                    Titulo = "Opciones de menú por roles",
                    Resultado = lista.Resultado
                };
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                throw new ResponseException("Ha ocurrido un error al ingresar la opción del menú", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<ResultadoHttpModelo> ModificarOpcionesMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            using var connection = await _connectionProvider.OpenAsync();
            try
            {

                var param = new DynamicParameters();
                param.Add("@PIdRol", rolOpcionMenuModelo.IdRol);
                param.Add("@PIdOpcionMenu", rolOpcionMenuModelo.IdOpcionMenu);
                param.Add("@PConsulta", rolOpcionMenuModelo.Consulta ? 1 : 0);
                param.Add("@PInserta", rolOpcionMenuModelo.Inserta ? 1 : 0);
                param.Add("@PModifica", rolOpcionMenuModelo.Modifica ? 1 : 0);
                param.Add("@PElimina", rolOpcionMenuModelo.Elimina ? 1 : 0);

                var filasAfectadas = await connection.ExecuteAsync(sqlCmdModificarOpcionMenuRol, param);
                var lista = await this.ObtenerOpcionesMenuPorRol(rolOpcionMenuModelo.IdRol);

                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha actualizado exitosamente",
                    Titulo = "Opciones de menú por roles",
                    Resultado = lista.Resultado
                };
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                throw new ResponseException("Ha ocurrido un error al actualizar la opción del menú", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }


        }

        public async Task<ResultadoHttpModelo> EliminarOpcionMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            var param = new DynamicParameters();
            param.Add("@IdRol", rolOpcionMenuModelo.IdRol);
            param.Add("@IdOpcionMenu", rolOpcionMenuModelo.IdOpcionMenu);

            string sqlDelete = @"DELETE FROM AD_ROLES_OPCIONES_MENU WHERE ID_ROL = :IdRol AND ID_OPCION_MENU = :IdOpcionMenu";
            using var conn = await _connectionProvider.OpenAsync();
            await conn.ExecuteAsync(sqlDelete, param);
            conn.Close();
            var lista = await this.ObtenerOpcionesMenuPorRol(rolOpcionMenuModelo.IdRol);
            return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
            {
                Mensaje = "El registro se ha eliminado exitosamente",
                Titulo = "Opciones de menú por roles",
                Resultado = lista.Resultado
            };
        }

        private const string sqlCmdObtenerOpcionMenuRol =
            "SELECT \n" +
            "   arom.ID_ROL idRol, \n" +
            "   arom.ID_OPCION_MENU idOpcionMenu, \n " +
            "   ar.NOMBRE Rol, \n" +
            "   aom.NOMBRE OpcionMenu, \n " +
            "   aom.DESCRIPCION DescripcionOpcion, \n" +
            "   arom.CONSULTA, \n" +
            "   arom.INSERTA, \n" +
            "   arom.MODIFICA,\n" +
            "   arom.ELIMINA \n" +
            "FROM AD_ROLES_OPCIONES_MENU arom \n" +
            "INNER JOIN AD_ROLES ar \n" +
            "ON ar.ID_ROL = arom.ID_ROL \n" +
            "INNER JOIN AD_OPCIONES_MENU aom \n" +
            "ON aom.ID_OPCION_MENU = arom.ID_OPCION_MENU \n" +
            "WHERE arom.ID_ROL = :IdRol ";

        private const string sqlCmdObtenerRoles =
            "SELECT \n" +
            "   ar.ID_ROL IdRol, \n" +
            "   ar.NOMBRE Nombre, \n" +
            "   ar.DESCRIPCION Descripcion, \n" +
            "   ar.ACTIVO Activo, \n " +
            "   (SELECT count(1) FROM AD_ROLES_OPCIONES_MENU arom WHERE ID_ROL = ar.ID_ROL) ContadorOpcionesMenu \n" +
            "FROM AD_ROLES ar \n " +
            "ORDER BY IdRol ";

        private const string sqlCmdEliminarRol =
            "DELETE FROM AD_ROLES \n" +
            "WHERE ID_ROL = :IdRol";

        private const string ObtenerRolesPorUsuarioSql =
            "SELECT " +
            "   r.ID_ROL IdRol, \n" +
            "   r.NOMBRE \n" +
            "FROM AD_ROLES r \n" +
            "JOIN AD_ROLES_USUARIOS ru ON(r.ID_ROL=ru.ID_ROL) \n " +
            "WHERE ru.NIT_USUARIO = :NitUsuario ";

        private const string sqlCmdInsertarRol =
            "INSERT INTO AD_ROLES(  \n" +
            "    ID_ROL, \n" +
            "    NOMBRE, \n" +
            "    DESCRIPCION, \n" +
            "    ACTIVO \n" +
            ") \n" +
            "VALUES \n" +
            "( \n" +
            "    (SELECT NVL(MAX(ID_ROL),0)+1 FROM AD_ROLES), \n" +
            "    :Nombre, \n" +
            "    :Descripcion, \n" +
            "    :Activo \n" +
            ")";

        private const string sqlCmdModificarRol =
            "UPDATE AD_ROLES  \n" +
            "SET \n" +
            "    NOMBRE = :Nombre, \n" +
            "    DESCRIPCION = :Descripcion, \n" +
            "    ACTIVO = :Activo \n" +
        "WHERE ID_ROL = :IdRol ";

        private const string sqlCmdInsertarOpcionMenuRol = @"
            INSERT INTO AD_ROLES_OPCIONES_MENU
            (
	            ID_ROL,
	            ID_OPCION_MENU,
	            CONSULTA,
	            INSERTA,
	            MODIFICA,
	            ELIMINA
            )
            VALUES(
	            :PIdRol,
	            :PIdOpcionMenu,
	            :PConsulta,
	            :PInserta,
	            :PModifica,
	            :PElimina
            )";

        string sqlCmdModificarOpcionMenuRol =
            "UPDATE AD_ROLES_OPCIONES_MENU \n" +
            "SET \n" +
            "   CONSULTA = :PConsulta, \n" +
            "   INSERTA = :PInserta, \n" +
            "   MODIFICA = :PModifica, \n" +
            "   ELIMINA = :PElimina \n" +
            "WHERE ID_ROL = :PIdRol \n" +
            "AND ID_OPCION_MENU = :PIdOpcionMenu";

    }

}
