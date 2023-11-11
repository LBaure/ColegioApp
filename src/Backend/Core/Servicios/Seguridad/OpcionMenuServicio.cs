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
    public class OpcionMenuServicio : IOpcionMenuServicio
    {
        public readonly IOpcionMenuRepositorio _opcionMenuRepositorio;
        public OpcionMenuServicio(IOpcionMenuRepositorio opcionMenuRepositorio)
        {
            this._opcionMenuRepositorio = opcionMenuRepositorio;            
        }

        public async Task<ResultadoHttpModelo> ActualizarOpcionMenu(OpcionMenuModelo opcionMenu)
        {
            return await _opcionMenuRepositorio.ActualizarOpcionMenu(opcionMenu);
        }

        public async Task<ResultadoHttpModelo> EliminarOpcionMenu(int idOpcionMenu)
        {
            return await _opcionMenuRepositorio.EliminarOpcionMenu(idOpcionMenu);
        }

        public async Task<IEnumerable<OpcionMenuModelo>> Get()
        {
            return await _opcionMenuRepositorio.Get();
        }

        public async Task<ResultadoHttpModelo> InsertarOpcionMenu(OpcionMenuModelo opcionMenu)
        {
            return await _opcionMenuRepositorio.InsertarOpcionMenu(opcionMenu);
        }

        public async Task<IEnumerable<OpcionesMenuUsuarioModelo>> ObtenerMenuAsync(string nitUsuario)
        {
            var menuFinal = new List<OpcionesMenuUsuarioModelo>();
            var opciones = await _opcionMenuRepositorio.ObtenerMenuAsync(nitUsuario);
            //Armando estructura final del menu
            var menusPrincipales = opciones.Where(x => x.IdOpcionMenuPadre == null);
            foreach (var menu in menusPrincipales)
            {
                menuFinal.Add(CargarSubmenus(menu, opciones));
            }
            return menuFinal;
        }

        public async Task<IEnumerable<OpcionMenuModelo>> ObtenerOpcionesMenu()
        {
            return await _opcionMenuRepositorio.ObtenerOpcionesMenu();
        }

        private OpcionesMenuUsuarioModelo CargarSubmenus(OpcionesMenuUsuarioModelo menuPadre, IEnumerable<OpcionesMenuUsuarioModelo> opciones)
        {
            menuPadre.Opciones = opciones.Where(x => x.IdOpcionMenuPadre == menuPadre.IdOpcionMenu).ToList();
            foreach (var menu in menuPadre.Opciones)
            {
                CargarSubmenus(menu, opciones);
            }
            return menuPadre;
        }


    }
}
