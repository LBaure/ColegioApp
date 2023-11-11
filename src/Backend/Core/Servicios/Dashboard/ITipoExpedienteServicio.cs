using Core.Models;
using Core.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Dashboard
{
    public interface ITipoExpedienteServicio
    {
        Task<IEnumerable<TipoExpedienteModelo>> ObtenerTiposExpedienteUsuario();
        Task<ResultadoHttpModelo> ObtenerExpedientesWorkFlow(ExpedientesPorTipoModelo filtrosModelo);
    }
}
