using Core.Models;
using Core.Models.Seguridad;
using Core.Repositorios.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Seguridad
{
    public class UsuarioServicio : IUsuarioServicio
    {

        private readonly IRolRepositorio _rolRepositorio;
        private readonly IOpcionMenuServicio _menuServicio;

        private readonly IUsuarioRepositorio _usuarioRepositorio;
        public UsuarioServicio(IUsuarioRepositorio usuarioRepositorio, IRolRepositorio rolRepositorio, IOpcionMenuServicio menuServicio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _menuServicio = menuServicio;
            _rolRepositorio = rolRepositorio;
        }

        public async Task<ResultadoHttpModelo> InsertarRolesUsuario(UsuarioRolesModelo usuarioRolesModelo)
        {
            return await _usuarioRepositorio.InsertarRolesUsuario(usuarioRolesModelo);
        }

        public async Task<ResultadoHttpModelo> InsertarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            return await _usuarioRepositorio.InsertarUsuario(usuarioAdministracion);
        }

        public async Task<ResultadoHttpModelo> Login(CredencialesUsuarioModelo credencialesUsuario)
        {
            return await _usuarioRepositorio.Login(credencialesUsuario);
        }

        public async Task<ResultadoHttpModelo> Logout(CredencialesUsuarioModelo credencialesUsuario)
        {
            return await _usuarioRepositorio.Logout(credencialesUsuario);
        }

        public async Task<ResultadoHttpModelo> ModificarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            return await _usuarioRepositorio.ModificarUsuario(usuarioAdministracion);
        }

        public async Task<ConfiguracionUsuarioModelo> ObtenerConfiguracionUsuario(DobleFactorAutenticacionModelo dobleFactorAutenticacion)
        {
            return await _usuarioRepositorio.ObtenerConfiguracionUsuario(dobleFactorAutenticacion);
        }

        public async Task<PerfilUsuarioModelo> ObtenerPerfilUsuarioAsync(string nitUsuario)
        {
            Console.WriteLine("******************ObtenerPerfilUsuarioAsync******************");
            var menu = await _menuServicio.ObtenerMenuAsync(nitUsuario);
            var roles = await _rolRepositorio.ObtenerRolesPorUsuarioAsync(nitUsuario);
            var rolesUsuario = new List<RolUsuarioModelo>();
            foreach (var rol in roles)
            {
                rolesUsuario.Add(new RolUsuarioModelo { IdRol = rol.IdRol, Nombre = rol.Nombre });
            }

            return new PerfilUsuarioModelo
            {
                Menu = menu,
                Roles = rolesUsuario,
            };
        }

        public async Task<ResultadoHttpModelo> ObtenerRolesUsuario(string NitUsuario)
        {
            return await _usuarioRepositorio.ObtenerRolesUsuario(NitUsuario);
        }

        public async Task<IEnumerable<UsuarioInternoModelo>> ObtenerUsuarios(string? nitUsuario)
        {
            return await _usuarioRepositorio.ObtenerUsuarios(nitUsuario);
        }
        public async Task<ResultadoHttpModelo> ObtenerBitacoraRolesUsuario(string NitUsuario)
        {
            return await _usuarioRepositorio.ObtenerBitacoraRolesUsuario(NitUsuario);
        }

        public async Task<ResultadoHttpModelo> EliminarUsuario(UsuarioAdministracionModelo usuarioAdministracion)
        {
            return await _usuarioRepositorio.EliminarUsuario(usuarioAdministracion);
        }

        public async Task<ResultadoHttpModelo> ActualizarMiPerfil(MiUsuarioModelo miUsuario)
        {
            return await _usuarioRepositorio.ActualizarMiPerfil(miUsuario);
        }
    }
}
