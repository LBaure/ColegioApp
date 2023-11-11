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
    public class RolServicio : IRolServicio
    {
        private readonly IRolRepositorio _rolRepositorio;
        public RolServicio(IRolRepositorio rolRepositorio)
        {
            _rolRepositorio = rolRepositorio;            
        }

        public async Task<ResultadoHttpModelo> EliminarRol(int idRol)
        {
            return await _rolRepositorio.EliminarRol(idRol);
        }

        public async Task<ResultadoHttpModelo> InsertarOpcionesMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            return await _rolRepositorio.InsertarOpcionesMenuPorRol(rolOpcionMenuModelo);
        }
        public async Task<ResultadoHttpModelo> ModificarOpcionesMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            return await _rolRepositorio.ModificarOpcionesMenuPorRol(rolOpcionMenuModelo);
        }

        public async Task<ResultadoHttpModelo> InsertarRol(RolModelo reqRol)
        {
            return await _rolRepositorio.InsertarRol(reqRol);
        }

        public async Task<ResultadoHttpModelo> ModificarRol(RolModelo reqRol)
        {
            return await _rolRepositorio.ModificarRol(reqRol);
        }

        public async Task<ResultadoHttpModelo> ObtenerOpcionesMenuPorRol(int idRol)
        {
            return await _rolRepositorio.ObtenerOpcionesMenuPorRol(idRol);
        }

        public async Task<ResultadoHttpModelo> ObtenerRoles()
        {
            return await _rolRepositorio.ObtenerRoles();
        }

        public async Task<ResultadoHttpModelo> EliminarOpcionMenuPorRol(RolOpcionMenuModelo rolOpcionMenuModelo)
        {
            return await _rolRepositorio.EliminarOpcionMenuPorRol(rolOpcionMenuModelo);
        }
    }
}
