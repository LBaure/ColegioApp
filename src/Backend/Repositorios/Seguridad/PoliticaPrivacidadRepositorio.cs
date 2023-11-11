using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Seguridad;
using Core.Repositorios;
using Core.Repositorios.Seguridad;
using Core.Servicios;
using Dapper;
using Exceptionless;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repositorios.Seguridad
{
    public class PoliticaPrivacidadRepositorio : IPoliticaPrivacidadRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;

        private readonly IUsuarioActualServicio _usuarioActual;
        public PoliticaPrivacidadRepositorio(IConnectionProvider connectionProvider, IUsuarioActualServicio usuarioActualServicio)
        {
            _connectionProvider = connectionProvider;            
            _usuarioActual = usuarioActualServicio;
        }

        public async Task<ResultadoHttpModelo> ObtenerConfiguracionMFA()
        {
            var usuarioActual = _usuarioActual.Get();
            var param = new DynamicParameters();
            param.Add("@Usuario", usuarioActual?.Nit);

            using var connection = await _connectionProvider.OpenAsync();
            var lista = await connection.QueryFirstOrDefaultAsync<PoliticaPrivacidadModelo>(sqlSelectConfigPolitica, param);
            connection.Close();

            if (lista != null)
            {
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha descargado exitosamente",
                    Titulo = "Configuración MFA",
                    Resultado = lista
                };
            } else
            {
                return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                {
                    Mensaje = $"No se ha encontrado información acerca de la autenticación en dos pasos para el usuario {usuarioActual?.Nombre}",
                    Titulo = "Configuración MFA",
                    Resultado = null
                };
            }
        }


        public async Task<bool> ValidarCredencialesUsuario(IDbConnection _connection, IDbTransaction _transaction, string nitUsuario, string valor)
        {
            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@Usuario", nitUsuario);
                parametros.Add("@Contrasenia", valor);
                var existe = await _connection.QueryFirstOrDefaultAsync<int>(sqlValidarCredenciales, parametros, _transaction);
                return existe == 1;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "ValidarCredencialesUsuario"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ha ocurrido un error al validar las credenciales, por favor comuniquese con el administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<IEnumerable<PoliticaPrivacidadModelo>> ObtenerPoliticaPrivacidad()
        {
            using var connection = await _connectionProvider.OpenAsync();
            var lista = await connection.QueryAsync<PoliticaPrivacidadModelo>(sqlSelectPoliticasPrivacidad);
            connection.Close();
            return lista;
        }

        public async Task<ResultadoHttpModelo> EliminarMFA(string valor)
        {
            // USUARIO LOGUEADO
            var usuarioActual = _usuarioActual.Get();
            // PARAMETROS 
            var param = new DynamicParameters();
            param.Add("@Usuario", usuarioActual?.Nit);
            param.Add("@Contrasenia", valor);

            string eliminarMultifactor =
                "DELETE FROM AD_POLITICA_USUARIO apu \n" +
                "WHERE NIT_USUARIO  = :Usuario \n" +
                "AND VALOR = :Contrasenia";

            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            
            // VALIDAR CREDENCIALES
            bool existe = await ValidarCredencialesUsuario(connection, transaction, usuarioActual.Nit, valor);
            if (!existe)
            {
                var intentos = await ObtenerIntentosPermitidosLogin(usuarioActual.Nit, connection, transaction);
                transaction.Commit();
                var complemento = (intentos.IntentosPermitidos - intentos.IntentosSesion) == 1 ? "<strong>1 intento</strong>" : $"{intentos.IntentosPermitidos - intentos.IntentosSesion} intentos";
                return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                {
                    Mensaje = $"Las credenciales son invalidas, <span class=text-danger> La cuenta se bloqueara temporalmente luego de {complemento} más.</span> verifique y vuelva a intentarlo.",
                    Titulo = "Verificación en dos pasos"
                };
            }

            try
            {
                var filasAfectadas = await connection.ExecuteAsync(eliminarMultifactor, param, transaction);
                if (filasAfectadas > 0)
                {
                    try
                    {
                        param.Add("@IntentosLogin", 0);
                        param.Add("@Activo", 0);

                        var filasAfectadasMod = await connection.ExecuteAsync(sqlActualizarDobleFactorUsuario, param, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ex.ToExceptionless().AddObject(new { 
                            controlador = "PoliticaPrivacidadController", 
                            Repositorio = "PoliticaPrivacidadRepositorio", 
                            Metodo = "EliminarMFA -> Modificar Usuario" 
                        }).Submit();
                        throw new ResponseException("Ocurrio un error al modificar los datos del usuario.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                    }
                }

                transaction.Commit();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "Se ha eliminado correctamente la Autenticación en dos pasos, por favor inicia sesión nuevamente.",
                    Titulo = "Configuración MFA"
                };
            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "EliminarMFA -> Eliminar Politica Usuario"
                }).Submit();
                throw new ResponseException("Ocurrio un error al modificar los datos del usuario.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<ResultadoHttpModelo> InsertarPoliticaUsuario(PoliticaUsuarioModelo politicaUsuarioModelo)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var param = new DynamicParameters();
                param.Add("@Usuario", politicaUsuarioModelo.NitUsuario);
                param.Add("@IdPolitica", politicaUsuarioModelo.IdPolitica);
                param.Add("@Contrasenia", politicaUsuarioModelo.Valor);
                param.Add("@IntentosLogin", politicaUsuarioModelo.IntentosLogin);
                param.Add("@Activo", 1);


                var filasAfectadas = await connection.ExecuteAsync(sqlInsertarPoliticaUsuarios, param, transaction);
                if (filasAfectadas > 0)
                {
                    try
                    {
                        var filasAfectadasMod = await connection.ExecuteAsync(sqlActualizarDobleFactorUsuario, param, transaction);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ex.ToExceptionless().AddObject(new
                        {
                            controlador = "PoliticaPrivacidadController",
                            Repositorio = "PoliticaPrivacidadRepositorio",
                            Metodo = "InsertarPoliticaUsuario -> Modificar Usuario"
                        }).Submit();
                        throw new ResponseException("Ocurrio un error al modificar los datos del usuario.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                    }                    
                }


                transaction.Commit();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "Se ha registrado correctamente la Autenticación en dos pasos, por favor Inicia Sesión nuevamente.",
                    Titulo = "Configuración MFA"
                };

            }
            catch(Exception ex)
            {
                transaction.Rollback();
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "InsertarPoliticaUsuario"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al registrar la verificación en dos pasos, por favor consulte a su administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }
        /// <summary>
        /// Actualiza los intentos de sesion para el usuario, de modo que se llene con los intentos realizados
        /// asi como; que resetee el valor a NULL para volver a iniciar el conteo de intentos de sesion.
        /// </summary>
        /// <param name="_connection"></param>
        /// <param name="_transaction"></param>
        /// <param name="nitUsuario"></param>
        /// <param name="intentosSesion"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task CambiarIntentosSesion(IDbConnection _connection, IDbTransaction _transaction, string nitUsuario, int? intentosSesion)
        {
            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@Usuario", nitUsuario);
                parametros.Add("@Intentos", intentosSesion);
                await _connection.ExecuteAsync(sqlInsertarIntentosSesion, parametros, _transaction);
            }
            catch(Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "CambiarIntentosSesion"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ha ocurrido un error al modificar los intentos de sesión, por favor comuniquese con el administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<IntentosSesionModelo> ObtenerIntentosPermitidosLogin(string nitUsuario, IDbConnection _connection, IDbTransaction _transaction)
        {
            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@Usuario", nitUsuario);

                var resultado = await _connection.QueryFirstOrDefaultAsync<IntentosSesionModelo>(sqlValidarIntentosSesion, parametros, _transaction);

                if (resultado != null)
                {
                    var intentosSesion = resultado.IntentosSesion + 1;

                    if (intentosSesion  < resultado.IntentosPermitidos)
                    {
                        resultado.IntentosSesion = intentosSesion;
                        await CambiarIntentosSesion(_connection, _transaction, nitUsuario, intentosSesion);
                        return resultado;
                    }
                    else
                    {
                        await InactivarUsuarioTemporalmente(nitUsuario, false, _connection, _transaction);
                        _transaction.Commit();
                        throw new ResponseException("La cuenta se ha inactivado temporalmente, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.Unauthorized);
                        // se ha bloqueado.
                    }
                }
                else
                {
                    throw new ResponseException("No se encontraron datos sobre la configuración de politica de privacidad del usuario.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                }


            }
            catch(ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "ValidarIntentosSesion"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al validar los intentos de inicio de sesión.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private static async Task InactivarUsuarioTemporalmente(string nitUsuario, bool estado, IDbConnection _connection, IDbTransaction _transaction)
        {
            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@Usuario", nitUsuario);
                parametros.Add("@Activo", estado ? 1 : 0);
                await _connection.ExecuteAsync(sqlInactivarUsuario, parametros, _transaction);

            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "InactivarUsuarioTemporalmente"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al inactivar/activar usuario.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<ResultadoHttpModelo> ActualizarPoliticaUsuario(CambioCredencialesModelo cambioCredenciales)
        {
            // USUARIO LOGUEADO
            var usuarioActual = _usuarioActual.Get();
            // PARAMETROS 
            var param = new DynamicParameters();
            param.Add("@Usuario", usuarioActual?.Nit);
            param.Add("Intentos", cambioCredenciales.IntentosLogin);

            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();

            // VALIDAR CREDENCIALES
            bool existe = await ValidarCredencialesUsuario(connection, transaction, usuarioActual.Nit, cambioCredenciales.ValorActual);
            if (!existe)
            {
                var intentos = await ObtenerIntentosPermitidosLogin(usuarioActual.Nit, connection, transaction);
                transaction.Commit();
                var complemento = (intentos.IntentosPermitidos - intentos.IntentosSesion) == 1 ? "<strong>1 intento</strong>" : $"{intentos.IntentosPermitidos - intentos.IntentosSesion} intentos";
                return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                {
                    Mensaje = $"Las credenciales son invalidas, <span class=text-danger> La cuenta se bloqueara temporalmente luego de {complemento} más.</span> verifique y vuelva a intentarlo.",
                    Titulo = "Verificación en dos pasos"
                };
            }
            try
            {
                string sqlActualizarPolitica = string.Empty;
                // ACTUALIZAR LOS INTENTOS DE SESION A NULL PARA REINICIAR SU VALOR
                await CambiarIntentosSesion(connection, transaction, usuarioActual.Nit, null);

                if (cambioCredenciales.ValorNuevo != null)
                {
                    try
                    {
                        param.Add("IdPolitica", cambioCredenciales.IdPolitica);
                        param.Add("Valor", cambioCredenciales.ValorNuevo);

                        sqlActualizarPolitica =
                        "UPDATE AD_POLITICA_USUARIO \n" +
                        "SET VALOR = :Valor \n" +
                        "WHERE NIT_USUARIO = :Usuario \n" +
                        "AND ID_POLITICA = :IdPolitica";

                        await connection.ExecuteAsync(sqlActualizarPolitica, param);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ex.ToExceptionless().AddObject(new
                        {
                            controlador = "PoliticaPrivacidadController",
                            Repositorio = "PoliticaPrivacidadRepositorio",
                            Metodo = "ActualizarPoliticaUsuario -> Actualizar Politica"
                        }).AddObject(ex).Submit();
                        throw new ResponseException("Ocurrio un error al actualizar la autenticación en dos pasos, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                    }                    
                }

                string sqlActualizarIntentosSesion =
                   "UPDATE AD_USUARIOS  \n" +
                    "SET INTENTOS_LOGIN = :Intentos  \n" +
                    "WHERE NIT_USUARIO  = :Usuario";

                var filasAfectadasUsuario = await connection.ExecuteAsync(sqlActualizarIntentosSesion, param);
                transaction.Commit();
                string complemento = (cambioCredenciales.ValorNuevo != null) ? ", por favor Inicia Sesión nuevamente." : ".";
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = $"Se ha actualizado correctamente la Autenticación en dos pasos{complemento}",
                    Titulo = "Configuración MFA"
                };
            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "PoliticaPrivacidadController",
                    Repositorio = "PoliticaPrivacidadRepositorio",
                    Metodo = "ActualizarPoliticaUsuario"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al actualizar la autenticación en dos pasos, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private const string sqlSelectPoliticasPrivacidad =
            "SELECT \n" +
            "   ID_POLITICA IdPolitica, \n" +
            "   NOMBRE Nombre, \n" +
            "   DESCRIPCION Descripcion, \n" +
            "   ETIQUETA Etiqueta, \n" +
            "   PATTERN Pattern, \n" +
            "   MINLENGTH Minlength, \n" +
            "   MESSAGE_PATTERN MensajePattern, \n" +
            "   MESSAGE_REQUIRED MensajeRequired, \n" +
            "   MESSAGE_MINLENGTH MensajeMinlength, \n" +
            "   ACTIVO Activo, \n" +
            "   MAXLENGTH Maxlength, \n" +
            "   MASK Mascara, \n" +
            "   CLASS Clase, \n" +
            "   DESCRIPCION_INICIO_SESION DescripcionInicioSesion \n" +
            "FROM AD_POLITICA_SEGURIDAD \n" +
            "WHERE ACTIVO = 1 ";

        private const string sqlSelectConfigPolitica =
        "SELECT  \n" +
        //"   au.DOBLE_FACTOR_AUTH DobleFactor, \n" +
        "   aps.ID_POLITICA IdPolitica, \n" +
        "   aps.NOMBRE Nombre, \n" +
        "   aps.DESCRIPCION Descripcion, \n" +
        "   aps.ETIQUETA Etiqueta, \n" +
        "   aps.PATTERN Pattern, \n" +
        "   aps.MINLENGTH Minlength, \n" +
        "   aps.MESSAGE_PATTERN MensajePattern, \n" +
        "   aps.MESSAGE_REQUIRED MensajeRequired, \n" +
        "   aps.MESSAGE_MINLENGTH MensajeMinlength, \n" +
        "   aps.ACTIVO Activo, \n" +
        "   aps.MAXLENGTH Maxlength, \n" +
        "   aps.MASK Mascara, \n" +
        "   aps.CLASS Clase, \n" +
        "   aps.DESCRIPCION_INICIO_SESION DescripcionInicioSesion  \n" +
        "FROM AD_POLITICA_SEGURIDAD aps \n" +
        "INNER JOIN AD_POLITICA_USUARIO apu  \n" +
        "ON apu.ID_POLITICA = aps.ID_POLITICA \n" +
        "INNER JOIN AD_USUARIOS au  \n" +
        "ON au.NIT_USUARIO = apu.NIT_USUARIO \n" +
        "WHERE apu.NIT_USUARIO = :Usuario";

        private const string sqlValidarCredenciales =
              "SELECT count(1) \n" +
              "FROM AD_POLITICA_USUARIO apu \n" +
              "WHERE NIT_USUARIO  = :Usuario  \n" +
              "AND VALOR = :Contrasenia";



        private const string sqlValidarIntentosSesion =
              "SELECT INTENTOS_LOGIN IntentosPermitidos, \n" +
              "INTENTOS_SESION IntentosSesion \n" +
              "FROM AD_USUARIOS apu \n" +
              "WHERE NIT_USUARIO  = :Usuario";

        private const string sqlInsertarIntentosSesion =
            "UPDATE AD_USUARIOS  \n" +
            "SET INTENTOS_SESION = :Intentos  \n" +
            "WHERE NIT_USUARIO  = :Usuario";



        private const string sqlInactivarUsuario =
            "UPDATE AD_USUARIOS  \n" +
            "SET ACTIVO = :Activo  \n" +
            "WHERE NIT_USUARIO  = :Usuario";

        

        private const string sqlInsertarPoliticaUsuarios =
            "INSERT INTO AD_POLITICA_USUARIO \n" +
            "( \n" +
                "NIT_USUARIO, \n" +
                "ID_POLITICA, \n" +
                "VALOR \n" +
            ") \n" +
            "VALUES \n" +
            "( \n" +
                ":Usuario, \n" +
                ":IdPolitica, \n" +
                ":Contrasenia \n" +
            ")";

        private const string sqlActualizarDobleFactorUsuario =
            "UPDATE AD_USUARIOS \n" +
            "   SET DOBLE_FACTOR_AUTH  = :Activo, \n" +
            "   INTENTOS_LOGIN = :IntentosLogin \n" +
            "WHERE NIT_USUARIO  = :Usuario ";
    }
}
