using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Seguridad;
using Core.Repositorios;
using Core.Repositorios.Seguridad;
using Dapper;
using Exceptionless;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorios.Seguridad
{
    public class OpcionMenuRepositorio : IOpcionMenuRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;
        public OpcionMenuRepositorio(IConnectionProvider connectionProvider)
        {
            this._connectionProvider = connectionProvider;  
        }

        public async Task<IEnumerable<OpcionMenuModelo>> Get()
        {
            string sqlCmdSelect = @"
            WITH  tree_view(
	            ID_OPCION_MENU,
	            ID_OPCION_MENU_PADRE,
	            NOMBRE,
	            DESCRIPCION,
	            ICONO,
	            URL,
	            ACTIVO,
	            ORDEN,
	            NIVEL,
	            SECUENCIA
            ) AS (
            SELECT 
	            ID_OPCION_MENU,
	            ID_OPCION_MENU_PADRE,
	            NOMBRE,
	            DESCRIPCION,
	            ICONO,
	            URL,
	            ACTIVO,
	            ORDEN,
	            0 AS NIVEL,
	            CAST(ORDEN  AS varchar(50)) AS SECUENCIA
            FROM AD_OPCIONES_MENU 
            WHERE ID_OPCION_MENU_PADRE  IS NULL OR ID_OPCION_MENU_PADRE = 0
            UNION ALL
            SELECT 
	            PADRE.ID_OPCION_MENU,
	            PADRE.ID_OPCION_MENU_PADRE,
	            PADRE.NOMBRE,
	            PADRE.DESCRIPCION,
	            PADRE.ICONO,
	            PADRE.URL,
	            PADRE.ACTIVO,
	            PADRE.ORDEN,
	            tv.NIVEL + 1 AS NIVEL,
            CAST(SECUENCIA || '.' || CAST(PADRE.ORDEN  AS VARCHAR (50)) AS VARCHAR(50)) AS SECUENCIA
            FROM AD_OPCIONES_MENU PADRE
            JOIN tree_view tv
            ON PADRE.ID_OPCION_MENU_PADRE = tv.ID_OPCION_MENU
            )

            SELECT 
	            H.ID_OPCION_MENU idOpcionMenu,
	            H.ID_OPCION_MENU_PADRE idOpcionMenuPadre,
	            H.NOMBRE,
                P.NOMBRE MenuPadre,
	            H.DESCRIPCION,
	            H.ICONO,
	            H.URL,
	            H.ACTIVO,
	            H.ORDEN,
	            H.NIVEL,
	            H.SECUENCIA
            FROM tree_view H
            LEFT JOIN AD_OPCIONES_MENU P
            ON H.ID_OPCION_MENU_PADRE = P.ID_OPCION_MENU 
            ORDER BY H.SECUENCIA";

            using var connection = await _connectionProvider.OpenAsync();
            var lista = await connection.QueryAsync<OpcionMenuModelo>(sqlCmdSelect);
            connection.Close();
            return lista.AsList(); ;
        }

        public async Task<IEnumerable<OpcionesMenuUsuarioModelo>> ObtenerMenuAsync(string nitUsuario)
        {
            using var connection = await _connectionProvider.OpenAsync();
            var menu = await connection.QueryAsync<OpcionesMenuUsuarioModelo>(ObtenerOpcionesPorRolSql, new { NitUsuario = nitUsuario });
            connection.Close();
            return menu;
        }

        public async Task<IEnumerable<OpcionMenuModelo>> ObtenerOpcionesMenu()
        {
            string sqlCmdSelect = @"
                WITH  tree_view(
	                ID_OPCION_MENU,
	                ID_OPCION_MENU_PADRE,
	                NOMBRE,
	                DESCRIPCION,
	                ICONO,
	                URL,
	                ACTIVO,
	                MOSTRAR_MENU,
	                ORDEN,
	                NIVEL,
	                SECUENCIA
                ) AS (
                SELECT 
	                ID_OPCION_MENU,
	                ID_OPCION_MENU_PADRE,
	                NOMBRE,
	                DESCRIPCION,
	                ICONO,
	                URL,
	                ACTIVO,
	                MOSTRAR_MENU,
	                ORDEN,
	                0 AS NIVEL,
	                CAST(ORDEN  AS varchar(50)) AS SECUENCIA
                FROM AD_OPCIONES_MENU 
                WHERE ID_OPCION_MENU_PADRE  IS NULL OR ID_OPCION_MENU_PADRE = 0
                UNION ALL
                SELECT 
	                PADRE.ID_OPCION_MENU,
	                PADRE.ID_OPCION_MENU_PADRE,
	                PADRE.NOMBRE,
	                PADRE.DESCRIPCION,
	                PADRE.ICONO,
	                PADRE.URL,
	                PADRE.ACTIVO,
	                PADRE.MOSTRAR_MENU,
	                PADRE.ORDEN,
	                tv.NIVEL + 1 AS NIVEL,
                CAST(SECUENCIA || '.' || CAST(PADRE.ORDEN  AS VARCHAR (50)) AS VARCHAR(50)) AS SECUENCIA
                FROM AD_OPCIONES_MENU PADRE
                JOIN tree_view tv
                ON PADRE.ID_OPCION_MENU_PADRE = tv.ID_OPCION_MENU
                )

                SELECT 
	                H.ID_OPCION_MENU idOpcionMenu,
	                H.ID_OPCION_MENU_PADRE idOpcionMenuPadre,
	                H.NOMBRE,
                    P.NOMBRE NombreMenuPadre,
	                H.DESCRIPCION,
	                H.ICONO,
	                H.URL,
	                H.ACTIVO,
	                H.MOSTRAR_MENU MostrarMenu,
	                H.ORDEN,
	                H.NIVEL,
	                H.SECUENCIA
                FROM tree_view H
                LEFT JOIN AD_OPCIONES_MENU P
                ON H.ID_OPCION_MENU_PADRE = P.ID_OPCION_MENU 
                ORDER BY H.SECUENCIA";


           
            using var connection = await _connectionProvider.OpenAsync();
            var lista = await connection.QueryAsync<OpcionMenuModelo>(sqlCmdSelect);
            connection.Close();
            return lista.AsList();        
        }

        public async Task<ResultadoHttpModelo> InsertarOpcionMenu(OpcionMenuModelo opcionMenu)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                string sqlInsRoles = @"
                INSERT INTO AD_OPCIONES_MENU ( 
	                ID_OPCION_MENU,
                    ID_OPCION_MENU_PADRE,
                    NOMBRE,
                    DESCRIPCION,
                    ICONO,
                    URL,
                    ACTIVO,
                    ORDEN,
                    MOSTRAR_MENU
                )
                VALUES
                (
                    (SELECT NVL(MAX(ID_OPCION_MENU),0)+1 FROM AD_OPCIONES_MENU),
                    :IdOpcionMenuPadre,
                    :Nombre,    
                    :Descripcion, 
                    :Icono, 
                    :Url, 
                    :Activo,
                    :Orden,
                    :MostrarMenu
                )";

                var param = new DynamicParameters();
                param.Add("@IdOpcionMenuPadre", opcionMenu.IdOpcionMenuPadre > 0 ? opcionMenu.IdOpcionMenuPadre : null);
                param.Add("@Nombre", opcionMenu.Nombre);
                param.Add("@Descripcion", opcionMenu.Descripcion);
                param.Add("@Icono", opcionMenu.Icono);
                param.Add("@Url", opcionMenu.Url);
                param.Add("@Activo", opcionMenu.Activo);
                param.Add("@Orden", opcionMenu.Orden);
                param.Add("@MostrarMenu", opcionMenu.MostrarMenu == true ? 1 : 0);

                var existeRegistro = ValidarRegistroOpcionMenu(opcionMenu.Nombre, opcionMenu.IdOpcionMenu, connection, transaction);
                if (existeRegistro)
                {
                    transaction.Rollback();
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Existe un registro con el mismo nombre, verifique y vuelva a intentarlo.",
                        Titulo = "Registro de Roles"
                    };
                }

                var filasAfectadas = await connection.ExecuteAsync(sqlInsRoles, param);
                if (filasAfectadas > 0)
                {
                    /* Esta validación se cumple si el orden de las opciones del menu cambian.
                     * es decir si hay un registro con el orden 1 y el registro entrante tiene el mismo 
                     * orden, se actualizaran con el listado entrante, el cual fue ordenado en el front-end */
                    if (opcionMenu.Opciones != null)
                    {
                        foreach (var opcion in opcionMenu.Opciones)
                        {
                            string sqlUpdate =
                            "UPDATE AD_OPCIONES_MENU \n" +
                            "   SET orden = :Orden \n" +
                            "WHERE ID_OPCION_MENU = :IdOpcionMenu ";
                            var paramUpdate = new DynamicParameters();
                            paramUpdate.Add("@Orden", opcion.Orden);
                            paramUpdate.Add("@IdOpcionMenu", opcion.IdOpcionMenu);

                            await connection.ExecuteAsync(sqlUpdate, paramUpdate);
                        }
                    }

                    transaction.Commit();
                    var listadoOpcionesMenu = await this.ObtenerOpcionesMenu();

                    return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                    {
                        Mensaje = "La información se ha guardado exitosamente",
                        Titulo = "Registro de Roles",
                        Resultado = listadoOpcionesMenu
                    };
                }
                else
                {
                    transaction.Rollback();
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Advertencia: No se ha ingresado el registro, intente más tarde.",
                        Titulo = "Registro de Roles"
                    };                    
                }

            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        private static async Task<bool> ActualizarOrdenOpcionMenu(OpcionMenuModelo opcionMenu, IDbConnection _conn, IDbTransaction _trx)
        {
            try 
            {
                if (opcionMenu.Opciones != null)
                {
                    foreach (var opcion in opcionMenu.Opciones)
                    {
                        string sqlUpdate =
                        "UPDATE AD_OPCIONES_MENU \n" +
                        "   SET orden = :Orden \n" +
                        "WHERE ID_OPCION_MENU = :IdOpcionMenu ";

                        var paramUpdate = new DynamicParameters();
                        paramUpdate.Add("@Orden", opcion.Orden);
                        paramUpdate.Add("@IdOpcionMenu", opcion.IdOpcionMenu);

                       await _conn.ExecuteAsync(sqlUpdate, paramUpdate, _trx);

                    }
                }    
                return true;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().Submit();
                throw new ResponseException("Ocurrio un error al actualizar el orden de las opciones de menú, cosnulte a su administrador", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }
        public async Task<ResultadoHttpModelo> ActualizarOpcionMenu(OpcionMenuModelo opcionMenu)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                string sqlInsRoles = @"
                UPDATE AD_OPCIONES_MENU 
	            SET
                    ID_OPCION_MENU_PADRE = :IdOpcionMenuPadre,
                    NOMBRE = :Nombre,
                    DESCRIPCION = :Descripcion,
                    ICONO = :Icono,
                    URL = :Url,
                    ACTIVO = :Activo,
                    ORDEN = :Orden,
                    MOSTRAR_MENU = :MostrarMenu
                WHERE ID_OPCION_MENU = :IdOpcionMenu";

                var param = new DynamicParameters();
                param.Add("@IdOpcionMenu", opcionMenu.IdOpcionMenu);
                param.Add("@IdOpcionMenuPadre", opcionMenu.IdOpcionMenuPadre > 0 ? opcionMenu.IdOpcionMenuPadre : null);
                param.Add("@Nombre", opcionMenu.Nombre);
                param.Add("@Descripcion", opcionMenu.Descripcion);
                param.Add("@Icono", opcionMenu.Icono);
                param.Add("@Url", opcionMenu.Url);
                param.Add("@Activo", opcionMenu.Activo);
                param.Add("@Orden", opcionMenu.Orden);
                param.Add("@MostrarMenu", opcionMenu.MostrarMenu == true ? 1 : 0);

                var existeRegistro = ValidarRegistroOpcionMenu(opcionMenu.Nombre, opcionMenu.IdOpcionMenu, connection, transaction);
                if (existeRegistro)
                {
                    transaction.Rollback();
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Existe un registro con el mismo nombre, verifique y vuelva a intentarlo.",
                        Titulo = "Registro de Opciones de Menú"
                    };
                }

                var filasAfectadas = await connection.ExecuteAsync(sqlInsRoles, param);
                if (filasAfectadas > 0)
                {
                    if (opcionMenu.Opciones != null)
                    {
                        await ActualizarOrdenOpcionMenu(opcionMenu, connection, transaction);
                    }

                    transaction.Commit();
                    var listadoOpcionesMenu = await this.ObtenerOpcionesMenu();

                    return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                    {
                        Mensaje = "La información se ha actualizado exitosamente",
                        Titulo = "Registro de Roles",
                        Resultado = listadoOpcionesMenu
                    };

                }
                else
                {
                    transaction.Rollback();
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Advertencia: No se ha realizado la actualización, intente más tarde.",
                        Titulo = "Registro de Roles"
                    };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ex.ToExceptionless().Submit();
                throw;
            }
        }


        private static bool ValidarRegistroOpcionMenu(string? nombre, int? idOpcionMenu, IDbConnection _conn, IDbTransaction _trx)
        {
            string sqlValidarOpcionMenu = "SELECT COUNT(1) \n" +
                                        "FROM AD_OPCIONES_MENU \n" +
                                        "WHERE UPPER(NOMBRE) = UPPER(:NombreOpcion)";

            var param = new DynamicParameters();
            param.Add("@NombreOpcion", nombre);

            if (idOpcionMenu > 0)
            {
                sqlValidarOpcionMenu += "\n AND ID_OPCION_MENU != :IdOpcionMenu";
                param.Add("@IdOpcionMenu", idOpcionMenu);
            }

            var exist = _conn.QueryFirst<int>(sqlValidarOpcionMenu, param, _trx);
            return exist > 0;
        }

        public async Task<ResultadoHttpModelo> EliminarOpcionMenu(int idOpcionMenu)
        {
            string sqlDeleteOpciones = @"DELETE FROM AD_OPCIONES_MENU WHERE ID_OPCION_MENU = :idOpcionMenu";
            var param = new DynamicParameters();
            param.Add("@IdOpcionMenu", idOpcionMenu);
            using var conn = await _connectionProvider.OpenAsync();
            try
            {
                await conn.ExecuteAsync(sqlDeleteOpciones, param);
                conn.Close();
                var listadoOpcionesMenu = await this.ObtenerOpcionesMenu();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "El registro se ha eliminado exitosamente",
                    Titulo = "Eliminación de Roles",
                    Resultado = listadoOpcionesMenu
                };
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("ORA-02292"))
                {
                    throw new ResponseException("Ha ocurrido un error al eliminar el registro, debido a que esta siendo usado en otra instancia del sistema.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }
                ex.ToExceptionless().Submit();   
                throw new ResponseException("Ha ocurrido un error al eliminar el registro.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }

        }

        private const string ObtenerOpcionesPorRolSql =
        "SELECT DISTINCT  \n" +
        "	o.ICONO, \n" +
        "	o.ID_OPCION_MENU IdOpcionMenu, \n" +
        "	o.ID_OPCION_MENU_PADRE IdOpcionMenuPadre, \n" +
        "	o.NOMBRE, \n" +
        "	o.URL, \n" +
        "	o.ORDEN, \n" +
        "	ro.CONSULTA, \n" +
        "	ro.INSERTA, \n" +
        "	ro.MODIFICA, \n" +
        "	ro.ELIMINA, \n" +
        "	o.MOSTRAR_MENU MostrarMenu \n" +
        "FROM AD_OPCIONES_MENU o \n" +
        "JOIN AD_ROLES_OPCIONES_MENU ro  \n" +
        "ON o.ID_OPCION_MENU = ro.ID_OPCION_MENU \n" +
        "JOIN AD_ROLES r  \n" +
        "ON	r.ID_ROL = ro.ID_ROL \n" +
        "JOIN AD_ROLES_USUARIOS ru  \n" +
        "ON r.ID_ROL = ru.ID_ROL \n" +
        "WHERE ru.NIT_USUARIO = :NitUsuario \n" +
        "AND o.ACTIVO  = 1" +
        "ORDER BY \n" +
        "	o.ORDEN, \n" +
        "	o.ID_OPCION_MENU, \n" +
        "	o.NOMBRE ";
    }

}
