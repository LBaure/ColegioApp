using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class UsuarioModelo
    {
        public string? NitUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public int Activo { get; set; }
        public string? NitRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ICollection<RolPorUsuarioModelo>? Roles { get; set; }
    }
    public class UsuarioAdministracionModelo
    {
        public string? NitUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? EmailInstitucional { get; set; }
        public string? Cargo { get; set; }
        public bool Activo { get; set; }
        public bool DobleFactorObligatorio { get; set; }
    }
    public class UsuarioInternoModelo : UsuarioAdministracionModelo
    {
        public string? EmailPersonal { get; set; }
        public string? Telefono { get; set; }
        public string? FotoPerfil { get; set; }
        public string? FotoFisica { get; set; }
        public string? FechaRegistro { get; set; }
        public int IntentosLogin { get; set; }
        public bool? DobleFactorAuth { get; set; }
        public bool? ExistePolitica { get; set; }
    }

    public class UsuarioRolesModelo
    {
        public string? NitUsuario { get; set; }
        public string? Roles { get; set; }
    }


    public class BitacoraUsuarioRolesModelo
    {
        public string? NitUsuario { get; set; }
        public string? Rol { get; set; }
        public bool Activo { get; set; }
        public string? UsuarioRegistro { get; set; }
        public string? NombreUsuarioRegistro { get; set; }
        public string? FechaRegistro { get; set; }

    }
    public class MiUsuarioModelo
    {
        public string? NitUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? EmailPersonal { get; set; }
        public string? Telefono { get; set; }
    }

    public class IntentosSesionModelo
    {
        public int IntentosPermitidos { get; set; }
        public int IntentosSesion { get; set; }
    }


}
