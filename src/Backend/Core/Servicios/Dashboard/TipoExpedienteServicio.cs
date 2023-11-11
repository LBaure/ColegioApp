using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Dashboard;
using Core.Models.Sso;
using Core.Repositorios.Dashboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Dashboard
{
    public class TipoExpedienteServicio : ITipoExpedienteServicio
    {
        private readonly ITipoExpedienteRepositorio _tipoExpedienteRepositorio;
        private readonly IDashboardRepositorio _dashboardRepositorio;
        private readonly IUsuarioActualServicio _usuarioActual;
        public TipoExpedienteServicio(ITipoExpedienteRepositorio tipoExpedienteRepositorio, IDashboardRepositorio dashboardRepositorio, IUsuarioActualServicio usuarioActualServicio)
        {
            _tipoExpedienteRepositorio = tipoExpedienteRepositorio;
            _dashboardRepositorio = dashboardRepositorio;
            _usuarioActual = usuarioActualServicio;
        }

        public async Task<IEnumerable<TipoExpedienteModelo>> ObtenerTiposExpedienteUsuario()
        {
            return await _tipoExpedienteRepositorio.ObtenerTiposExpedienteUsuario();
        }
        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Luis Bautista    03/10/2023     Se creo para obtener los expedientes para el 
        ///                                 work-flow de los expedientes, obtiene los expe-
        ///                                 dientes agrupado por fases.
        /// </summary>
        /// <param name="filtrosModelo"></param>
        /// <returns></returns>
        public async Task<ResultadoHttpModelo> ObtenerExpedientesWorkFlow(ExpedientesPorTipoModelo filtrosModelo)
        {
            try
            {
                UsuarioSsoModelo usuario = _usuarioActual.Get();
                var idCompania = await _dashboardRepositorio.ObtenerCompania(usuario.Nit);

                var fases = await _tipoExpedienteRepositorio.ObtenerFasesTipoExpediente(filtrosModelo, idCompania);
                foreach (var fase in fases)
                {
                    var listaExpedientes = await _tipoExpedienteRepositorio.ObtenerExpedientesPorFase(filtrosModelo, idCompania, fase.IdFase);
                    fase.Expedientes = listaExpedientes.ToList();
                }

                ResultadoHttpModelo resultado = new ResultadoHttpModelo(EstadoSolicitudHttp.success);
                resultado.Titulo = "Fases por tipo de expediente";
                resultado.Mensaje = "Información obtenida exitosamente.";
                resultado.Resultado = fases;

                return resultado;
            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch
            {
                throw;
            }
        }
    }
}
