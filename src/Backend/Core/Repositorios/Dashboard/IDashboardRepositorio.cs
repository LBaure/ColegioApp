using Core.Models;
using Core.Models.Dashboard;
using Core.Models.Sso;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios.Dashboard
{
    public interface IDashboardRepositorio
    {
        Task<CantidadExpedientesPorFaseModelo> ObtenerExpedientesPorTipoExpediente(ExpedientesPorTipoModelo Filtros);
        Task<IEnumerable<CantidadExpedientesPorUsuarioModelo>> ObtenerUsuariosConExpedientes(ExpedientesPorTipoModelo Filtros);
        Task<List<ExpedientesConFaseModelo>> ObtenerExpedientesParaGrafico(ExpedientesPorTipoModelo Filtros);
        Task<List<ExpedientesConFaseModelo>> ObtenerExpedientesFinalizadosParaGrafico(ExpedientesPorTipoModelo Filtros);
        Task<dynamic> ObtenerExpedientesPorTipo(ExpedientesPorTipoModelo Filtros, int idCompania);
        Task<ResultadoHttpModelo> ObtenerEncabezadoExpediente(FiltrosEncabezadoModelo filtrosEncabezado, UsuarioSsoModelo usuario);
        Task<IEnumerable<FaseModelo>> ObtenerFasesPorTipoExpediente(string idTipoExpediente, int idCompania);
        Task<IEnumerable<FaseModelo>> ObtenerFasesExpedientesPorTipo(ExpedientesPorTipoModelo Filtros, int idCompania);
        Task<IEnumerable<int>> ObtenerEjerciciosPorTipoExpediente(string idTipoExpediente, int idCompania);
        Task<int> ObtenerCompania(string NitUsuario);

    }
}

