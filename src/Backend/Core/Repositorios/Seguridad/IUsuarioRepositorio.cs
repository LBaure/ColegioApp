using Core.Models;
using Core.Models.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios.Seguridad
{
    public interface IUsuarioRepositorio
    {
        Task<IEnumerable<UsuarioInternoModelo>> ObtenerUsuarios(string? nitUsuario);
        Task<ConfiguracionUsuarioModelo> ObtenerConfiguracionUsuario(DobleFactorAutenticacionModelo dobleFactorAutenticacion);
        Task<ResultadoHttpModelo> Login(CredencialesUsuarioModelo credencialesUsuario);
        Task<ResultadoHttpModelo> Logout(CredencialesUsuarioModelo credencialesUsuario);
        Task<ResultadoHttpModelo> InsertarUsuario(UsuarioAdministracionModelo usuarioAdministracion);
        Task<ResultadoHttpModelo> ModificarUsuario(UsuarioAdministracionModelo usuarioAdministracion);
        Task<ResultadoHttpModelo> EliminarUsuario(UsuarioAdministracionModelo usuarioAdministracion);
        Task<ResultadoHttpModelo> ObtenerRolesUsuario(string NitUsuario);
        Task<ResultadoHttpModelo> InsertarRolesUsuario(UsuarioRolesModelo usuarioRolesModelo);
        Task<ResultadoHttpModelo> ObtenerBitacoraRolesUsuario(string NitUsuario);
        Task<ResultadoHttpModelo> ActualizarMiPerfil(MiUsuarioModelo miUsuario);
    }
}
