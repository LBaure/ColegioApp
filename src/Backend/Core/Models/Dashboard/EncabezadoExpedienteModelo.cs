using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Dashboard
{
    public class EncabezadoExpedienteModelo
    {
        public string? ExpedienteGlobal { get; set; }
        public string? FechaGrabacion { get; set; }
        public string? FechaTraslado { get; set; }
        public string? TiempoTranscurrido { get; set; }
        public string? RecibidoDe { get; set; }
        public string? Origen { get; set; }
        public string? UnidadAdministrativa { get; set; }
        public string? DescripcionIngreso { get; set; }
        public string? ObservacionesIngreso { get; set; }
        public string? UsuarioGrabo { get; set; }
        public string? UsuarioAsignado { get; set; }
        public string? IdFaseActual { get; set; }
        public string? IdFase { get; set; }
        public string? IdTipoExpediente { get; set; }
    }

    public class FiltrosEncabezadoModelo
    {
        public int NoExpediente { get; set; }
        public string? IdTipoExpediente { get; set; }
        public string? ExpedienteGlobal { get; set; }
    }

    public class DatosExpedienteModelo
    {
        public string? Etiqueta { get; set; }
        public string? Valor { get; set; }

    }


    public class HistoricoExpedienteModelo
    {
        public string? DescripcionExpediente { get; set; }
        public string? NoExpediente { get; set; }
        public string? IdFase { get; set; }
        public string? DescripcionFase { get; set; }
        public string? FechaTraslado { get; set; }
        public string? IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public string? Observaciones { get; set; }

    }
}
