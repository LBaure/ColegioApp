using Core.Models;
using Core.Models.Dashboard;

namespace Core.Servicios.Dashboard
{
    public interface IDashboardServicio
    {
        Task<CantidadExpedientesPorFaseModelo> ObtenerExpedientesPorTipoExpediente(ExpedientesPorTipoModelo Filtros);
        Task<ResultadoHttpModelo> ObtenerDatosDashboard(ExpedientesPorTipoModelo Filtros);
        Task<ResultadoHttpModelo> ObtenerExpedientesPorTipo(ExpedientesPorTipoModelo Filtros);
        Task<ResultadoHttpModelo> ObtenerEncabezadoExpediente(FiltrosEncabezadoModelo filtrosEncabezado);
        Task<ResultadoHttpModelo> ObtenerFasesPorTipoExpediente(string IdTipoExpediente);
    }
}
