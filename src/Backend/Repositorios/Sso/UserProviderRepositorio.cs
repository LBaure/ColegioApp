using Core.Models.Seguridad;
using Core.Models.Sso;
using Core.Repositorios.Sso;
using System;
using Minfin.SSO.Api.Models.TicketAutenticacion;
using Minfin.SSO.Api.Models.Usuario;
using Minfin.SSO.WebApi.Client;
using Minfin.SSO.WebApi.Client.Clients;
using Core.Excepciones;

namespace Repositorios.Sso
{
    public class UserProviderRepositorio : IUserProvider
    {
        private readonly ConfiguracionSsoModelo _configuracion;

        public UserProviderRepositorio(ConfiguracionSsoModelo configuracion)
        {
            _configuracion = configuracion;
        }

        public UsuarioSsoModelo Autenticar(SsoAutModelo SSOReq)
        {
            using (var am = new AuthenticationManager(SSOReq.PrivateKeyXml))
            {
                var serviceEndpointUri = new Uri(_configuracion.ServiceEndpoint);
                var clienteTicket = new TicketAutenticacionClient(serviceEndpointUri, am);
                var clienteUsuario = new UsuarioClient(serviceEndpointUri, am);
                var clienteAcceso = new AccesoClient(serviceEndpointUri, am);

                RespuestaUsar respuesta = new();

                try
                {
                    respuesta = clienteTicket.Usar(SSOReq.TicketAut);
                }
                catch (Exception ex)
                {
                    throw new SsoException("Error al usar el ticket. " + ex.Message);
                }

                if (respuesta.Estado == EstadoRespuestaUsar.SitioNoCoincide)
                {
                    throw new SsoException("El sitio para el cual se generó el ticket no es mismo que lo está validando.");
                }
                else if (respuesta.Estado == EstadoRespuestaUsar.TicketInvalido)
                {
                    throw new SsoException("El ticket proporcionado no es válido o ya está vencido, por favor intente iniciar sesión nuevamente.");
                }

                bool tieneAcceso = false;

                try
                {
                    tieneAcceso = clienteAcceso.Obtener(respuesta.Nit);
                }
                catch (Exception ex)
                {
                    throw new SsoException("Error al obtener acceso para el usuario. " + ex.Message);
                }

                if (!tieneAcceso)
                {
                    throw new SsoException("El usuario no tiene los privilegios para ingresar a esta aplicación.");
                }

                Usuario usuario = new();

                try
                {
                    usuario = clienteUsuario.Obtener(respuesta.Nit);
                }
                catch (Exception ex)
                {
                    throw new SsoException("Error al obtener los datos del usuario. " + ex.Message);
                }


                if (!usuario.Activo)
                {
                    throw new SsoException("El usuario no está activo en el portal de autenticación, por favor contacte al administrador.");
                }

                var usuarioSRBM = new UsuarioSsoModelo()
                {
                    Nit = usuario.Nit,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Activo = usuario.Activo
                };
                return usuarioSRBM;
            }
        }

        public bool CambiarAcceso(SSOAcceso SSOReq)
        {
            using (var am = new AuthenticationManager(SSOReq.PrivateKeyXml))
            {
                var serviceEndpointUri = new Uri(_configuracion.ServiceEndpoint);
                var clienteAcceso = new AccesoClient(serviceEndpointUri, am);
                if (SSOReq.Otorgar)
                {
                    clienteAcceso.Otorgar(SSOReq.Nit, _configuracion.UsuarioRegistranteSso);
                }
                else
                {
                    clienteAcceso.Revocar(SSOReq.Nit, _configuracion.UsuarioRegistranteSso);
                }
                return clienteAcceso.Obtener(SSOReq.Nit);
            }
        }

        public bool ConsultaAccesoSitio(SSOAcceso SSOReq)
        {
            throw new NotImplementedException();
        }

        public string ObtenerCorreoUsuarioSso(SSOObtenerCorreo SSOReq)
        {
            throw new NotImplementedException();
        }

        public UsuarioModelo ObtenerInfoUsuarioSso(SSOObtenerCorreo SSOReq)
        {
            throw new NotImplementedException();
        }

        public ResultadoRegistro RegistrarUsuario(SSORegistrar SSOReq)
        {
            var result = ResultadoRegistro.Ok;
            EstadoRegistro estadoRegistro;
            var usuarioSso = new Usuario()
            {
                Nit = SSOReq.Usuario.Nit,
                Nombre = SSOReq.Usuario.Nombre,
                Correo = SSOReq.Usuario.Correo,
                Activo = SSOReq.Usuario.Activo
            };
            if (string.IsNullOrWhiteSpace(SSOReq.PrivateKeyXml))
            {
                throw new SsoException("La llave privada se encuentra vacía");
            }

            using (var am = new AuthenticationManager(SSOReq.PrivateKeyXml))
            {
                var serviceEndpointUri = new Uri(_configuracion.ServiceEndpoint);
                var clienteTicket = new TicketAutenticacionClient(serviceEndpointUri, am);
                var clienteUsuario = new UsuarioClient(serviceEndpointUri, am);
                estadoRegistro = clienteUsuario.Registrar(usuarioSso, _configuracion.UsuarioRegistranteSso);
                var clienteAcceso = new AccesoClient(serviceEndpointUri, am);
                switch (estadoRegistro)
                {
                    case EstadoRegistro.DatosRequeridos: throw new SsoException("No se recibieron todos los datos requeridos.");
                    case EstadoRegistro.NitInvalido: throw new SsoException("El Nit provisto es inválido.");
                    case EstadoRegistro.UsuarioYaExiste:
                        {
                            var cuentaConAcceso = clienteAcceso.Obtener(usuarioSso.Nit);
                            if (cuentaConAcceso)
                            {
                                return ResultadoRegistro.PermisosAsignados;
                            }
                            break;
                        }
                }
                clienteAcceso.Otorgar(usuarioSso.Nit, _configuracion.UsuarioRegistranteSso);
                return result;
            }

            throw new NotImplementedException();
        }
    }
}
