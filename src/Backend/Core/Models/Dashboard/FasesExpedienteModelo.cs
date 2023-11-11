using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Dashboard
{
    public class FasesExpedienteModelo : FaseModelo
    {
        public List<EncabezadoExpedienteModelo>? Expedientes {get; set;}
    }

    public class FaseModelo
    {
        public string? IdFase { get; set; }
        public string? NombreFase { get; set; }
        public string? TipoFase { get; set; }
    }

    public class ExpedientesWorkFlowModelo
    {
        public string? NoExpedienteMinfin { get; set; }
        public string? NoExpediente { get; set; }
        public string? FechaGrabacion { get; set; }
        public string? TiempoTranscurrido { get; set; }
        public string? IdTipoExpediente { get; set; }
        public string? Descripcion { get; set; }
        public string? IdFase{ get; set; }
        public string? UsuarioAsignado { get; set; }

    }

    public class FasesConExpedientesWorkFlowModelo
    {
        public string? IdFase { get; set; }
        public string? Descripcion { get; set; }
        public string? IdTipoExpediente { get; set; }
        public List<ExpedientesWorkFlowModelo>? Expedientes { get; set; }
    }
}
