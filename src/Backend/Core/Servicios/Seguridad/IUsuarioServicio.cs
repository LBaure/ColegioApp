using Core.Models;
using Core.Models.Seguridad;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Seguridad
{
    public interface IUsuarioServicio
    {
        Task<IEnumerable<UsuarioInternoModelo>> ObtenerUsuarios(string? nitUsuario);
        Task<ConfiguracionUsuarioModelo> ObtenerConfiguracionUsuario(DobleFactorAutenticacionModelo dobleFactorAutenticacion);
        Task<ResultadoHttpModelo> Login(CredencialesUsuarioModelo credencialesUsuario);
        Task<PerfilUsuarioModelo> ObtenerPerfilUsuarioAsync(string nitUsuario);
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
