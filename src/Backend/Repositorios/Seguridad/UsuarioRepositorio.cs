using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Seguridad;
using Core.Models.Sso;
using Core.Repositorios;
using Core.Repositorios.Seguridad;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Exceptionless;
using Core.Repositorios.Sso;
using Minfin.SSO.Api.Models.Usuario;
using System.IO;

using IoFile = System.IO.File;
using System.Configuration;
using Core.Servicios;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Core.Servicios.Seguridad;
using Newtonsoft.Json.Linq;

namespace Repositorios.Seguridad
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ConfiguracionSsoModelo _configuracionSsoModelo;
        private readonly IConfiguration _configuration;
        private readonly IUserProvider _userProvider;
        private readonly IUsuarioActualServicio _usuarioActual;
        private readonly IPoliticaPrivacidadRepositorio _politicaPrivacidadRepositorio;
        public UsuarioRepositorio(
            IConnectionProvider connectionProvider, 
            ConfiguracionSsoModelo configuracionSsoModelo, 
            IConfiguration configuration, 
            IUserProvider userProvider, 
            IUsuarioActualServicio usuarioActual,
            IPoliticaPrivacidadRepositorio politicaPrivacidadRepositorio
        )
        {
            _connectionProvider = connectionProvider;
            _configuracionSsoModelo = configuracionSsoModelo;   
            _configuration = configuration;
            _userProvider = userProvider;
            _usuarioActual = usuarioActual;
            _politicaPrivacidadRepositorio = politicaPrivacidadRepositorio;
        }

        public async Task<ConfiguracionUsuarioModelo> ObtenerConfiguracionUsuario(DobleFactorAutenticacionModelo dobleFactorAutenticacion)
        {
            string sqlCmdSelect =
                "SELECT \n" +
                "	NIT_USUARIO NitUsuario, \n" +
                "	DOBLE_FACTOR_AUTH DobleFactorAuth, \n" +
                "	INTENTOS_LOGIN IntentosLogin, \n" +
                "	ID_POLITICA IdPolitica, \n" +
                "	DESCRIPCION, \n" +
                "	ETIQUETA, \n" +
                "	MINLENGTH, \n" +
                "	PATTERN, \n" +
                "	MESSAGE_PATTERN MessagePattern, \n" +
                "	MESSAGE_REQUIRED MessageRequired, \n" +
                "	MESSAGE_MINLENGTH MessageMinlength, \n" +
                "	MAXLENGTH, \n" +
                "	MASK, \n" +
                "	CLASS, \n" +
                "	DESCRIPCION_INICIO_SESION DescriptionLogin, \n" +
                "   DOBLE_FACTOR_OBLIGATORIO DobleFactorObligatorio \n" +
                "FROM AD_USUARIOS au \n" +
                "NATURAL LEFT JOIN ( \n" +
                "	SELECT * \n" +
                "	FROM AD_POLITICA_USUARIO \n" +
                "	NATURAL JOIN AD_POLITICA_SEGURIDAD \n" +
                ") x \n" +
                "WHERE NIT_USUARIO = :NitUser \n" +
                "AND ACTIVO = 1";

            var param = new DynamicParameters();
            param.Add("@NitUser", dobleFactorAutenticacion.NitUsuario);

            string sqlCmdExist = "SELECT \n" +
                "	count(1) \n" +
                "FROM AD_USUARIOS \n" +
                "WHERE NIT_USUARIO = :NitUser ";

            string sqlCmdStatus =
                "SELECT \n" +
                "	count(1) \n" +
                "FROM AD_USUARIOS \n" +
                "WHERE NIT_USUARIO = :NitUser \n" +
                "AND ACTIVO = 1";

            using (var conn = await _connectionProvider.OpenAsync())
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int existUser = await conn.QueryFirstAsync<int>(sqlCmdExist, param, transaction);
                        int statusUser = await conn.QueryFirstAsync<int>(sqlCmdStatus, param, transaction);
                        if (existUser < 1)
                        {
                            throw new ResponseException("El usuario no existe.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.Unauthorized);
                        }
                        if (statusUser < 1)
                        {
                            throw new ResponseException("El usuario se encuentra temporalmente INACTIVO.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.Unauthorized);
                        }
                        var user = await conn.QueryFirstAsync<ConfiguracionUsuarioModelo>(sqlCmdSelect, param, transaction);
                        int sesionValida = await GetEstadoSesion(dobleFactorAutenticacion, conn, transaction);
                        user.SesionActiva = sesionValida == 1;
                        return user;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
        public async Task<int> GetEstadoSesion(DobleFactorAutenticacionModelo dobleFactor, IDbConnection _connection, IDbTransaction _trx)
        {
            string sqlCmd = "SELECT \n" +
                "COUNT(1) \n" +
                "FROM AD_SESIONES \n" +
                "WHERE NIT_USUARIO = :NitUser \n" +
                "AND TOKEN = :TokenSesion \n" +
                "AND FECHA_LOGOUT IS NULL \n" +
                "AND FECHA_EXP_TOKEN > SYSDATE";

            var param = new DynamicParameters();
            param.Add("@NitUser", dobleFactor.NitUsuario);
            param.Add("@TokenSesion", dobleFactor.Valor);

            var countSesion = await _connection.QueryFirstAsync<int>(sqlCmd, param, _trx);
            return countSesion;
        }


        public async Task<IEnumerable<UsuarioInternoModelo>> ObtenerUsuarios(string? nitUsuario = null)
        {
            string strSelectUser = @"
            SELECT
                AU.NIT_USUARIO NitUsuario,
                AU.NOMBRE_COMPLETO NombreCompleto,
                AU.EMAIL_INSTITUCIONAL EmailInstitucional,
                AU.EMAIL_PERSONAL EmailPersonal,
                AU.CARGO Cargo,
                AU.TELEFONO Telefono,
                AU.FOTO_PERFIL FotoPerfil,
                AU.FOTO_FISICA FotoFisica,
                TO_CHAR(AU.FECHA_REGISTRO, 'DD/MM/YYYY') FechaRegistro,
                AU.DOBLE_FACTOR_AUTH DobleFactorAuth,
                AU.INTENTOS_LOGIN IntentosLogin,
                AU.ACTIVO Activo,
                AU.DOBLE_FACTOR_OBLIGATORIO DobleFactorObligatorio,
                (SELECT ID_POLITICA FROM AD_POLITICA_USUARIO WHERE NIT_USUARIO = AU.NIT_USUARIO ) AS ExistePolitica
            FROM AD_USUARIOS AU
            WHERE AU.NIT_USUARIO = NVL(:NitUsuario, AU.NIT_USUARIO)";


            var param = new DynamicParameters();
            param.Add("@NitUsuario", nitUsuario);

            using var connection = await _connectionProvider.OpenAsync();
            var result = await connection.QueryAsync<UsuarioInternoModelo>(strSelectUser, param);
            connection.Close();
            return result;
        }

        public async Task<ResultadoHttpModelo> Login(CredencialesUsuarioModelo credencialesUsuario)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var resultado = new ResultadoHttpModelo();

                if (credencialesUsuario.DobleFactor)
                {
                    bool existe = await _politicaPrivacidadRepositorio.ValidarCredencialesUsuario(connection, transaction, credencialesUsuario.NitUsuario, credencialesUsuario.Valor);
                    if (!existe)
                    {
                        var intentos = await _politicaPrivacidadRepositorio.ObtenerIntentosPermitidosLogin(credencialesUsuario.NitUsuario, connection, transaction);
                        transaction.Commit();
                        var complemento = (intentos.IntentosPermitidos - intentos.IntentosSesion) == 1 ? "<strong>1 intento</strong>" : $"{intentos.IntentosPermitidos - intentos.IntentosSesion} intentos";
                        return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                        {
                            Mensaje = $"Las credenciales son invalidas, <span class=text-danger> La cuenta se bloqueara temporalmente luego de {complemento} más.</span> verifique y vuelva a intentarlo.",
                            Titulo = "Verificación en dos pasos"
                        };
                    }
                }
                // Obtener la información del usuario
                var usuario = await ObtenerInformacionUsuario(credencialesUsuario.NitUsuario, connection, transaction);
                // Actualizar los intentos de sesion a null, para resetear el valor e iniciar la cuenta nuevamente.
                await _politicaPrivacidadRepositorio.CambiarIntentosSesion(connection, transaction, credencialesUsuario.NitUsuario, null);
                // Obtener un nuevo token JWT
                string tokenJwt = GenerateToken(usuario);

                var parametros = new DynamicParameters();
                parametros.Add("@Usuario", credencialesUsuario.NitUsuario);
                parametros.Add("@Jwt", tokenJwt);
                // Insertar la sesion del usuario
                await connection.ExecuteAsync(sqlInsertarSesionUsuario, parametros, transaction);
                transaction.Commit();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = tokenJwt,
                    Titulo = "Login"
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
                    controlador = "UsuarioController",
                    Repositorio = "UsuarioRepositorio",
                    Metodo = "Login"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al iniciar sesión, por favor comuniquese con el administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private static async Task<UsuarioSsoModelo> ObtenerInformacionUsuario(string nitUsuario, IDbConnection _connection, IDbTransaction _trx)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Usuario", nitUsuario);
                var usuario = await _connection.QueryFirstOrDefaultAsync<UsuarioSsoModelo>(sqlSelectUsuario, param, _trx);

                if (usuario == null)
                {
                    throw new ResponseException("Usuario no encontrado, verifique y vuelva a intentarlo.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }
                else
                {
                    return usuario;
                }
            }
            catch(Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "UsuarioController",
                    Repositorio = "UsuarioRepositorio",
                    Metodo = "Login"
                }).AddObject(ex).Submit();
                throw new ResponseException($"Ocurrio un error al obtener la informacion del usuario con Nit: {nitUsuario}.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private string GenerateToken(UsuarioSsoModelo usuarioModelo)
        {
            // obtener datos del appconfig
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            var claims = new[]
            {
                new Claim("NumeroNit", usuarioModelo.Nit),
                new Claim("Activo", usuarioModelo.Activo ? "1" : "0"),
                new Claim(JwtRegisteredClaimNames.Name, usuarioModelo.Nombre),
                new Claim(JwtRegisteredClaimNames.Email, usuarioModelo.Correo),
            };

            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidarCredenciales(string nitUsuario, string valor, IDbConnection _connection, IDbTransaction _trx)
        {
            string sqlValidPass = @"
                    SELECT COUNT(1)
                    FROM AD_POLITICA_USUARIO
                    WHERE NIT_USUARIO = :NitUsuario
                    AND VALOR = :Valor";

            var param = new DynamicParameters();
            param.Add("@NitUsuario", nitUsuario);
            param.Add("@Valor", valor);

            var exist = _connection.QueryFirst<int>(sqlValidPass, param, _trx);
            return exist == 1;
        }

        public async Task<ResultadoHttpModelo> Logout(CredencialesUsuarioModelo credencialesUsuario)
        {            
            var param = new DynamicParameters();
            param.Add("@NitUser", credencialesUsuario.NitUsuario);
            param.Add("@Token", credencialesUsuario.Valor);

            using var conn = await _connectionProvider.OpenAsync();
            string sqlCmdSelect =
            "UPDATE AD_SESIONES \n" +
            "SET FECHA_LOGOUT = SYSDATE \n" +
            "WHERE NIT_USUARIO = :NitUser \n" +
            "AND TOKEN = :Token";
            try
            {
                int filasAfectadas = await conn.ExecuteAsync(sqlCmdSelect, param);
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Titulo = "Cerrar Sesión",
                    Mensaje = "Se ha cerrado correctamente la sesión."                    
                };
            }
            catch (Exception)
            {
                throw new ResponseException("Ha ocurrido un error al cerrar la sesión, intente más tarde.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.InternalServerError);
            }
        }

        public async Task<ResultadoHttpModelo> InsertarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                var existeUsuario = ValidarRegistroUsuario(usuarioAdministracion, connection, transaction);
                if (existeUsuario)
                {
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = "Advertencia: Existe un usuario registrado con el mismo NIT, verifique y vuelva a intentarlo.",
                        Titulo = "Registro de Usuario"
                    };
                }

                var param = new DynamicParameters();
                param.Add("@NitUsuario", usuarioAdministracion.NitUsuario);
                param.Add("@NombreCompleto", usuarioAdministracion.NombreCompleto);
                param.Add("@EmailInstitucional", usuarioAdministracion.EmailInstitucional);
                param.Add("@Cargo", usuarioAdministracion.Cargo);
                param.Add("@Activo", usuarioAdministracion.Activo ? 1 : 0);
                param.Add("@DobleFactorObligatorio", usuarioAdministracion.DobleFactorObligatorio ? 1 : 0);
                param.Add("@NitUsuarioLogin", usuarioAdministracion.NitUsuario);

                var filasAfectadas = await connection.ExecuteAsync(sqlCmdInsertarUsuario, param);
                var usuarioSSO = await registrarUsuarioSso(usuarioAdministracion);

                transaction.Commit();
                var listadoUsuarios = await ObtenerUsuarios();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha guardado exitosamente",
                    Titulo = "Registro de Usuarios",
                    Resultado = listadoUsuarios
                };                
            }
            catch(ResponseException)
            {
                transaction.Rollback();
                throw;
            }
            catch(Exception ex)
            {
                transaction.Rollback(); 
                ex.ToExceptionless().AddObject(new { servicio = "InsertarUsuario", controlador = "UsuariosController" }).Submit();
                throw new ResponseException("Ha ocurrido un error al eliminar el registro.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private static bool ValidarRegistroUsuario(UsuarioAdministracionModelo usuarioAdministracion, IDbConnection _conn, IDbTransaction _trx)
        {
            string sqlValidarOpcionMenu = 
                "SELECT COUNT(1) \n" +
                "FROM AD_USUARIOS \n" +
                "WHERE UPPER(NIT_USUARIO) = UPPER(:ValidarNitUsuario)";

            var param = new DynamicParameters();
            param.Add("@ValidarNitUsuario", usuarioAdministracion.NitUsuario);
            
            var exist = _conn.QueryFirst<int>(sqlValidarOpcionMenu, param, _trx);
            return exist > 0;
        }

        public async Task<bool> registrarUsuarioSso(UsuarioAdministracionModelo administracionModelo)
        {
            try
            {
                var usuarioSso = new UsuarioSsoModelo()
                {
                    Nit = administracionModelo.NitUsuario ?? "",
                    Activo = true,
                    Nombre = administracionModelo.NombreCompleto ?? "",
                    Correo = administracionModelo.EmailInstitucional ?? ""
                };

                var privateKeyXml = GetPrivateKeyXml(_configuracionSsoModelo.PrivateKeyFile);

                SSORegistrar SSOReq = new()
                {
                    Usuario = usuarioSso,
                    PrivateKeyXml = privateKeyXml
                };
                var result = _userProvider.RegistrarUsuario(SSOReq);
                return result > 0;
            }
            catch (SsoException se)
            {
                se.ToExceptionless().AddObject(new
                {
                    Servicio = "UsuarioRepositorio.cs",
                    Metodo = "registrarUsuarioSso",
                    Usuario = administracionModelo.NitUsuario,
                }, name: "Datos Generales").AddObject(se).Submit();
                throw new ResponseException("Ocurrio un problema al grabar el usuario en el Sistema de Acceso de Usuarios",EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.InternalServerError);
            }
        }

        private static string GetPrivateKeyXml(string path)
        {
            if (!IoFile.Exists(path))
            {
                throw new ResponseException("No existe una llave privada para validar la autenticación.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }

            return IoFile.ReadAllText(path);
        }

        public async Task<ResultadoHttpModelo> ModificarUsuario(UsuarioAdministracionModelo usuario)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();
            var usuarioActual = _usuarioActual.Get();

            try
            {
                var param = new DynamicParameters();
                param.Add("@NitUsuario", usuario.NitUsuario);
                param.Add("@NombreCompleto", usuario.NombreCompleto);
                param.Add("@EmailInstitucional", usuario.EmailInstitucional);
                param.Add("@Cargo", usuario.Cargo);
                param.Add("@Activo", usuario.Activo ? 1 : 0);
                param.Add("@DobleFactorObligatorio", usuario.DobleFactorObligatorio ? 1 : 0);
                param.Add("@NitUsuarioLogin", usuario.NitUsuario);

                var filasAfectadas = await connection.ExecuteAsync(sqlCmdModificarUsuario, param);
                // se obtiene la informacion del usuario, debido a que el sso, devuelve un error cuando se le
                // envia el mismo estado del usuario ejemplo: esta activo y se envia activo.
                // se valida que el estado sea distinto para enviarlo al SSO. LINEA 474
                var usuarios = await ObtenerUsuarios(usuario.NitUsuario);
                var datosUsuario = usuarios.FirstOrDefault();

                // Si se actualizo el estado del usuario a inactivo, se debe quitar los accesos al SSO
                if (filasAfectadas > 0)
                {
                    try
                    {
                        var privateKeyXml = GetPrivateKeyXml(_configuracionSsoModelo.PrivateKeyFile);
                        SSOAcceso SSOReq = new()
                        {
                            Nit = usuario.NitUsuario ?? "",
                            PrivateKeyXml = privateKeyXml,
                            Otorgar = usuario.Activo
                        };

                        if (datosUsuario != null && datosUsuario.Activo != usuario.Activo)
                        {
                            _userProvider.CambiarAcceso(SSOReq);
                        }

                    }
                    catch (ResponseException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        string estado = usuario.Activo ? "Activo" : "Inactivo";
                        ex.ToExceptionless().AddObject(new
                        {
                            servicio = "ModificarUsuario", 
                            controlador = "UsuariosController",
                            sso = $"Cambiar acceso al usuario: {usuario.NitUsuario} con el estado {estado}",
                            usuarioLogin = usuario.NitUsuario
                        }).Submit();
                        throw new ResponseException("Ha ocurrido un error al cambiar el acceso al usuario en el Sistema SSO.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                    }
                }

                transaction.Commit();
                var listadoUsuarios = await ObtenerUsuarios();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha guardado exitosamente",
                    Titulo = "Registro de Usuarios",
                    Resultado = listadoUsuarios
                };
            }
            catch (ResponseException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ex.ToExceptionless().AddObject(new { servicio = "ModificarUsuario", controlador = "UsuariosController" }).Submit();
                throw new ResponseException("Ha ocurrido un error al modificar el registro.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }

        }

        public async Task<ResultadoHttpModelo> ObtenerRolesUsuario(string NitUsuario)
        {
            string strSelectUser = @"
            SELECT 
                NIT_USUARIO NitUsuario,
                '['|| LISTAGG(ID_ROL, ',') WITHIN GROUP (ORDER BY ID_ROL) ||']' AS Roles
            FROM AD_ROLES_USUARIOS WHERE NIT_USUARIO = :Usuario
            GROUP BY NIT_USUARIO";

            using var connection = await _connectionProvider.OpenAsync();
            var param = new DynamicParameters();
            param.Add("@Usuario", NitUsuario);

            try
            {
                var lista = await connection.QueryAsync<UsuarioRolesModelo>(strSelectUser, param);
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Titulo = "Cerrar Sesión",
                    Mensaje = "Se ha cerrado correctamente la sesión.",
                    Resultado = lista
                };
            }
            catch (Exception)
            {
                throw new ResponseException("Ha ocurrido un error al cerrar la sesión, intente más tarde.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.InternalServerError);
            }


        }

        public async Task<ResultadoHttpModelo> InsertarRolesUsuario(UsuarioRolesModelo usuarioRolesModelo)
        {
            using var connection = await _connectionProvider.OpenAsync();
            using var transaction = connection.BeginTransaction();

            var usuarioActual = _usuarioActual.Get();
            if (usuarioActual == null)
            {
                throw new ResponseException("Ha ocurrido un error, el usuario no esta autorizado para realizar esta acción.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.Unauthorized);
            }

            var param = new DynamicParameters();
            param.Add("@NitUsuario", usuarioRolesModelo.NitUsuario);
            param.Add("@NitUsuarioLogin", usuarioActual.Nit);

            // ROLES QUE SE VAN AGREGAR AL USUARIO DESDE EL MODELO.
            var roles = JsonConvert.DeserializeObject<List<int>>(usuarioRolesModelo.Roles ?? "") ?? new List<int>();
            // SE CONSULTA LOS ROLES QUE TIENE EL USUARIO ACTUALMENTE
            var rolesActual = await connection.QueryAsync<RolUsuarioModelo>(rolesActualesUsuario, param);
            try
            {

                // roles eliminados 
                foreach (var rol in rolesActual)
                {
                    // Se agrega este parametro para realizar las validaciones.
                    param.Add("@IdRol", rol.IdRol);
                    var existe = roles.Where(elemento => elemento == rol.IdRol);
                    if (!existe.Any())
                    {
                        try
                        {
                            await ProcesarRolesUsuario(param, sqlEliminarRolesUsuario, 0, connection, transaction);
                        }
                        catch (ResponseException rex)
                        {
                            throw rex;
                        }
                    }
                }

                // roles entrantes
                foreach (var rol in roles)
                {
                    // Se agrega este parametro para realizar las validaciones.
                    param.Add("@IdRol", rol);
                    var existe = rolesActual.Where(elemento => elemento.IdRol == rol);
                    if (!existe.Any())
                    {
                        try
                        {
                            await ProcesarRolesUsuario(param, sqlInstarRolesUsuario, 1, connection, transaction);
                        }
                        catch (ResponseException rex)
                        {
                            throw rex;
                        }                       
                    }
                }
                
                transaction.Commit();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "La información se ha guardado exitosamente",
                    Titulo = "Registro de Roles"
                };
            }
            catch(ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ex.ToExceptionless().AddObject(new { controlador = "UsuariosController", Repositorio = "UsuarioRepositorio", Metodo = "InsertarRolesUsuario" }).Submit();
                throw new ResponseException("Ha ocurrido un error al insertar el registro.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parametros">parametros que se crearon con el modelo del metodo donde se llama a este otro metodo.</param>
        /// <param name="scriptEjecutar">Este puede ser eliminar roles o insertar roles</param>
        /// <param name="activo">Este valor nos permite enviar a bitacora si es alta o baja el registro.</param>
        /// <param name="_connection"></param>
        /// <param name="_transaction"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        private static async Task ProcesarRolesUsuario(DynamicParameters parametros, string scriptEjecutar, int activo, IDbConnection _connection, IDbTransaction _transaction)
        {
            try
            {         
                int filasAfectadas = await _connection.ExecuteAsync(scriptEjecutar, parametros, _transaction);
                if (filasAfectadas > 0)
                {
                    string sqlBuscarRol = "SELECT NOMBRE FROM AD_ROLES WHERE ID_ROL = :IdRol";
                    string nombreRol = await _connection.QueryFirstAsync<string>(sqlBuscarRol, parametros, _transaction);

                    if (nombreRol != null)
                    {
                        try
                        {
                            parametros.Add("@NombreRol", nombreRol);
                            parametros.Add("@Activo", activo);
                            int filaAfectada = await _connection.ExecuteAsync(sqlInstarBitacoraRolesUsuario, parametros, _transaction);
                        }
                        catch (Exception ex)
                        {
                            _transaction.Rollback();
                            ex.ToExceptionless().AddObject(new { controlador = "UsuariosController", Repositorio = "UsuarioRepositorio", Metodo = "InsertarRolesUsuario" }).Submit();
                            throw new ResponseException("Ha ocurrido un error al insertar el registro en bitacora.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                        }
                    }
                    else
                    {
                        _transaction.Rollback();
                        throw new ResponseException($"No se encontro el nombre del rol seleccionado.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                ex.ToExceptionless().AddObject(new { controlador = "UsuariosController", Repositorio = "UsuarioRepositorio", Metodo = "InsertarRolesUsuario" }).Submit();
                string metodo = activo == 1 ? "insertar" : "eliminar";
                throw new ResponseException($"Ha ocurrido un error al {metodo} roles al usuario.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }

        }

        public async Task<ResultadoHttpModelo> ObtenerBitacoraRolesUsuario(string NitUsuario)
        {
            using var connection = await _connectionProvider.OpenAsync();
            var param = new DynamicParameters();
            param.Add("@Usuario", NitUsuario);

            try
            {
                var lista = await connection.QueryAsync<BitacoraUsuarioRolesModelo>(sqlObtenerBitacora, param);
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Titulo = "Bitacora de roles usuario",
                    Mensaje = "Datos descargados exitosamente.",
                    Resultado = lista
                };
            }
            catch (Exception)
            {
                throw new ResponseException("Ha ocurrido un error al obtener los datos de la bitacora, intente más tarde.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.InternalServerError);
            }

        }

        public async Task<ResultadoHttpModelo> EliminarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            var param = new DynamicParameters();
            param.Add("@NitUsuario", usuarioAdministracion.NitUsuario);

            string sqlDelete = @"DELETE FROM AD_USUARIOS WHERE NIT_USUARIO = :NitUsuario";
            using var connection = await _connectionProvider.OpenAsync();
            try
            {
                var filasAfectadas = await connection.ExecuteAsync(sqlDelete, param);
                
                if (filasAfectadas > 0)
                {
                    try
                    {
                        var privateKeyXml = GetPrivateKeyXml(_configuracionSsoModelo.PrivateKeyFile);
                        SSOAcceso SSOReq = new()
                        {
                            Nit = usuarioAdministracion.NitUsuario ?? "",
                            PrivateKeyXml = privateKeyXml,
                            Otorgar = false
                        };
                        _userProvider.CambiarAcceso(SSOReq);
                    }
                    catch (Exception ex)
                    {
                        string estado = "Inactivo";
                        ex.ToExceptionless().AddObject(new
                        {
                            servicio = "ModificarUsuario",
                            controlador = "UsuariosController",
                            sso = $"Cambiar acceso al usuario: {usuarioAdministracion.NitUsuario} con el estado {estado}"
                        }).Submit();
                        throw new ResponseException("Ha ocurrido un error al cambiar el acceso al usuario en el Sistema SSO.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                    }
                }
                var listadoUsuarios = await ObtenerUsuarios();
                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "El usuario se ha eliminado exitosamente",
                    Titulo = "Eliminación de Usuarios",
                    Resultado = listadoUsuarios
                };
            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("ORA-02292"))
                {
                    return new ResultadoHttpModelo(EstadoSolicitudHttp.warning)
                    {
                        Mensaje = $"No se ha podido eliminar al usuario {usuarioAdministracion.NombreCompleto} debido a que esta siendo usado en otras instancias del sistema.",
                        Titulo = "Usuarios"
                    };
                }
                else
                {
                    throw new ResponseException($"Ha ocurrido un error al eliminar al usuario {usuarioAdministracion.NombreCompleto}, consulte al administrador del sistema.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
                }
            }
            
        }

        public async Task<ResultadoHttpModelo> ActualizarMiPerfil(MiUsuarioModelo miUsuario)
        {
            string sqlInsertUsuario = @"
            UPDATE AD_USUARIOS
            SET		            
	            NOMBRE_COMPLETO = :NombreCompleto,
	            EMAIL_PERSONAL = :EmailPersonal,
	            TELEFONO = :Telefono
            WHERE NIT_USUARIO = :NitUsuario";

            var param = new DynamicParameters();
            param.Add("@NitUsuario", miUsuario.NitUsuario);
            param.Add("@NombreCompleto", miUsuario.NombreCompleto);
            param.Add("@EmailPersonal", miUsuario.EmailPersonal);
            param.Add("@Telefono", miUsuario.Telefono);

            using var conn = await _connectionProvider.OpenAsync();
            await conn.ExecuteAsync(sqlInsertUsuario, param);
            return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
            {
                Mensaje = "La información se ha actualizado correctamente.",
                Titulo = "Mi perfil"
            };

        }

        private const string sqlCmdInsertarUsuario = @"
            INSERT INTO AD_USUARIOS
            (	
	            NIT_USUARIO,
	            NOMBRE_COMPLETO,
	            EMAIL_INSTITUCIONAL,
	            CARGO,
	            FECHA_REGISTRO,
	            ACTIVO,
	            USUARIO_GUARDO,
                DOBLE_FACTOR_OBLIGATORIO
            )
            VALUES 
            (
	            :NitUsuario,
	            :NombreCompleto,
	            :EmailInstitucional,
	            :Cargo,
	            SYSDATE,
	            :Activo,
	            :NitUsuarioLogin,
	            :DobleFactorObligatorio
            )";

        private const string sqlCmdModificarUsuario = @"
            UPDATE AD_USUARIOS
            SET		            
	            NOMBRE_COMPLETO = :NombreCompleto,
	            EMAIL_INSTITUCIONAL = :EmailInstitucional,
	            CARGO = :Cargo,
	            ACTIVO = :Activo,
	            DOBLE_FACTOR_OBLIGATORIO = :DobleFactorObligatorio
            WHERE NIT_USUARIO = :NitUsuario";


        private const string sqlInstarBitacoraRolesUsuario =
               "INSERT INTO BITACORA_ROLES_USUARIO \n" +
               "( \n" +
               "   ID_BITACORA_ROLES_USUARIO, " +
               "   NIT_USUARIO, \n" +
               "   ID_ROL, \n" +
               "   ROL, \n" +
               "   ACTIVO," +
               "   USUARIO_REGISTRO \n" +
               ")  \n" +
               "VALUES \n" +
               "( \n" +
               "   (SELECT NVL(MAX(ID_BITACORA_ROLES_USUARIO), 0) + 1 FROM BITACORA_ROLES_USUARIO), \n" +
               "   :NitUsuario, \n" +
               "   :IdRol, \n" +
               "   :NombreRol, \n" +
               "   :Activo, \n" +
               "   :NitUsuarioLogin \n" +
               ")";

       
        private const string rolesActualesUsuario =
            "SELECT \n" +
            "	aru.ID_ROL IdRol, \n" +
            "	ar.NOMBRE  Nombre \n" +
            "FROM AD_ROLES_USUARIOS aru  \n" +
            "INNER JOIN AD_ROLES ar \n" +
            "ON ar.ID_ROL = aru.ID_ROL  \n" +
            "WHERE	NIT_USUARIO = :NitUsuario \n";

        private const string sqlInstarRolesUsuario =
            "INSERT INTO AD_ROLES_USUARIOS \n" +
            "( \n" +
            "   NIT_USUARIO, \n" +
            "   ID_ROL, \n" +
            "   NIT_REGISTRO \n" +
            ")  \n" +
            "VALUES \n" +
            "( \n" +
            "   :NitUsuario, \n" +
            "   :IdRol, \n" +
            "   :NitUsuarioLogin \n" +
            ")";


        private const string sqlEliminarRolesUsuario = 
            "DELETE FROM AD_ROLES_USUARIOS \n" +
            "WHERE NIT_USUARIO = :NitUsuario \n" +
            "AND ID_ROL = :IdRol";


        private const string sqlObtenerBitacora =
            "SELECT \n" +
            "   bru.NIT_USUARIO NitUsuario,\n" +
            "   bru.ROL Rol, \n" +
            "   bru.ACTIVO Activo,\n" +
            "   bru.USUARIO_REGISTRO UsuarioRegistro, \n" +
            "   au.NOMBRE_COMPLETO NombreUsuarioRegistro, \n" +
            "   TO_CHAR(bru.FECHA_REGISTRO, 'DD/MM/YYYY hh:mi:ss am') FechaRegistro \n" +
            "FROM BITACORA_ROLES_USUARIO bru \n" +
            "INNER JOIN AD_USUARIOS au \n" +
            "ON au.NIT_USUARIO = bru.USUARIO_REGISTRO \n" +
            "WHERE bru.NIT_USUARIO = :Usuario \n" +
            "ORDER BY bru.ID_ROL DESC, bru.FECHA_REGISTRO DESC ";


        private const string sqlInsertarSesionUsuario =
            "INSERT INTO AD_SESIONES \n" +
            "( \n" +
            "    ID_SESION, \n" +
            "    NIT_USUARIO, \n" +
            "    TOKEN, \n" +
            "    FECHA_LOGIN, \n" +
            "    FECHA_LOGOUT, \n" +
            "    FECHA_EXP_TOKEN \n" +
            ") \n" +
            "VALUES \n" +
            "( \n" +
            "    (SELECT nvl(max(ID_SESION) ,0) + 1 FROM   AD_SESIONES), \n" +
            "    :Usuario, \n" +
            "    :Jwt, \n" +
            "    SYSDATE, \n" +
            "    NULL, \n" +
            "    SYSDATE + (1/24*12) -- la sesion dura 6 horas \n" +
            ")";

        private const string sqlSelectUsuario =
            "SELECT " +
            "   NIT_USUARIO Nit, \n" +
            "   NOMBRE_COMPLETO Nombre, \n" +
            "   EMAIL_INSTITUCIONAL Correo, \n" +
            "   ACTIVO \n" +
            "FROM AD_USUARIOS au \n" +
            "WHERE NIT_USUARIO = :Usuario \n" +
            "AND ACTIVO = 1 ";
    }
}
