using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Dashboard
{
    public class DashboardModelo
    {
        public string? IdTipoExpediente { get; set; }
        public string? FechaInicial { get; set; }
        public string? FechaFinal { get; set; }
        public CantidadExpedientesPorFaseModelo cantidadExpedientesPorFase {  get; set; }
        public List<CantidadExpedientesPorUsuarioModelo> cantidadExpedientesPorUsuarios { get; set; }
        public List<string> EjeX { get; set; }
        public List<int> EjeYGenerados { get; set; }
        public List<int> EjeYFinalizados { get; set; }


    }

    public class UnidadesAdministrativasModelo
    {
        public string? IdTipoExpediente { get; set; }
        public int? anio { get; set; }
        public string? IdUnidad { get; set; }
        public string? NombreUnidad { get; set; }
        public int? Cantidad { get; set; }
    }

    public class CantidadExpedientesPorFaseModelo
    {
        public string? IdTipoExpediente { get; set; }
        public int? Total { get; set; }
        public List<FaseExpedienteModelo>? Fases { get; set; }
    }

    public class FaseExpedienteModelo
    {
        public string? IdTipoExpediente { get; set; }
        public string? IdFase { get; set; }
        public string? Descripcion { get; set; }
        public int? Total { get; set; }
        public int? UltimoMes { get; set; }  
    
    }

    public class CantidadExpedientesPorUsuarioModelo
    {
        public string? IdTipoExpediente { get; set; }
        public string? IdUsuario { get; set; }
        public string? IdUnidadAdministrativa { get; set; }
        public string? Extension { get; set; }
        public string? Descripcion { get; set; }
        public int? Total { get; set; }
    }

    public class ExpedientesConFaseModelo
    {
        public int? Valor { get; set; }
        public string? NoExpediente { get; set; }
        public string? NoExpedienteGlobal { get; set; }
        public string? FechaGrabacion { get; set; }
        public string? FechaAsignacion { get; set; }
        public string? TipoFase { get; set; }
    }


}
