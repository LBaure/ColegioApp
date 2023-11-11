using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Dashboard;
using Core.Models.Sso;
using Core.Repositorios.Dashboard;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Servicios.Dashboard
{
    public class DashboardServicio : IDashboardServicio
    {
        private readonly IDashboardRepositorio _dashboardRepositorio;
        private readonly IUsuarioActualServicio _usuarioActual;
        public DashboardServicio(IDashboardRepositorio dashboardRepositorio, IUsuarioActualServicio usuarioActual)
        {
            _dashboardRepositorio = dashboardRepositorio;
            _usuarioActual = usuarioActual;
        }

        public async Task<ResultadoHttpModelo> ObtenerExpedientesPorTipo(ExpedientesPorTipoModelo Filtros)
         {
            try
            {
                UsuarioSsoModelo usuario = ValidarUsuario();
                var idCompania = await ObtenerCompania(usuario.Nit);
                var listaFases = await _dashboardRepositorio.ObtenerFasesExpedientesPorTipo(Filtros, idCompania);
                
                HeaderExpedientesPorTipoModelo primerColumna = new()
                {
                    Text = "No. Expediente",
                    Value = "NoExpediente"
                };
                List<HeaderExpedientesPorTipoModelo> headerList = new()
                {
                    primerColumna
                };

                foreach (var item in listaFases)
                {
                    HeaderExpedientesPorTipoModelo encabezado = new()
                    {
                        Text = item.NombreFase,
                        Value = item.IdFase
                    };
                    headerList.Add(encabezado);
                }

                var listaExpedientes = await _dashboardRepositorio.ObtenerExpedientesPorTipo(Filtros, idCompania);

                ResultadoHttpModelo resultado = new ResultadoHttpModelo(EstadoSolicitudHttp.success);
                resultado.Titulo = "Fases por tipo de expediente";
                resultado.Mensaje = "Información obtenida exitosamente.";
                resultado.Resultado = new
                {
                    header = headerList,
                    expedientes = listaExpedientes
                };

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

        public async Task<CantidadExpedientesPorFaseModelo> ObtenerExpedientesPorTipoExpediente(ExpedientesPorTipoModelo Filtros)
        {
            return await _dashboardRepositorio.ObtenerExpedientesPorTipoExpediente(Filtros);
        }

        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo López    09/10/2023     Se añaden las validaciones de fecha
        /// Romeo López    02/11/2023     Se creó la funcion calculoGrafico para trabajar de forma recursiva
        ///                               los intervalos que fueran necesarios para fechas de inicio y final
        ///                               con menor diferencia
        /// </summary>
        /// <param name="Filtros"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<ResultadoHttpModelo> ObtenerDatosDashboard(ExpedientesPorTipoModelo Filtros)
        {
            try
            { 
                DashboardModelo res = new DashboardModelo();

                /* Se realizan las consultas sobre cada uno de los segmentos del dashboard
                 * panel lateral (TOTAL DE EXPEDIENTES POR FASE) Y (EXPEDIENTES POR FASE) 
                */
                res.cantidadExpedientesPorFase = await _dashboardRepositorio.ObtenerExpedientesPorTipoExpediente(Filtros);
                //Panel inferior (CANTIDAD DE EXPEDIENTES PENDIENTES POR USUARIO)                
                res.cantidadExpedientesPorUsuarios = (List<CantidadExpedientesPorUsuarioModelo>)await _dashboardRepositorio.ObtenerUsuariosConExpedientes(Filtros);
                //EXP. GENERADOS GRAFICO                
                var expedientesGenerados = await _dashboardRepositorio.ObtenerExpedientesParaGrafico(Filtros);
                //EXP. FINALIZADOS GRAFICO
                var expedientesFinalizados = await _dashboardRepositorio.ObtenerExpedientesFinalizadosParaGrafico(Filtros);
                if (res.cantidadExpedientesPorFase.Total == 0)
                {
                    res.EjeX = new List<string>();
                    res.EjeYGenerados = new List<int>();
                    res.EjeYFinalizados = new List<int>();
                    res.FechaInicial = Filtros.FechaInicial;
                    res.FechaFinal = Filtros.FechaFinal;
                    ResultadoHttpModelo resultadoSinExpedientes = new(EstadoSolicitudHttp.warning)
                    {
                        Resultado = res,
                        Mensaje = "correcto",
                        Titulo = "Dashboard"
                    };
                    return resultadoSinExpedientes;
                }
                
                if (Filtros.EjercicioFiscal != null)
                {
                    Filtros.FechaInicial = $"01/01/{Filtros.EjercicioFiscal}";
                    Filtros.FechaFinal = $"31/12/{Filtros.EjercicioFiscal}";
                }
                
                //Se valida si la fecha inicial y final ingresan null
                if (Filtros.FechaInicial == null || Filtros.FechaFinal == null)
                {
                    var year = DateTime.Now.Year;
                    Filtros.FechaInicial = $"01/01/{year}";
                    Filtros.FechaFinal = $"31/12/{year}";                    
                }
                //inicialmente se trabajan 7 intervalos, si se necesita estos se reducen hasta 1 intervalo mínimo
                var result = calculoGrafico(7, Filtros, expedientesFinalizados, expedientesGenerados);
                res.EjeX = result.EjeX;
                res.EjeYGenerados = result.EjeYGenerados;
                res.EjeYFinalizados = result.EjeYFinalizados;
                res.FechaInicial = Filtros.FechaInicial;
                res.FechaFinal = Filtros.FechaFinal;

                ResultadoHttpModelo resultado = new(EstadoSolicitudHttp.success)
                {
                    Resultado = res,
                    Mensaje = "correcto",
                    Titulo = "Dashboard"
                };
                return resultado;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Función que realiza el cálculo de los intervalos a tomar en cuenta, definición de dominios con el cálculo generado
        /// y evaluación de intervalos
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// </summary>
        /// <param name="intervaloActual">cantidad de intervalos a utilizar para el cálculo</param>
        /// <param name="Filtros">Filtros que ingresa el usuario</param>
        /// <param name="expedientesFinalizados">Listado de expedientes finalizados para las fechas ingresadas</param>
        /// <param name="expedientesGenerados">Listado de expedientes generados para las fechas ingresadas</param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        private DashboardModelo calculoGrafico(int intervaloActual, ExpedientesPorTipoModelo Filtros, List<ExpedientesConFaseModelo> expedientesFinalizados, List<ExpedientesConFaseModelo> expedientesGenerados)
        {
            DashboardModelo res = new DashboardModelo();
            //PENDIENTES
            DateTime fechaInicial = DateTime.ParseExact(Filtros.FechaInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fechaFinal = DateTime.ParseExact(Filtros.FechaFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            TimeSpan diferencia = fechaFinal - fechaInicial;
            
            int INTERVALOS = intervaloActual;
            decimal amplSinAproximar = (diferencia.Days + 1) / INTERVALOS;
            
            if (amplSinAproximar < 1)
            {
                res = calculoGrafico(INTERVALOS - 1, Filtros, expedientesFinalizados, expedientesGenerados);

            }
            else
            {
                int amplitudDeIntervalo = (int)Math.Round(amplSinAproximar, MidpointRounding.AwayFromZero);
                
                //Se definen los listados de los resultados
                List<int> listaTotales = new List<int>();//Totales generados
                List<int> listaTotales2 = new List<int>();//Totales finalizados
                List<string> listaDescripciones = new List<string>();//Intervalos

                //Asignación de los intervalos al listado
                listaDescripciones = definirDominios(fechaInicial, fechaFinal, INTERVALOS, amplitudDeIntervalo);

                //se recorre el listado de intervalos para realizar los calculos
                foreach (var intervalo in listaDescripciones)
                {
                    Console.WriteLine($"intervalo: {intervalo}");
                    var listaFechas = intervalo.Split("-");


                    DateTime paramInicial = DateTime.ParseExact(listaFechas[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime paramFinal = DateTime.ParseExact(listaFechas[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    //se realizan las asignaciones de los expedientes y en cada iteracion se descartan los expedientes ya evaluados
                    expedientesGenerados = busquedaExpedientes(paramInicial, paramFinal, expedientesGenerados, true, intervalo == listaDescripciones[listaDescripciones.Count-1]  ? true : false, ref listaTotales);
                    expedientesFinalizados = busquedaExpedientes(paramInicial, paramFinal, expedientesFinalizados, false, intervalo == listaDescripciones[listaDescripciones.Count-1] ? true : false, ref listaTotales2);
                }
                res.EjeX = definirFormatoFinalDominios(listaDescripciones);
                //res.EjeX = listaDescripciones;
                res.EjeYGenerados = listaTotales;
                res.EjeYFinalizados = listaTotales2;
                res.FechaInicial = Filtros.FechaInicial;
                res.FechaFinal = Filtros.FechaFinal;
            }
            
            
            return res;

        }

        /// <summary>
        /// Función para establecer los intervalos del rango de fecha completo
        /// Solamente devuelve un array de strings con los dominios inicial y final en cada elemento
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo López     02/11/2023      Se modificaron los dominios iniciales desde el segundo intervalo
        ///                                 para tomar el día después del dominio final del intervalo anterior
        /// </summary>
        /// <param name="inicio">Fecha de inicio de la consulta de expedientes</param>
        /// <param name="final">Fecha final de la consulta de expedientes</param>
        /// <param name="INTERVALOS">Cantidad de intervalos que se requieren para el grafico (7 intervalos)</param>
        /// <param name="amplitud">Cantidad de valores tomados en cuenta para los intervalos</param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        private List<string> definirDominios(DateTime inicio, DateTime final, int INTERVALOS, int amplitud)
        {
            List<string> listaDominios = new List<string>();
            DateTime actual = inicio;
            DateTime inicioDominio = inicio;
            int count = 1;
            while (actual != final)
            {
                if (count == amplitud)
                {
                    if (listaDominios.Count != (INTERVALOS -1))
                    {
                        listaDominios.Add(inicioDominio.ToString("dd/MM/yyyy") + " - " + actual.ToString("dd/MM/yyyy"));
                        inicioDominio = actual.AddDays(1);
                        count = 1;
                    }
                }
                else
                {
                    count++;
                }
                actual = actual.AddDays(1);


            }
            listaDominios.Add(inicioDominio.ToString("dd/MM/yyyy") + " - " + final.ToString("dd/MM/yyyy"));
            return listaDominios;

        }

        /// <summary>
        /// Función para resumir los intervalos de fecha
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// </summary>
        /// <param name="listado">Listado de intervalos de fecha utilizados para el cálculo del gráfico</param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        private List<string> definirFormatoFinalDominios(List<string> listado)
        {
            List<string> res = new List<string> { };
            foreach (var item in listado)
            {
                var listaFechas = item.Split(" - ");
                DateTime paramInicial = DateTime.ParseExact(listaFechas[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime paramFinal = DateTime.ParseExact(listaFechas[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (paramInicial == paramFinal)
                {
                    res.Add(paramInicial.ToString("dd/MM/yyyy"));
                }
                else
                {
                    res.Add(paramInicial.ToString("dd/MM/yyyy") + " - " + paramFinal.ToString("dd/MM/yyyy"));
                }

            }
            return res;

        }


        /// <summary>
        /// Función para establecer los intervalos del rango de fecha completo
        /// Solamente devuelve un array de strings con los dominios inicial y final en cada elemento
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo López      02/11/2023     Se resumieron las condiciones para sumar los expedientes a un intervalo
        ///                                 debido a que el dominio inicial ya no toma en cuenta el día del dominio 
        ///                                 final anterior
        /// </summary>
        /// <param name="inicio">Fecha de inicio del intervalo actual que se está evaluando</param>
        /// <param name="fin">Fecha final del intervalo actual que se está evaluando</param>
        /// <param name="listaExpedientes">Arreglo de ExpedientesConFaseModleo sobre los cuales se realizará la búsqueda</param>
        /// <param name="tipoExpedientes">True realiza la búsqueda sobre FECHA_GRABACION y false sobre el campo FECHA_ASIGNACION</param>
        /// <param name="ultimo">True si es el último intervalo, false para todos los demas</param>
        /// <param name="listaTotal">Lista de totales sobre el intérvalo que se está evaluando en inicio y fin, se maneja como referencia porque no retorna el valor</param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        private List<ExpedientesConFaseModelo> busquedaExpedientes(DateTime inicio, DateTime fin, List<ExpedientesConFaseModelo> listaExpedientes, bool tipoExpedientes, bool ultimo, ref List<int> listaTotal)
        {
            List<int> resultados = new List<int>();
            List<ExpedientesConFaseModelo> listaAEliminar = new List<ExpedientesConFaseModelo>();
            int total = 0;
            if (listaExpedientes.Count() > 0)
            {
                for (int i = 0; i <= listaExpedientes.Count() - 1; i++)
                {

                    if (tipoExpedientes == true)//SE TOMA EN CUENTA FECHA_GRABACION
                    {

                        var fechaGrabacion = DateTime.ParseExact(listaExpedientes[i].FechaGrabacion, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if ((fechaGrabacion >= inicio) && (fechaGrabacion <= fin))
                        {
                            total += 1;
                            listaAEliminar.Add(listaExpedientes[i]);
                        }

                    }
                    else//SE TOMA EN CUENTA FECHA_ASIGNACION
                    {

                        var FechaAsignacion = DateTime.ParseExact(listaExpedientes[i].FechaAsignacion, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if ((FechaAsignacion >= inicio) && (FechaAsignacion <= fin))
                        {
                            total += 1;
                            listaAEliminar.Add(listaExpedientes[i]);
                        }

                    }
                }
            }

            listaTotal.Add(total);
            foreach (var item in listaAEliminar)
            {
                listaExpedientes.Remove(listaExpedientes.Find(e => e == item));
            }
            return listaExpedientes;
        }

        public async Task<ResultadoHttpModelo> ObtenerEncabezadoExpediente(FiltrosEncabezadoModelo filtrosEncabezado)
        {

            UsuarioSsoModelo usuario = ValidarUsuario();

            return await _dashboardRepositorio.ObtenerEncabezadoExpediente(filtrosEncabezado, usuario);
        }

        public UsuarioSsoModelo ValidarUsuario()
        {
            var usuarioActual = _usuarioActual.Get();

            if (usuarioActual == null || usuarioActual.Nit == null)
            {
                throw new ResponseException("Usuario no autenticado", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.Unauthorized);
            }
            return usuarioActual;
        }


        public async Task<ResultadoHttpModelo> ObtenerFasesPorTipoExpediente(string IdTipoExpediente)
        {
            try
            {
                UsuarioSsoModelo usuario = ValidarUsuario();
                var idCompania = await ObtenerCompania(usuario.Nit);
                var listado = await _dashboardRepositorio.ObtenerFasesPorTipoExpediente(IdTipoExpediente, idCompania);
                var ejercicios = await _dashboardRepositorio.ObtenerEjerciciosPorTipoExpediente(IdTipoExpediente, idCompania);

                ResultadoHttpModelo resultado = new ResultadoHttpModelo(EstadoSolicitudHttp.success);
                resultado.Titulo = "Fases por tipo de expediente";
                resultado.Mensaje = "Información obtenida exitosamente.";
                resultado.Resultado = new
                {
                    listaFases = listado,
                    listaEjercicios = ejercicios
                };

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

        public async Task<int> ObtenerCompania(string nitUsuario)
        {
            return await _dashboardRepositorio.ObtenerCompania(nitUsuario);
        }


    }
}
