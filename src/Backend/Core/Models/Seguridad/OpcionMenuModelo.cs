using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class OpcionMenuModelo
    {
        public int? IdOpcionMenu { get; set; }
        public int? IdOpcionMenuPadre { get; set; }
        public string? Nombre { get; set; }
        public string? NombreMenuPadre { get; set; }
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
        public string? Url { get; set; }
        public int? Orden { get; set; }
        public int? Activo { get; set; }
        public bool? MostrarMenu { get; set; }
        public List<OpcionMenuModelo>? Opciones { get; set; }
    }
    public class OpcionesMenuUsuarioModelo
    {
        public int? IdOpcionMenu { get; set; }
        public int? IdOpcionMenuPadre { get; set; }
        public string? Nombre { get; set; }
        public string? Url { get; set; }
        public bool? Consulta { get; set; }
        public bool? Inserta { get; set; }
        public bool? Modifica { get; set; }
        public bool? Elimina { get; set; }
        public string? Icono { get; set; }
        public bool? MostrarMenu { get; set; }
        public ICollection<OpcionesMenuUsuarioModelo>? Opciones { get; set; }
    }
}
