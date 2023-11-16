using Core.Constants;
using Core.Exceptions;
using Core.Models;
using Core.Models.Security;
using Core.Repositories;
using Core.Repositories.Security;
using Dapper;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Security
{
    public class MenuPermissionRepository : IMenuPermissionRepository
    {
        private readonly IConnectionProvider _connectionProvider;
        public MenuPermissionRepository(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;            
        }

        public async Task<ResponseHttpModel> GetListMenuPermissions()
        {
            using var connection = await _connectionProvider.OpenAsync();
            try
            {
                string sqlCmdSelect =
                    "WITH recursive cte ( \n" +
                    "	id_permiso, \n" +
                    "	id_permiso_padre, \n" +
                    "	nombre, \n" +
                    "	descripcion, \n" +
                    "	icono, \n" +
                    "	url, \n" +
                    "	orden, \n" +
                    "	activo, \n" +
                    "	mostrar_menu, \n" +
                    "	nivel, \n" +
                    "	secuencia)  \n" +
                    "AS  \n" +
                    "( \n" +
                    "SELECT  \n" +
                    "	id_permiso, \n" +
                    "	id_permiso_padre, \n" +
                    "	nombre, \n" +
                    "	descripcion, \n" +
                    "	icono, \n" +
                    "	url, \n" +
                    "	orden, \n" +
                    "	activo, \n" +
                    "	mostrar_menu, \n" +
                    "	0 AS nivel, \n" +
                    "	CAST(ORDEN  AS char) AS secuencia \n" +
                    "FROM permisos_menu  \n" +
                    "WHERE id_permiso_padre  IS NULL OR id_permiso_padre = 0 \n" +
                    "UNION ALL \n" +
                    "SELECT  \n" +
                    "	PADRE.id_permiso, \n" +
                    "	PADRE.id_permiso_padre, \n" +
                    "	PADRE.nombre, \n" +
                    "	PADRE.descripcion, \n" +
                    "	PADRE.icono, \n" +
                    "	PADRE.url, \n" +
                    "	PADRE.orden, \n" +
                    "	PADRE.activo, \n" +
                    "	PADRE.mostrar_menu, \n" +
                    "	tv.NIVEL + 1 AS nivel, \n" +
                    "CAST(SECUENCIA || '.' || CAST(PADRE.ORDEN  AS char) AS char) AS secuencia \n" +
                    "FROM permisos_menu PADRE \n" +
                    "JOIN cte tv \n" +
                    "ON PADRE.id_permiso_padre = tv.id_permiso \n" +
                    ") \n" +
                    "SELECT  \n" +
                    "	h.id_permiso IdPermission, \n" +
                    "	h.id_permiso_padre IdParentPermission, \n" +
                    "	h.nombre Name, \n" +
                    "	p.nombre NameParentPermission, \n" +
                    "	h.descripcion Description, \n" +
                    "	h.icono Icon, \n" +
                    "	h.url Path, \n" +
                    "	h.orden OrderMenu, \n" +
                    "	h.activo Active, \n" +
                    "   h.mostrar_menu ShowMenu, \n" +
                    "	h.nivel level, \n" +
                    "	h.secuencia secuence \n" +
                    "FROM cte h \n" +
                    "LEFT JOIN permisos_menu p \n" +
                    "ON H.id_permiso_padre = p.id_permiso  \n" +
                    "ORDER BY h.secuencia ";


                var permisosMenu = await connection.QueryAsync<ListMenuPermissionModel>(sqlCmdSelect);
                
                return new ResponseHttpModel
                {
                    Message = "Información obtenida exitosamente.",
                    Status = EstadoSolicitudHttp.success.ToString(),
                    Title = "Menú",
                    Result = permisosMenu
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<ResponseHttpModel> InsertMenuPermissions(ListMenuPermissionModel model)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                string sqlInsRoles = @"
                INSERT INTO permisos_menu (
                    id_permiso_padre,
                    nombre,
                    descripcion,
                    icono,
                    url,
                    orden,
                    activo,
                    mostrar_menu
                )
                VALUES
                (
                    @IdParentPermission,
                    @Name,    
                    @Description, 
                    @Icon, 
                    @Path, 
                    @OrderMenu,
                    @Active,
                    @ShowMenu
                )";

                var param = new DynamicParameters();
                param.Add("@IdParentPermission", model.IdParentPermission > 0 ? model.IdParentPermission : null);
                param.Add("@Name", model.Name);
                param.Add("@Description", model.Description);
                param.Add("@Icon", model.Icon);
                param.Add("@Path", model.Path);
                param.Add("@Active", model.Active == true ? 1 : 0);
                param.Add("@OrderMenu", model.OrderMenu);
                param.Add("@ShowMenu", model.ShowMenu == true ? 1 : 0);

                var recordExists = ValidateRecordExists(model.Name, model.IdPermission, connection, transaction);
                if (recordExists)
                {
                    transaction.Rollback();
                    return new ResponseHttpModel
                    {
                        Message = "Existe un registro con el mismo nombre, verifique y vuelva a intentarlo.",
                        Status = EstadoSolicitudHttp.warning.ToString(),
                        Title = "Menú"
                    };
                }

                var filasAfectadas = await connection.ExecuteAsync(sqlInsRoles, param);
                if (filasAfectadas > 0)
                {
                    /* Esta validación se cumple si el orden de las opciones del menu cambian.
                     * es decir si hay un registro con el orden 1 y el registro entrante tiene el mismo 
                     * orden, se actualizaran con el listado entrante, el cual fue ordenado en el front-end */
                    if (model.MenuOptions != null)
                    {
                        foreach (var opcion in model.MenuOptions)
                        {
                            string sqlUpdate =
                            "UPDATE permisos_menu \n" +
                            "   SET orden = :OrderMenu \n" +
                            "WHERE id_permiso_padre = :IdParentPermission ";
                            var paramUpdate = new DynamicParameters();
                            paramUpdate.Add("@OrderMenu", opcion.OrderMenu);
                            paramUpdate.Add("@IdParentPermission", opcion.IdParentPermission);

                            await connection.ExecuteAsync(sqlUpdate, paramUpdate);
                        }
                    }

                    transaction.Commit();
                    var listadoOpcionesMenu = await this.GetListMenuPermissions();

                    return new ResponseHttpModel
                    {
                        Message = "Registro guardado correctamente.",
                        Status = EstadoSolicitudHttp.success.ToString(),
                        Title = "Menú",
                        Result = listadoOpcionesMenu.Result
                    };
                }
                else
                {
                    transaction.Rollback();
                    return new ResponseHttpModel
                    {
                        Message = "Advertencia: No se ha ingresado el registro, intente más tarde.",
                        Status = EstadoSolicitudHttp.warning.ToString(),
                        Title = "Menú"
                    };
                }

            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private static bool ValidateRecordExists(string? Name, int? IdPermiso, IDbConnection _conn, IDbTransaction _trx)
        {
            string sqlValidarOpcionMenu = 
                "SELECT COUNT(1) \n" +
                "FROM permisos_menu \n" +
                "WHERE UPPER(NOMBRE) = UPPER(@NombreOpcion)";

            var paramSelect = new DynamicParameters();
            paramSelect.Add("@NombreOpcion", Name);

            if (IdPermiso > 0)
            {
                sqlValidarOpcionMenu += "\n AND id_permiso != @IdPermiso";
                paramSelect.Add("@IdPermiso", IdPermiso);
            }

            var exist = _conn.QueryFirst<int>(sqlValidarOpcionMenu, paramSelect, _trx);
            return exist > 0;
        }

        public async Task<ResponseHttpModel> UpdateMenuPermissions(ListMenuPermissionModel permissionModel)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                string sqlInsRoles = @"
                UPDATE permisos_menu 
	            SET
                    id_permiso_padre = @IdParentPermission,
                    nombre = @Name,
                    descripcion = @Description,
                    icono = @Icon,
                    url = @Path,
                    activo = @Active,
                    orden = @OrderMenu,
                    mostrar_menu = @ShowMenu
                WHERE id_permiso = @IdPermission";

                var param = new DynamicParameters();
                param.Add("@IdPermission", permissionModel.IdPermission);
                param.Add("@IdParentPermission", permissionModel.IdParentPermission> 0 ? permissionModel.IdParentPermission : null);
                param.Add("@Name", permissionModel.Name);
                param.Add("@Description", permissionModel.Description);
                param.Add("@Icon", permissionModel.Icon);
                param.Add("@Path", permissionModel.Path);
                param.Add("@Active", permissionModel.Active);
                param.Add("@OrderMenu", permissionModel.OrderMenu);
                param.Add("@ShowMenu", permissionModel.ShowMenu);

                var recordExists = ValidateRecordExists(permissionModel.Name, permissionModel.IdPermission, connection, transaction);
                if (recordExists)
                {
                    transaction.Rollback();
                    return new ResponseHttpModel
                    {
                        Message = "Existe un registro con el mismo nombre, verifique y vuelva a intentarlo.",
                        Status = EstadoSolicitudHttp.warning.ToString(),
                        Title = "Menú"
                    };
                }

                var rowsAffected = await connection.ExecuteAsync(sqlInsRoles, param, transaction);
                if (rowsAffected > 0)
                {
                    if (permissionModel.MenuOptions != null)
                    {
                        await UpdateOptionsMenuOrder(permissionModel, connection, transaction);
                    }

                    transaction.Commit();
                    var listMenuPermissions = await this.GetListMenuPermissions();
                    return new ResponseHttpModel
                    {
                        Message = "Registro actualizado correctamente.",
                        Status = EstadoSolicitudHttp.success.ToString(),
                        Title = "Menú",
                        Result = listMenuPermissions.Result
                    };

                }
                else
                {
                    transaction.Rollback();
                    return new ResponseHttpModel
                    {
                        Message = "Advertencia: No se ha realizado la actualización, intente más tarde.",
                        Status = EstadoSolicitudHttp.warning.ToString(),
                        Title = "Menú"
                    };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
        }

        private static async Task<bool> UpdateOptionsMenuOrder(ListMenuPermissionModel permissionModel, IDbConnection _conn, IDbTransaction _trx)
        {
            try
            {
                if (permissionModel.MenuOptions != null)
                {
                    foreach (var opcion in permissionModel.MenuOptions)
                    {
                        string sqlUpdate =
                        "UPDATE permisos_menu \n" +
                        "   SET orden = @OrderMenu \n" +
                        "WHERE id_permiso = @IdPermission ";

                        var paramUpdate = new DynamicParameters();
                        paramUpdate.Add("@OrderMenu", opcion.OrderMenu);
                        paramUpdate.Add("@IdPermission", opcion.IdPermission);

                        await _conn.ExecuteAsync(sqlUpdate, paramUpdate, _trx);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new ResponseException("Ocurrio un error al actualizar el orden de las opciones de menú, cosnulte a su administrador", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<ResponseHttpModel> DeleteMenuPermissions(int id)
        {
            string sqlDeleteOpciones = @"DELETE FROM permisos_menu WHERE id_permiso = @IdPermission";
            var param = new DynamicParameters();
            param.Add("@IdPermission", id);
            using var conn = await _connectionProvider.OpenAsync();
            try
            {
                await conn.ExecuteAsync(sqlDeleteOpciones, param);
                conn.Close();
                var listMenuPermissions = await this.GetListMenuPermissions();
                return new ResponseHttpModel
                {
                    Message = "Registro eliminado correctamente.",
                    Status = EstadoSolicitudHttp.success.ToString(),
                    Title = "Menú",
                    Result = listMenuPermissions.Result
                };
            }
            catch (Exception ex)
            {
                throw new ResponseException("Ha ocurrido un error al eliminar el registro.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }
    }
}
