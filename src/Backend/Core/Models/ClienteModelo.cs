using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ClienteModelo
    {
        public string? NitCliente { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? FechaRegistro { get; set; }
        public bool Estado { get; set; }
    }

    public class AgregarClienteModelo
    {
        public ClienteModelo? Cliente { get; set; }
        public VehiculoModelo? Vehiculo { get; set; }
    }
}
