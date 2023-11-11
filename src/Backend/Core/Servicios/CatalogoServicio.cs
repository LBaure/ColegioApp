using Core.Models;
using Core.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios
{
    public class CatalogoServicio : ICatalogoServicio
    {
        private readonly ICatalogoRepositorio _catalogoRepositorio;
        public CatalogoServicio(ICatalogoRepositorio catalogoRepositorio)
        {
            _catalogoRepositorio = catalogoRepositorio;            
        }
        public async Task<ResultadoHttpModelo> ObtenerOpcionesMenu()
        {
            return await _catalogoRepositorio.ObtenerOpcionesMenu();
        }

        public async Task<ResultadoHttpModelo> ObtenerRoles()
        {
            return await _catalogoRepositorio.ObtenerRoles();
        }
    }
}
