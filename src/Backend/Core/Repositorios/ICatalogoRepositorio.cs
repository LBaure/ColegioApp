using Core.Models;
using Core.Models.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios
{
    public interface ICatalogoRepositorio
    {
        Task<ResultadoHttpModelo> ObtenerOpcionesMenu();
        Task<ResultadoHttpModelo> ObtenerRoles();
    }
}
