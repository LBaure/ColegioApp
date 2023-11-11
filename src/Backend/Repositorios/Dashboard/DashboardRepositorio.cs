using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Dashboard;
using Core.Models.Sso;
using Core.Repositorios;
using Core.Repositorios.Dashboard;
using Core.Servicios;
using Dapper;
using Exceptionless;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Repositorios.Dashboard
{
    public class DashboardRepositorio : IDashboardRepositorio
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly IUsuarioActualServicio _usuarioActual;

        public DashboardRepositorio(IConnectionProvider connectionProvider, IUsuarioActualServicio usuarioActualServicio)
        {
            _connectionProvider = connectionProvider;
            _usuarioActual = usuarioActualServicio;
        }

        public async Task<dynamic> ObtenerExpedientesPorTipo(ExpedientesPorTipoModelo filtros, int idCompania)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try
            {                
                // Se utiliza para la tabla pivote
                var fasesString = string.Empty;
                // Validacion: si el parametro de filtros viene nullo se filtran todas las fases del tipo de expediente.
                if (filtros.FasesMostrar == null || filtros.FasesMostrar.Length == 0)
                {
                    var execFunction = "SELECT FN_FASES_TIPO_EXPEDIENTE('" + filtros.IdTipoExpediente + "', "+ idCompania +" ) FROM DUAL";
                    fasesString = await conn.QueryFirstOrDefaultAsync<string>(execFunction);
                }
                // Validacion: si el parametro de filtros trae informacion se toman las fases y se filtran
                else
                {
                    var fases = filtros.FasesMostrar;
                    var tamanio = fases.Length - 1;
                    // Se le agregan comillas simples a cada item del array, ademas se concatena  a la variable string
                    // que servira para la tabla pivote
                    for(var indice = 0; indice < filtros.FasesMostrar.Length; indice++)
                    {
                        fasesString = fasesString + "'" + filtros.FasesMostrar[indice] + "'";
                        if (indice < tamanio)
                        {
                            fasesString += "," ;
                        }
                    }
                }

                #region Construccion del Query, expedientes para el dashboard, filtrados o no.

                var paramExpedientes = new DynamicParameters();
                paramExpedientes.Add("@IdTipoExpediente", filtros.IdTipoExpediente);

                string sqlExpedientesDashboard =
                "SELECT * FROM ( \n" +
                    "SELECT \n" +
                    "    EXPEJEEXPE.ID_EJERCICIO || '-' || EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL NO_EXPEDIENTE_MINFIN, \n" +
                    "    EXPEXPE.NO_EXPEDIENTE, \n" +
                    "    TRUNC((SYSDATE - EXPEXPE.FECHA_GRABACION))|| ' días' TIEMPO_TRANSCURRIDO, \n" +
                    "    EXPEXPE.ID_TIPO_EXPEDIENTE, \n" +
                    "    EXPEXPE.DESCRIPCION, \n" +
                    "    EXPEXPE.ID_FASE, \n" +
                    "    DECODE(EXPF.TIPO_FASE, 'F', NULL, GENU.ID_USUARIO || ' - ' || GENU.DESCRIPCION) USUARIO_ASIGNADO, \n" +
                    "    GENU.EXTENSION, \n" +
                    "    GENU.ID_UNIDAD_ADMINISTRATIVA \n" +
                    "FROM \n" +
                    "	EXP_TIPOS_EXPEDIENTE EXPTEXP, \n" +
                    "	EXP_EXPEDIENTES EXPEXPE, \n" +
                    "	EXP_EJERCICIO_EXPEDIENTES EXPEJEEXPE, \n" +
                    "	EXP_FASES EXPF, \n" +
                    "	GEN_USUARIOS GENU, \n" +
                    "	EXP_ORIGENES EXPORI, \n" +
                    "	PLA_UNIDADES_ADMINISTRATIVAS PLA, \n" +
                    "	EXP_USUARIOS_FASE_EXPEDIENTE EXPUFE \n" +
                    "WHERE \n" +
                    "	EXPEXPE.CORRELATIVO_ASIGNACION = EXPUFE.CORRELATIVO \n" +
                    "AND \n" +
                    "    EXPF.TIPO_FASE <> 'F' \n" +
                    "AND \n" +
                    "    EXPUFE.ID_USUARIO_ASIGNADO = EXPEXPE.ID_USUARIO_ASIGNADO \n" +
                    "AND \n" +
                    "    EXPUFE.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    EXPUFE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE \n" +
                    "AND \n" +
                    "    EXPUFE.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
                    "AND \n" +
                    "    PLA.ID_UNIDAD_ADMINISTRATIVA = EXPEXPE.ID_UNIDAD_ADMINISTRATIVA \n" +
                    "AND \n" +
                    "    PLA.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    EXPORI.ID_ORIGEN = EXPEXPE.ID_ORIGEN \n" +
                    "AND \n" +
                    "    EXPORI.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    GENU.ID_USUARIO = EXPEXPE.ID_USUARIO_ASIGNADO \n" +
                    "AND \n" +
                    "    GENU.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    EXPF.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    EXPF.ID_FASE = EXPEXPE.ID_FASE \n" +
                    "AND \n" +
                    "    EXPEJEEXPE.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    EXPEJEEXPE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE \n" +
                    "AND \n" +
                    "    EXPEJEEXPE.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
                    "AND \n" +
                    "    EXPTEXP.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
                    "AND \n" +
                    "    EXPTEXP.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
                    "AND \n" +
                    "    EXPEXPE.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n";


                // *************** AREA DE CONDICIONES EN EL QUERY STRING ***************

                // *********** SE FILTRARAN LOS EXPEDIENTES CON LAS FASES QUE VENGAN SELECCIONADAS ***********
                if (filtros.FasesMostrar?.Length > 0)
                {
                    sqlExpedientesDashboard +=
                        "AND  \n" +
                        "	EXPEXPE.ID_FASE IN(" + fasesString +") \n";
                }


                if (filtros.NoExpediente != null)
                {
                    paramExpedientes.Add("@Ejercicio", filtros.Ejercicio);
                    paramExpedientes.Add("@NoExpediente", filtros.NoExpediente);
                    sqlExpedientesDashboard +=
                        "AND  \n" +
                        "	EXPEJEEXPE.ID_EJERCICIO = :Ejercicio \n" +
                        "AND  \n" +
                        "	EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL = :NoExpediente \n";
                }

                // *************** SE VALIDA EN LOS PARAMETROS SI EXISTE UN FILTRO CON FECHAS ***************
                // *************** DE LO CONTRARIO SE COLOCARA EL EJERCICIO ACTUAL            *************** 
                if (filtros.FechaInicial != null)
                {
                    paramExpedientes.Add("@fechaInicial", filtros.FechaInicial);
                    paramExpedientes.Add("@fechaFinal", filtros.FechaFinal);
                    sqlExpedientesDashboard +=
                        "AND  \n" +
                        "	TRUNC(EXPEXPE.FECHA_GRABACION) >= TO_DATE(:fechaInicial, 'DD/MM/YYYY' ) \n" +
                        "AND  \n" +
                        "	TRUNC(EXPEXPE.FECHA_GRABACION) <= TO_DATE(:fechaFinal, 'DD/MM/YYYY') \n";
                } 
                else if (filtros.EjercicioFiscal != null)
                {
                    paramExpedientes.Add("@IdEjercicio", filtros.EjercicioFiscal);
                    sqlExpedientesDashboard +=
                        "AND \n" +
                        "    EXPEJEEXPE.ID_EJERCICIO = :IdEjercicio \n";
                }
                else
                {
                    var year = DateTime.Now.Year;
                    paramExpedientes.Add("@ejercicioActual", year);
                    sqlExpedientesDashboard +=
                        "AND \n" +
                        "    TO_CHAR(EXPEXPE.FECHA_GRABACION, 'YYYY') = :ejercicioActual \n";
                }


                sqlExpedientesDashboard +=
                    ") \n" +
                    "PIVOT (    \n" +
                    "    COUNT(*)  \n" +
                    "    FOR ID_FASE IN(" + fasesString + ") \n" +
                    ") \n" +
                    "ORDER BY \n" +
                        " TIEMPO_TRANSCURRIDO DESC ";


                #endregion


                var list = await conn.QueryAsync(sqlExpedientesDashboard, paramExpedientes);
                return list;


            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private UsuarioSsoModelo ValidarUsuario()
        {
            var usuarioActual = _usuarioActual.Get();

            if (usuarioActual == null || usuarioActual.Nit == null)
            {
                throw new ResponseException("Usuario no autenticado", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.Unauthorized);
            }
            return usuarioActual;
        }

        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo Lopez    09/10/2023     Se omite la busqueda de los ultimos expedientes
        ///                                 por mes y se añaden las validaciones de fecha
        /// </summary>
        /// <param name="Filtros"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<CantidadExpedientesPorFaseModelo> ObtenerExpedientesPorTipoExpediente(ExpedientesPorTipoModelo Filtros)
        {
            var param = new DynamicParameters();
            string sqlSelect =

              "WITH FASEs AS ( " +
              "SELECT  " +
              "DISTINCT(EXPGRPCN.ID_FASE) ID_FASE,   " +
              "EXPF.DESCRIPCION,  " +
              "EXPGRPCN.ID_TIPO_EXPEDIENTE,   " +
              "EXPF.ID_COMPANIA " +
              "FROM EXP_GRUPO_CONSULTA_FASE EXPGRPCN,   " +
              "EXP_FASES EXPF " +
              "WHERE EXPF.ID_FASE = EXPGRPCN.ID_FASE " +
              "AND EXPF.ID_COMPANIA = EXPGRPCN.ID_COMPANIA " +
              "AND EXPGRPCN.ID_TIPO_EXPEDIENTE = :idTipoExpediente " +
              "AND EXPGRPCN.ID_FASE NOT LIKE '%ARC%' " +
              "AND EXPGRPCN.ID_COMPANIA = 1 " +
              "ORDER BY EXPGRPCN.ID_FASE ASC " +
              ") " +
              "SELECT " +
              "f.id_tipo_expediente idTipoExpediente, " +
              "f.id_fase IdFase, " +
              "f.descripcion Descripcion, " +
              "COUNT(ee.NO_EXPEDIENTE) Total " +
              "FROM  FASES f " +
              "LEFT JOIN EXP_EXPEDIENTES ee " +
              "ON f.ID_FASE = ee.ID_FASE " +
              "AND f.ID_TIPO_EXPEDIENTE = ee.ID_TIPO_EXPEDIENTE ";
                
               


            // *************** SE VALIDA EN LOS PARAMETROS SI EXISTE UN FILTRO CON FECHAS ***************
            // *************** DE LO CONTRARIO SE COLOCARA EL EJERCICIO ACTUAL            *************** 
            if (Filtros.FechaFinal != null)
            {
                param.Add("@fechaInicio", Filtros.FechaInicial);
                param.Add("@fechaFinal", Filtros.FechaFinal);

                sqlSelect += "and TO_DATE(ee.FECHA_GRABACION, 'dd/mm/yy') >= TO_DATE(:fechaInicio, 'dd/mm/yy') " +
                            "and TO_DATE(ee.FECHA_GRABACION, 'dd/mm/yy') <= TO_DATE(:fechaFinal, 'dd/mm/yy') ";
            }else if(Filtros.EjercicioFiscal != null)
            {
                param.Add("@ejercicioFiscal", Filtros.EjercicioFiscal);

                sqlSelect += " AND TO_CHAR(ee.FECHA_GRABACION, 'YYYY') = :ejercicioFiscal \n ";
            }
            else
            {
                var year = DateTime.Now.Year;
                param.Add("@ejercicioActual", year);
                sqlSelect +=
                    "AND \n" +
                    "    TO_CHAR(ee.FECHA_GRABACION, 'YYYY') = :ejercicioActual \n";
            }

            sqlSelect += "GROUP BY f.ID_FASE, f.DESCRIPCION, f.id_tipo_expediente " +
                "ORDER BY\r\n\t" +
                "COUNT(ee.NO_EXPEDIENTE) DESC";

            //string sqlSelectUltimoMes =

            //    "WITH FASEs AS ( " +
            //    "SELECT  " +
            //    "DISTINCT(EXPGRPCN.ID_FASE) ID_FASE,   " +
            //    "EXPF.DESCRIPCION,  " +
            //    "EXPGRPCN.ID_TIPO_EXPEDIENTE,   " +
            //    "EXPF.ID_COMPANIA " +
            //    "FROM EXP_GRUPO_CONSULTA_FASE EXPGRPCN,   " +
            //    "EXP_FASES EXPF " +
            //    "WHERE EXPF.ID_FASE = EXPGRPCN.ID_FASE " +
            //    "AND EXPF.ID_COMPANIA = EXPGRPCN.ID_COMPANIA " +
            //    "AND EXPGRPCN.ID_TIPO_EXPEDIENTE = :idTipoExpediente " +
            //    "AND EXPGRPCN.ID_FASE NOT LIKE '%ARC%' " +
            //    "AND EXPGRPCN.ID_COMPANIA = 1 " +
            //    "ORDER BY EXPGRPCN.ID_FASE ASC " +
            //    ") " +
            //    "SELECT " +
            //    "f.id_tipo_expediente idTipoExpediente, " +
            //    "f.id_fase IdFase, " +
            //    "f.descripcion Descripcion, " +
            //    "COUNT(ee.NO_EXPEDIENTE) Total " +
            //    "FROM  FASES f " +
            //    "LEFT JOIN EXP_EXPEDIENTES ee " +
            //    "ON f.ID_FASE = ee.ID_FASE " +
            //    "AND f.ID_TIPO_EXPEDIENTE = ee.ID_TIPO_EXPEDIENTE " +
            //    "and TO_DATE(ee.FECHA_GRABACION, 'dd/mm/yy') >= TO_DATE(:fechaInicio, 'dd/mm/yy') " +
            //    "and TO_DATE(ee.FECHA_GRABACION, 'dd/mm/yy') <= TO_DATE(:fechaFinal, 'dd/mm/yy') " +
            //    "AND to_char(to_date(ee.FECHA_ASIGNACION, 'dd/mm/yyyy'), 'mm') = to_char(to_date(:fechaFinal, 'dd/mm/yyyy'), 'mm') " +
            //    "GROUP BY f.ID_FASE, f.DESCRIPCION, f.id_tipo_expediente " +
            //    "ORDER BY  f.ID_FASE";


            using var conn = await _connectionProvider.OpenAsync();

            try
            {
                UsuarioSsoModelo usuario = ValidarUsuario();
                
                param.Add("@IdUsuario", usuario.Nit);
                //param.Add("@IdUsuario", "72176504");

                var IdCompania = await conn.QueryFirstOrDefaultAsync<int>(sqlSelectInfoUsuario, param);

                if (IdCompania < 1)
                {
                    throw new ResponseException("Usuario no asignado a una compañia.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }

                param.Add("@idTipoExpediente", Filtros.IdTipoExpediente);
                var lista = await conn.QueryAsync<FaseExpedienteModelo>(sqlSelect, param);
                //var listaUltimoMes = await conn.QueryAsync<FaseExpedienteModelo>(sqlSelectUltimoMes, param);
                //var FasesUltimoMes = listaUltimoMes.AsList();
                conn.Close();

                CantidadExpedientesPorFaseModelo res = new CantidadExpedientesPorFaseModelo();
                res.Total = 0;
                int i = 0;
                foreach (var item in lista)
                {
                    //item.UltimoMes = FasesUltimoMes.Find(e => e.IdFase == item.IdFase).Total;
                    res.Total += item.Total;
                    i++;
                }
                res.IdTipoExpediente = Filtros.IdTipoExpediente;
                res.Fases = lista.AsList();
                return res;

            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo Lopez    09/10/2023     Se añaden las validaciones de fecha
        /// </summary>
        /// <param name="Filtros"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<IEnumerable<CantidadExpedientesPorUsuarioModelo>> ObtenerUsuariosConExpedientes(ExpedientesPorTipoModelo Filtros)
        {
            var param = new DynamicParameters();
            string sqlSelect =
                "WITH USUARIOS AS ( " +
                "SELECT  " +
                "DISTINCT(EUFTE.ID_USUARIO) ID_USUARIO, " +
                "GU.DESCRIPCION, " +
                "GU.ID_UNIDAD_ADMINISTRATIVA, " +
                "GU.EXTENSION, " +
                "EUFTE.ID_TIPO_EXPEDIENTE, " +
                "EUFTE.ID_COMPANIA " +
                "FROM " +
                "EXP_USUARIOS_FASE_TIPO_EXPE EUFTE, GEN_USUARIOS GU " +
                " WHERE " +
                "GU.ID_COMPANIA = EUFTE.ID_COMPANIA " +
                "AND GU.ID_USUARIO = EUFTE.ID_USUARIO " +
                "AND EUFTE.ID_TIPO_EXPEDIENTE = :IdTipoExpediente " +
                "AND EUFTE.ID_COMPANIA = 1\r\n" +
                "ORDER BY EUFTE.ID_USUARIO)\r\n" +
                "SELECT\r\n\t" +
                "f.id_tipo_expediente idTipoExpediente,\r\n\t" +
                "f.ID_USUARIO IdUsuario,\r\n\t" +
                "f.ID_UNIDAD_ADMINISTRATIVA IdUnidadAdministrativa,\r\n\t" +
                "f.EXTENSION Extension,\r\n\t" +
                "f.DESCRIPCION Descripcion,\r\n\t" +
                "COUNT(ee.NO_EXPEDIENTE) Total\r\n" +
                "FROM\r\n\t" +
                "USUARIOS f\r\n" +
                "LEFT JOIN EXP_EXPEDIENTES ee ON\r\n\t" +
                "f.ID_USUARIO = ee.ID_USUARIO_ASIGNADO \r\n\t" +
                "AND f.ID_TIPO_EXPEDIENTE = ee.ID_TIPO_EXPEDIENTE\r\n\t" +
                "AND ee.ID_FASE NOT LIKE '%ARC%'\r\n\t";
                
                

            // *************** SE VALIDA EN LOS PARAMETROS SI EXISTE UN FILTRO CON FECHAS ***************
            // *************** DE LO CONTRARIO SE COLOCARA EL EJERCICIO ACTUAL            *************** 
            if (Filtros.FechaFinal != null)
            {
                param.Add("@fechaInicio", Filtros.FechaInicial);
                param.Add("@fechaFinal", Filtros.FechaFinal);

                sqlSelect += "and TO_DATE(ee.FECHA_GRABACION, 'dd/mm/yy') >= TO_DATE(:fechaInicio, 'dd/mm/yy') " +
                "and TO_DATE(ee.FECHA_GRABACION, 'dd/mm/yy') <= TO_DATE(:fechaFinal, 'dd/mm/yy') ";
            }
            else if (Filtros.EjercicioFiscal != null)
            {
                param.Add("@ejercicioFiscal", Filtros.EjercicioFiscal);

                sqlSelect += " AND TO_CHAR(ee.FECHA_GRABACION, 'YYYY') = :ejercicioFiscal \n ";
            }
            else
            {
                var year = DateTime.Now.Year;
                param.Add("@ejercicioActual", year);
                sqlSelect +=
                    "AND \n" +
                    "    TO_CHAR(ee.FECHA_GRABACION, 'YYYY') = :ejercicioActual \n";
            }

            sqlSelect += "GROUP BY\r\n\t" +
                "f.ID_USUARIO,\r\n\t" +
                "f.DESCRIPCION,\r\n\t" +
                "f.id_tipo_expediente,\r\n" +
                "f.EXTENSION,\r\n\t" +
                "f.ID_UNIDAD_ADMINISTRATIVA\r\n" +
                "ORDER BY\r\n\t" +
                "COUNT(ee.NO_EXPEDIENTE) DESC";





            string sqlSelectInfoUsuario =
                "SELECT ID_COMPANIA  " +
                "FROM GEN_USUARIOS gu " +
                "WHERE gu.ID_USUARIO  = :IdUsuario";

            using var conn = await _connectionProvider.OpenAsync();

            try
            {
                UsuarioSsoModelo usuario = ValidarUsuario();
                
                param.Add("@IdUsuario", usuario.Nit);
                //param.Add("@IdUsuario", "72176504");

                var IdCompania = await conn.QueryFirstOrDefaultAsync<int>(sqlSelectInfoUsuario, param);

                if (IdCompania < 1)
                {
                    throw new ResponseException("Usuario no asignado a una compañia.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }

                param.Add("@idTipoExpediente", Filtros.IdTipoExpediente);
                var lista = await conn.QueryAsync<CantidadExpedientesPorUsuarioModelo>(sqlSelect, param);

                return lista;

            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }


        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo Lopez    09/10/2023     Se añaden las validaciones de fecha
        /// </summary>
        /// <param name="Filtros"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<List<ExpedientesConFaseModelo>> ObtenerExpedientesParaGrafico(ExpedientesPorTipoModelo Filtros)
        {
            var param = new DynamicParameters();
            string sqlSelect =
                "    SELECT \r\n " +
                "    EXPEJEEXPE.ID_EJERCICIO||'-'||EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL NoExpediente,\r\n " +
                "    EXPEXPE.NO_EXPEDIENTE NoExpedienteGlobal,\r\n " +
                "    TO_CHAR(TO_DATE(EXPEXPE.FECHA_GRABACION , 'dd/mm/yy'), 'dd/mm/yyyy') FechaGrabacion,\r\n " +
                "    TO_CHAR(TO_DATE(EXPEXPE.FECHA_ASIGNACION , 'dd/mm/yy'), 'dd/mm/yyyy') FechaAsignacion,\r\n " +
                "    EXPF.TIPO_FASE TipoFase\r\n " +
                "    FROM \r\n " +
                "    EXP_EXPEDIENTES EXPEXPE,\r\n " +
                "    EXP_EJERCICIO_EXPEDIENTES EXPEJEEXPE,\r\n " +
                "    EXP_FASES EXPF\r\n " +
                "    WHERE\r\n " +
                "    EXPF.ID_COMPANIA = EXPEXPE.ID_COMPANIA\r\n " +
                "    AND\r\n " +
                "    EXPF.ID_FASE = EXPEXPE.ID_FASE\r\n " +
                "    AND\r\n " +
                "    EXPEJEEXPE.ID_COMPANIA = EXPEXPE.ID_COMPANIA\r\n " +
                "    AND\r\n " +
                "    EXPEJEEXPE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE\r\n " +
                "    AND\r\n " +
                "    EXPEJEEXPE.ID_TIPO_EXPEDIENTE =EXPEXPE.ID_TIPO_EXPEDIENTE\r\n " +
                "    AND\r\n " +
                "    EXPEXPE.ID_TIPO_EXPEDIENTE = :idTipoExpediente\r\n " +
                "    AND\r\n " +
                "    EXPEXPE.ID_COMPANIA = 1\r\n ";
               
                


            // *************** SE VALIDA EN LOS PARAMETROS SI EXISTE UN FILTRO CON FECHAS ***************
            // *************** DE LO CONTRARIO SE COLOCARA EL EJERCICIO ACTUAL            *************** 
            if (Filtros.FechaFinal != null)
            {
                param.Add("@fechaInicio", Filtros.FechaInicial);
                param.Add("@fechaFinal", Filtros.FechaFinal);

                sqlSelect += "    and TO_DATE(EXPEXPE.FECHA_GRABACION, 'dd/mm/yy') >= TO_DATE(:fechaInicio, 'dd/mm/yy') " +
                "    and TO_DATE(EXPEXPE.FECHA_GRABACION, 'dd/mm/yy') <= TO_DATE(:fechaFinal, 'dd/mm/yy') ";
            }
            else if (Filtros.EjercicioFiscal != null)
            {
                param.Add("@ejercicioFiscal", Filtros.EjercicioFiscal);

                sqlSelect += " AND TO_CHAR(EXPEXPE.FECHA_GRABACION, 'YYYY') = :ejercicioFiscal \n ";
            }
            else
            {
                var year = DateTime.Now.Year;
                param.Add("@ejercicioActual", year);
                sqlSelect +=
                    "AND \n" +
                    "    TO_CHAR(EXPEXPE.FECHA_GRABACION, 'YYYY') = :ejercicioActual \n";
            }

            sqlSelect += "    ORDER BY EXPEXPE.FECHA_GRABACION ASC";





            string sqlSelectInfoUsuario =
                "SELECT ID_COMPANIA  " +
                "FROM GEN_USUARIOS gu " +
                "WHERE gu.ID_USUARIO  = :IdUsuario";

            using var conn = await _connectionProvider.OpenAsync();

            try
            {
                UsuarioSsoModelo usuario = ValidarUsuario();
                
                param.Add("@IdUsuario", usuario.Nit);
                var IdCompania = await conn.QueryFirstOrDefaultAsync<int>(sqlSelectInfoUsuario, param);

                if (IdCompania < 1)
                {
                    throw new ResponseException("Usuario no asignado a una compañia.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }

                param.Add("@idTipoExpediente", Filtros.IdTipoExpediente);
                var lista = await conn.QueryAsync<ExpedientesConFaseModelo>(sqlSelect, param);

                return lista.AsList();

            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Romeo Lopez    09/10/2023     Se añaden las validaciones de fecha
        /// </summary>
        /// <param name="Filtros"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<List<ExpedientesConFaseModelo>> ObtenerExpedientesFinalizadosParaGrafico(ExpedientesPorTipoModelo Filtros)
        {
            var param = new DynamicParameters();
            string sqlSelect =
                "    SELECT \r\n " +
                "    EXPEJEEXPE.ID_EJERCICIO||'-'||EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL NoExpediente,\r\n " +
                "    EXPEXPE.NO_EXPEDIENTE NoExpedienteGlobal,\r\n " +
               "    TO_CHAR(TO_DATE(EXPEXPE.FECHA_ASIGNACION , 'dd/mm/yy'), 'dd/mm/yyyy') FechaAsignacion,\r\n " +
                "    EXPF.TIPO_FASE TipoFase\r\n " +
                "    FROM \r\n " +
                "    EXP_EXPEDIENTES EXPEXPE,\r\n " +
                "    EXP_EJERCICIO_EXPEDIENTES EXPEJEEXPE,\r\n " +
                "    EXP_FASES EXPF\r\n " +
                "    WHERE\r\n " +
                "    EXPF.TIPO_FASE = 'F'\r\n " +
                "    AND\r\n " +
                "    EXPF.ID_COMPANIA = EXPEXPE.ID_COMPANIA\r\n " +
                "    AND\r\n " +
                "    EXPF.ID_FASE = EXPEXPE.ID_FASE\r\n " +
                "    AND\r\n " +
                "    EXPEJEEXPE.ID_COMPANIA = EXPEXPE.ID_COMPANIA\r\n " +
                "    AND\r\n " +
                "    EXPEJEEXPE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE\r\n " +
                "    AND\r\n " +
                "    EXPEJEEXPE.ID_TIPO_EXPEDIENTE =EXPEXPE.ID_TIPO_EXPEDIENTE\r\n " +
                "    AND\r\n " +
                "    EXPEXPE.ID_TIPO_EXPEDIENTE = :idTipoExpediente\r\n " +
                "    AND\r\n " +
                "    EXPEXPE.ID_COMPANIA = 1\r\n ";
                
                

            // *************** SE VALIDA EN LOS PARAMETROS SI EXISTE UN FILTRO CON FECHAS ***************
            // *************** DE LO CONTRARIO SE COLOCARA EL EJERCICIO ACTUAL            *************** 
            if (Filtros.FechaFinal != null)
            {
                param.Add("@fechaInicio", Filtros.FechaInicial);
                param.Add("@fechaFinal", Filtros.FechaFinal);

                sqlSelect += "    and TO_DATE(EXPEXPE.FECHA_ASIGNACION, 'dd/mm/yy') >= TO_DATE(:fechaInicio, 'dd/mm/yy') " +
                "    and TO_DATE(EXPEXPE.FECHA_ASIGNACION, 'dd/mm/yy') <= TO_DATE(:fechaFinal, 'dd/mm/yy') ";
            }
            else if (Filtros.EjercicioFiscal != null)
            {
                param.Add("@ejercicioFiscal", Filtros.EjercicioFiscal);

                sqlSelect += " AND TO_CHAR(EXPEXPE.FECHA_ASIGNACION, 'YYYY') = :ejercicioFiscal \n ";
            }
            else
            {
                var year = DateTime.Now.Year;
                param.Add("@ejercicioActual", year);
                sqlSelect +=
                    "AND \n" +
                    "    TO_CHAR(EXPEXPE.FECHA_ASIGNACION, 'YYYY') = :ejercicioActual \n";
            }

            sqlSelect += "    ORDER BY EXPEXPE.FECHA_ASIGNACION ASC";

            string sqlSelectInfoUsuario =
                "SELECT ID_COMPANIA  " +
                "FROM GEN_USUARIOS gu " +
                "WHERE gu.ID_USUARIO  = :IdUsuario";

            using var conn = await _connectionProvider.OpenAsync();

            try
            {
                UsuarioSsoModelo usuario = ValidarUsuario();
                param.Add("@IdUsuario", usuario.Nit);

                var IdCompania = await conn.QueryFirstOrDefaultAsync<int>(sqlSelectInfoUsuario, param);

                if (IdCompania < 1)
                {
                    throw new ResponseException("Usuario no asignado a una compañia.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }

                param.Add("@idTipoExpediente", Filtros.IdTipoExpediente);
                var lista = await conn.QueryAsync<ExpedientesConFaseModelo>(sqlSelect, param);

                return lista.AsList();

            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        
        public async Task<ResultadoHttpModelo> ObtenerEncabezadoExpediente(FiltrosEncabezadoModelo filtrosEncabezado, UsuarioSsoModelo usuario)
        {
            using var conn = await _connectionProvider.OpenAsync();

            try
            {
                var param = new DynamicParameters();
                param.Add("@pNoExpediente", filtrosEncabezado.NoExpediente);
                param.Add("@pIdTipoExpediente", filtrosEncabezado.IdTipoExpediente);

                var encabezado = await conn.QueryFirstOrDefaultAsync<EncabezadoExpedienteModelo>(sqlEncabezadoExpediente, param);
                var datos = await conn.QueryAsync<DatosExpedienteModelo>(sqlDatosExpediente, param);

                var historico = await conn.QueryAsync<HistoricoExpedienteModelo>(sqlHistoricoExpediente, param);


                return new ResultadoHttpModelo(EstadoSolicitudHttp.success)
                {
                    Mensaje = "Información descargada correctamente",
                    Titulo = "Dashboard",
                    Resultado = new {
                        encabezado,
                        datos,
                        historico
                    }
                };
            }
            catch (ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex);
                throw;
            }
        }
        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Luis Bautista    03/10/2023     Se creo el metodo para devolver las fases del 
        ///                                 tipo de Expediente por Compañia
        /// </summary>
        /// <param name="idTipoExpediente"></param>
        /// <param name="idCompania"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<IEnumerable<FaseModelo>> ObtenerFasesPorTipoExpediente(string idTipoExpediente, int idCompania)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdTipoExpediente", idTipoExpediente);
                param.Add("@IdCompania", idCompania);

                string sqlSelectFases =
                    "SELECT \n" +
                    "    DISTINCT(EXPGRPCN.ID_FASE) IdFase, \n" +
                    "    EXPF.DESCRIPCION NombreFase, \n" +
                    "    EXPF.TIPO_FASE TipoFase \n" +
                    "FROM EXP_GRUPO_CONSULTA_FASE EXPGRPCN, \n" +
                    "     EXP_FASES EXPF \n" +
                    "WHERE EXPF.ID_FASE = EXPGRPCN.ID_FASE \n" +
                    "AND EXPF.ID_COMPANIA = EXPGRPCN.ID_COMPANIA \n" +
                    "AND EXPGRPCN.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n" +
                    "AND EXPGRPCN.ID_COMPANIA = :IdCompania \n" +
                    "AND EXPF.TIPO_FASE<> 'F' \n" +
                    "ORDER BY EXPGRPCN.ID_FASE ASC";

                var listaFases = await conn.QueryAsync<FaseModelo>(sqlSelectFases, param);
                return listaFases;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "DashboardCotroller",
                    Repositorio = "DashboardRepositorio",
                    Metodo = "ObtenerFasesPorTipoExpediente"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al obtener las fases del tipo de expediente.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }


        public async Task<IEnumerable<FaseModelo>> ObtenerFasesExpedientesPorTipo(ExpedientesPorTipoModelo filtros, int idCompania)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdTipoExpediente", filtros.IdTipoExpediente);
                param.Add("@IdCompania", idCompania);
                param.Add("@FasesMostrar", filtros.FasesMostrar);

                string sqlSelectFases =
                    "SELECT \n" +
                    "    DISTINCT(EXPGRPCN.ID_FASE) IdFase, \n" +
                    "    EXPF.DESCRIPCION NombreFase, \n" +
                    "    EXPF.TIPO_FASE TipoFase \n" +
                    "FROM EXP_GRUPO_CONSULTA_FASE EXPGRPCN, \n" +
                    "     EXP_FASES EXPF \n" +
                    "WHERE EXPF.ID_FASE = EXPGRPCN.ID_FASE \n" +
                    "AND EXPF.ID_COMPANIA = EXPGRPCN.ID_COMPANIA \n" +
                    "AND EXPGRPCN.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n" +
                    "AND EXPGRPCN.ID_COMPANIA = :IdCompania \n" +
                    "AND EXPF.TIPO_FASE<> 'F' \n";

                if (filtros.FasesMostrar?.Length > 0)
                {
                    sqlSelectFases += "AND EXPF.ID_FASE IN :FasesMostrar \n";
                }
                sqlSelectFases += "ORDER BY EXPGRPCN.ID_FASE ASC";

                var listaFases = await conn.QueryAsync<FaseModelo>(sqlSelectFases, param);
                return listaFases;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "DashboardCotroller",
                    Repositorio = "DashboardRepositorio",
                    Metodo = "ObtenerFasesPorTipoExpediente"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al obtener las fases del tipo de expediente.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }


        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Luis Bautista    03/10/2023     Unicamente retorna el id de la compania del 
        ///                                 usuario logueado.
        /// </summary>
        /// <param name="NitUsuario"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<int> ObtenerCompania(string NitUsuario)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try
            {

                var parametro = new DynamicParameters();
                parametro.Add("@IdUsuario", NitUsuario);

                var idCompania = await conn.QueryFirstOrDefaultAsync<int>(sqlSelectInfoUsuario, parametro);
                return idCompania;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "DashboardCotroller",
                    Repositorio = "DashboardRepositorio",
                    Metodo = "ObtenerCompania"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al obtener la Compañia del usuario.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        public async Task<IEnumerable<int>> ObtenerEjerciciosPorTipoExpediente(string idTipoExpediente, int idCompania)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try
            {

                var parametro = new DynamicParameters();
                parametro.Add("@IdTipoExpediente", idTipoExpediente);
                parametro.Add("@IdCompania", idCompania);

                var listaEjercicios = await conn.QueryAsync<int>(sqlObtenerEjercicios, parametro);
                return listaEjercicios;


            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "DashboardCotroller",
                    Repositorio = "DashboardRepositorio",
                    Metodo = "ObtenerEjerciciosPorTipoExpediente"
                }).AddObject(ex).Submit();
                throw new ResponseException("Ocurrio un error al obtener el listado de ejercicios fiscales.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private const string sqlSelectInfoUsuario =
               "SELECT ID_COMPANIA  " +
               "FROM GEN_USUARIOS gu " +
               "WHERE gu.ID_USUARIO  = :IdUsuario";


        private const string sqlSelectTipoExpediente =
            "SELECT \n" +
            "	EXPEJEEXPE.ID_EJERCICIO || '-' || EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL ExpedienteGlobal, \n" +
            "	TO_CHAR(EXPEXPE.FECHA_GRABACION, 'DD/MM/YYYY HH24:MI:SS') FechaGrabacion, \n" +
            "   TRUNC((SYSDATE - EXPEXPE.FECHA_GRABACION))|| ' días'   TiempoTranscurrido," +
            "	EXPEXPE.RECIBIDO_DE RecibidoDe, \n" +
            "	EXPORI.DESCRIPCION Origen, \n" +
            "	PLA.ID_UNIDAD_ADMINISTRATIVA || ' - ' || PLA.DESCRIPCION UnidadAdministrativa, \n" +
            "	EXPEXPE.DESCRIPCION DescripcionIngreso, \n" +
            "	DECODE(EXPF.TIPO_FASE, 'F', NULL, GENU.ID_USUARIO || ' - ' || GENU.DESCRIPCION) UsuarioAsignado, \n" +
            "   EXPEXPE.ID_FASE IdFase \n" +
            "FROM \n" +
            "	EXP_TIPOS_EXPEDIENTE EXPTEXP, \n" +
            "	EXP_EXPEDIENTES EXPEXPE, \n" +
            "	EXP_EJERCICIO_EXPEDIENTES EXPEJEEXPE, \n" +
            "	EXP_FASES EXPF, \n" +
            "	GEN_USUARIOS GENU, \n" +
            "	EXP_ORIGENES EXPORI, \n" +
            "	PLA_UNIDADES_ADMINISTRATIVAS PLA, \n" +
            "	EXP_USUARIOS_FASE_EXPEDIENTE EXPUFE \n" +
            "WHERE EXPEXPE.CORRELATIVO_ASIGNACION = EXPUFE.CORRELATIVO \n" +
            "AND PLA.ID_UNIDAD_ADMINISTRATIVA = EXPEXPE.ID_UNIDAD_ADMINISTRATIVA \n" +
            "AND PLA.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPORI.ID_ORIGEN = EXPEXPE.ID_ORIGEN \n" +
            "AND EXPORI.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND GENU.ID_USUARIO = EXPEXPE.ID_USUARIO_ASIGNADO \n" +
            "AND GENU.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPF.ID_COMPANIA = EXPUFE.ID_COMPANIA \n" +
            "AND EXPF.ID_FASE = EXPEXPE.ID_FASE \n" +
            "AND EXPUFE.ID_FASE = EXPEXPE.ID_FASE \n" +
            "AND EXPUFE.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPUFE.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPUFE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE \n" +
            "AND EXPEJEEXPE.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPEJEEXPE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE \n" +
            "AND EXPEJEEXPE.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPTEXP.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPTEXP.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPEXPE.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n" +
            "AND EXPEJEEXPE.ID_EJERCICIO >= 2022";


        private const string sqlEncabezadoExpediente =
            "SELECT  \n" +
            "  EXPEJEEXPE.ID_EJERCICIO||'-'||EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL ExpedienteGlobal, \n" +
            "  EXPTEXP.ID_TIPO_EXPEDIENTE||' - '||EXPTEXP.DESCRIPCION IdTipoExpediente, \n" +
            "  TO_CHAR(EXPEXPE.FECHA_GRABACION,'DD/MM/YYYY HH24:MI:SS') FechaGrabacion, \n" +
            "  db_f_calcula_tiempo (SYSDATE - EXPEXPE.FECHA_GRABACION) TiempoTranscurrido, \n" +
            "  EXPEXPE.RECIBIDO_DE RecibidoDe, \n" +
            "  EXPORI.DESCRIPCION Origen, \n" +
            "  PLA.ID_UNIDAD_ADMINISTRATIVA ||' - ' || PLA.DESCRIPCION UnidadAdministrativa, \n" +
            "  EXPEXPE.DESCRIPCION DescripcionIngreso, \n" +
            "  EXPEXPE.OBSERVACIONES ObservacionesIngreso , \n" +
            "  EXPEXPE.ID_FASE||' - '||EXPF.DESCRIPCION IdFaseActual, \n" +
            "  TO_CHAR(EXPUFE.FECHA,'DD/MM/YYYY HH24:MI:SS') FechaTraslado, \n" +
            "  EXPUSER.ID_USUARIO || ' - ' || EXPUSER.DESCRIPCION UsuarioGrabo, \n" +
            "  DECODE(EXPF.TIPO_FASE,'F',NULL,GENU.ID_USUARIO || ' - ' || GENU.DESCRIPCION) UsuarioAsignado -- Se muestra usuario solo si no es una fase final \n" +
            "FROM  \n" +
            "  EXP_TIPOS_EXPEDIENTE EXPTEXP, \n" +
            "  EXP_EXPEDIENTES EXPEXPE, \n" +
            "  EXP_EJERCICIO_EXPEDIENTES EXPEJEEXPE, \n" +
            "  EXP_FASES EXPF, \n" +
            "  --EXP_FASES_EXPEDIENTE EXPFE, \n" +
            "  GEN_USUARIOS GENU, \n" +
            "  GEN_USUARIOS EXPUSER, \n" +
            "  EXP_ORIGENES EXPORI, \n" +
            "  PLA_UNIDADES_ADMINISTRATIVAS PLA, \n" +
            "  EXP_USUARIOS_FASE_EXPEDIENTE EXPUFE \n" +
            "WHERE EXPEXPE.CORRELATIVO_ASIGNACION =  EXPUFE.CORRELATIVO \n" +
            "AND PLA.ID_UNIDAD_ADMINISTRATIVA = EXPEXPE.ID_UNIDAD_ADMINISTRATIVA \n" +
            "AND PLA.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPORI.ID_ORIGEN = EXPEXPE.ID_ORIGEN \n" +
            "AND EXPORI.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND GENU.ID_USUARIO = EXPEXPE.ID_USUARIO_ASIGNADO \n" +
            "AND GENU.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPUSER.ID_USUARIO = EXPEXPE.ID_USUARIO \n" +
            "AND EXPUSER.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPF.ID_COMPANIA = EXPUFE.ID_COMPANIA \n" +
            "AND EXPF.ID_FASE = EXPEXPE.ID_FASE \n" +
            "AND EXPUFE.ID_FASE = EXPEXPE.ID_FASE \n" +
            "AND EXPUFE.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPUFE.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPUFE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE \n" +
            "AND EXPEJEEXPE.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPEJEEXPE.NO_EXPEDIENTE = EXPEXPE.NO_EXPEDIENTE \n" +
            "AND EXPEJEEXPE.ID_TIPO_EXPEDIENTE =EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPTEXP.ID_TIPO_EXPEDIENTE = EXPEXPE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPTEXP.ID_COMPANIA = EXPEXPE.ID_COMPANIA \n" +
            "AND EXPEXPE.NO_EXPEDIENTE = :pNoExpediente \n" +
            "AND EXPEXPE.ID_TIPO_EXPEDIENTE = :pIdTipoExpediente ";


        private const string sqlDatosExpediente =
            "SELECT \n" +
            "    ede.id_compania, \n" +
            "    ede.no_expediente, \n" +
            "    ede.id_tipo_expediente, \n" +
            "    ede.id_plantilla, \n" +
            "    ede.id_plantilla_detalle, \n" +
            "    gpd.etiqueta || ':' etiqueta, \n" +
            "    ede.valor, \n" +
            "    ede.secuencia_despliegue \n" +
            "FROM exp_datos_expediente ede \n" +
            "INNER JOIN gen_plantillas_detalle gpd \n" +
            "ON gpd.id_compania = ede.id_compania \n" +
            "AND gpd.id_plantilla_detalle = ede.id_plantilla_detalle \n" +
            "AND gpd.id_plantilla = ede.id_plantilla \n" +
            "INNER JOIN gen_plantillas gp \n" +
            "ON gp.id_compania = gpd.id_compania \n" +
            "AND gp.id_plantilla = gpd.id_plantilla \n" +
            "INNER JOIN exp_tipos_expediente ete \n" +
            "ON ete.id_compania = gp.id_compania \n" +
            "AND ete.id_plantilla_datos = gp.id_plantilla \n" +
            "WHERE ede.id_tipo_expediente = :pIdTipoExpediente \n" +
            "AND ete.id_tipo_expediente = ede.id_tipo_expediente \n" +
            "AND ede.no_expediente = :pNoExpediente \n" +
            "ORDER BY ede.secuencia_despliegue";


        private const string sqlHistoricoExpediente =
            "SELECT \n" +
            "	EXPTEXPE.DESCRIPCION DescripcionExpediente, \n" +
            "	EXPFE.NO_EXPEDIENTE NoExpediente, \n" +
            "	EXPFE.ID_FASE IdFase,\n" +
            "	EXPF.DESCRIPCION DescripcionFase,\n" +
            "	TO_CHAR(EXPFE.FECHA, 'dd/mm/yyyy HH24:MI:SS') FechaTraslado,\n" +
            "	EXPFE.ID_USUARIO IdUsuario, \n" +
            "	GENU.DESCRIPCION Usuario, \n" +
            "	EXPFE.OBSERVACIONES Observaciones\n" +
            "FROM EXP_FASES_EXPEDIENTE EXPFE,\n" +
            "	EXP_TIPOS_EXPEDIENTE EXPTEXPE,\n" +
            "	GEN_USUARIOS GENU, \n" +
            "	EXP_FASES EXPF\n" +
            "WHERE EXPF.ID_COMPANIA = EXPFE.ID_COMPANIA \n" +
            "AND EXPF.ID_FASE = EXPFE.ID_FASE \n" +
            "AND GENU.ID_USUARIO = EXPFE.ID_USUARIO \n" +
            "AND GENU.ID_COMPANIA = EXPFE.ID_COMPANIA \n" +
            "AND EXPTEXPE.ID_TIPO_EXPEDIENTE = EXPFE.ID_TIPO_EXPEDIENTE \n" +
            "AND EXPTEXPE.ID_COMPANIA = EXPFE.ID_COMPANIA \n" +
            "AND EXPFE.no_expediente = :pNoExpediente \n" +
            "AND EXPFE.ID_TIPO_EXPEDIENTE = :pIdTipoExpediente \n" +
            "AND EXPFE.ID_COMPANIA = 1\n" +
            "ORDER BY fecha DESC ";


        private const string sqlObtenerEjercicios =
            "SELECT DISTINCT(ID_EJERCICIO) \n" +
            "FROM EXP_EJERCICIO_EXPEDIENTES eee, \n" +
            "EXP_EXPEDIENTES ee, \n" +
            "EXP_FASES ef \n" +
            "WHERE ee.ID_COMPANIA = eee.ID_COMPANIA \n" +
            "AND ee.ID_TIPO_EXPEDIENTE = eee.ID_TIPO_EXPEDIENTE \n" +
            "AND ee.NO_EXPEDIENTE = eee.NO_EXPEDIENTE \n" +
            "AND ef.ID_COMPANIA  = ee.ID_COMPANIA \n" +
            "AND ef.ID_FASE = ee.ID_FASE \n" +
            "AND ef.TIPO_FASE<> 'F' \n" +
            "AND eee.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n" +
            "AND eee.ID_COMPANIA = :IdCompania \n" +
            "ORDER BY ID_EJERCICIO ";
    }
}
