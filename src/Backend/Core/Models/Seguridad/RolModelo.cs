using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class RolPorUsuarioModelo
    {
        public int IdRol { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public ICollection<UsuarioModelo>? Usuarios { get; set; }
        public ICollection<OpcionMenuModelo>? OpcionesMenu { get; set; }
        public int? IdInstitucion { get; set; }
        public string? Institucion { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public string? NitUsuarioAsignacion { get; set; }
        public string? NombreUsuarioAsignacion { get; set; }
    }

    public class RolUsuarioModelo
    {
        public int IdRol { get; set;}
        public string? Nombre { get; set;}
    }

    public class RolModelo
    {
        public int IdRol { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public int? ContadorOpcionesMenu { get; set; }
    }
    public class RolOpcionMenuModelo
    {
        public int IdRol { get; set; }
        public int IdOpcionMenu { get; set; }
        public string? Rol { get; set; }
        public string? OpcionMenu { get; set; }
        public string? DescripcionOpcion { get; set; }
        public bool Consulta { get; set; }
        public bool Inserta { get; set; }
        public bool Modifica { get; set; }
        public bool Elimina { get; set; }
    }
}
