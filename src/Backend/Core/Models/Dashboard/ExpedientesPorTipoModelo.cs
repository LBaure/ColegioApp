using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Dashboard
{
    public class ExpedientesPorTipoModelo
    {
        public string? IdTipoExpediente { get; set; }
        public string? FechaInicial { get; set;}
        public string? FechaFinal { get; set; }
        public int? Ejercicio { get; set; }
        public string? NoExpediente { get; set; }
        public string[]? FasesMostrar { get; set; }
        public int? EjercicioFiscal { get; set; }
    }

    public class HeaderExpedientesPorTipoModelo
    {
        public string? Text { get; set; }
        public string? Value { get; set; }

    }
}
