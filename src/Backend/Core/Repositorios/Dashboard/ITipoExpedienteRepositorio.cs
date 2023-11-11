using Core.Models;
using Core.Models.Dashboard;
using Core.Models.Seguridad;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios.Dashboard
{
    public interface ITipoExpedienteRepositorio
    {
        Task<IEnumerable<TipoExpedienteModelo>> ObtenerTiposExpedienteUsuario();
        Task<IEnumerable<FasesConExpedientesWorkFlowModelo>> ObtenerFasesTipoExpediente(ExpedientesPorTipoModelo filtrosModelo, int idCompania);
        Task<IEnumerable<ExpedientesWorkFlowModelo>> ObtenerExpedientesPorFase(ExpedientesPorTipoModelo filtrosModelo, int idCompania, string? IdFase);
    }
}
