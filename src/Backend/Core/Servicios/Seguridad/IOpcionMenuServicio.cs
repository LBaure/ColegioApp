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
    public interface IOpcionMenuServicio
    {
        Task<IEnumerable<OpcionMenuModelo>> Get();
        Task<IEnumerable<OpcionesMenuUsuarioModelo>> ObtenerMenuAsync(string nitUsuario);
        Task<IEnumerable<OpcionMenuModelo>> ObtenerOpcionesMenu();
        Task<ResultadoHttpModelo> InsertarOpcionMenu(OpcionMenuModelo opcionMenu);
        Task<ResultadoHttpModelo> ActualizarOpcionMenu(OpcionMenuModelo opcionMenu);
        Task<ResultadoHttpModelo> EliminarOpcionMenu(int idOpcionMenu);
    }
}
