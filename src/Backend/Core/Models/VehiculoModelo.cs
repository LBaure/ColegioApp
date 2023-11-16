using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Models
{
    public class VehiculoModelo
    {
        public string? NoPlaca { get; set; }
        public string? NitCliente { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int Anio { get; set; }
        public string? VIN { get; set; }
        public string? Chasis { get; set; }
        public int Kilometraje { get; set; }
        public int NoPuertas { get; set; }
        public string? Observaciones { get; set; }
    }
}
