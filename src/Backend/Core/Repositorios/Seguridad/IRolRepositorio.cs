using Core.Models;
using Core.Models.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios.Seguridad
{
    public interface IRolRepositorio
    {
        Task<IEnumerable<RolPorUsuarioModelo>> ObtenerRolesPorUsuarioAsync(string nitUsuario);
        Task<ResultadoHttpModelo> ObtenerRoles();
        Task<ResultadoHttpModelo> InsertarRol(RolModelo reqRol);
        Task<ResultadoHttpModelo> ModificarRol(RolModelo reqRol);
        Task<ResultadoHttpModelo> EliminarRol(int idRol);
        Task<ResultadoHttpModelo> ObtenerOpcionesMenuPorRol(int idRol);
        Task<ResultadoHttpModelo> InsertarOpcionesMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo);
        Task<ResultadoHttpModelo> ModificarOpcionesMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo);
        Task<ResultadoHttpModelo> EliminarOpcionMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo);
    }
}
