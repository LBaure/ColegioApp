using Core.Models;
using Core.Models.Seguridad;
using Core.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Seguridad
{
    public class UsuarioValidadorServicio : IUsuarioServicio
    {
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly IValidador _validador;

        public UsuarioValidadorServicio(IUsuarioServicio usuarioServicio, IValidador validador)
        {
            _usuarioServicio = usuarioServicio;
            _validador = validador;
        }

        public Task<ResultadoHttpModelo> InsertarRolesUsuario(UsuarioRolesModelo usuarioRolesModelo)
        {
            _validador.Validar(usuarioRolesModelo);
            return _usuarioServicio.InsertarRolesUsuario(usuarioRolesModelo);
        }

        public Task<ResultadoHttpModelo> InsertarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            _validador.Validar(usuarioAdministracion);
            return _usuarioServicio.InsertarUsuario(usuarioAdministracion);
        }

        public Task<ResultadoHttpModelo> Login(CredencialesUsuarioModelo credencialesUsuario)
        {
            _validador.Validar(credencialesUsuario);
            return _usuarioServicio.Login(credencialesUsuario);
        }

        public Task<ResultadoHttpModelo> Logout(CredencialesUsuarioModelo credencialesUsuario)
        {
            _validador.Validar(credencialesUsuario);
            return _usuarioServicio.Logout(credencialesUsuario);
        }

        public Task<ResultadoHttpModelo> ModificarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            _validador.Validar(usuarioAdministracion);
            return _usuarioServicio.ModificarUsuario(usuarioAdministracion);
        }

        public Task<ConfiguracionUsuarioModelo> ObtenerConfiguracionUsuario(DobleFactorAutenticacionModelo dobleFactorAutenticacion)
        {
            _validador.Validar(dobleFactorAutenticacion);
            return _usuarioServicio.ObtenerConfiguracionUsuario(dobleFactorAutenticacion);
        }

        public Task<PerfilUsuarioModelo> ObtenerPerfilUsuarioAsync(string nitUsuario)
        {
            return _usuarioServicio.ObtenerPerfilUsuarioAsync(nitUsuario);
        }

        public Task<ResultadoHttpModelo> ObtenerRolesUsuario(string NitUsuario)
        {
            return _usuarioServicio.ObtenerRolesUsuario(NitUsuario);

        }

        public Task<IEnumerable<UsuarioInternoModelo>> ObtenerUsuarios(string? nitUsuario)
        {
            return _usuarioServicio.ObtenerUsuarios(nitUsuario);
        }
        public Task<ResultadoHttpModelo> ObtenerBitacoraRolesUsuario(string NitUsuario)
        {
            return _usuarioServicio.ObtenerBitacoraRolesUsuario(NitUsuario);

        }

        public Task<ResultadoHttpModelo> EliminarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            _validador.Validar(usuarioAdministracion);
            return _usuarioServicio.EliminarUsuario(usuarioAdministracion);
        }

        public Task<ResultadoHttpModelo> ActualizarMiPerfil(MiUsuarioModelo miUsuario)
        {
            _validador.Validar(miUsuario);
            return _usuarioServicio.ActualizarMiPerfil(miUsuario);
        }
    }
}
