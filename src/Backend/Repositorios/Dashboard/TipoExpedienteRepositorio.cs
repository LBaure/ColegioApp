using Core.Constantes;
using Core.Excepciones;
using Core.Models;
using Core.Models.Dashboard;
using Core.Models.Seguridad;
using Core.Repositorios;
using Core.Repositorios.Dashboard;
using Core.Servicios;
using Dapper;
using Exceptionless;
using Microsoft.AspNetCore.Mvc;
using Minfin.SSO.Api.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorios.Dashboard
{
    public class TipoExpedienteRepositorio : ITipoExpedienteRepositorio
    {

        private readonly IConnectionProvider _connectionProvider;

        private readonly IUsuarioActualServicio _usuarioActual;

        public TipoExpedienteRepositorio(IConnectionProvider connectionProvider, IUsuarioActualServicio usuarioActualServicio)
        {
            _connectionProvider = connectionProvider;
            _usuarioActual = usuarioActualServicio;
        }
        public async Task<IEnumerable<TipoExpedienteModelo>> ObtenerTiposExpedienteUsuario()
        {
            string sqlSelect =
            "SELECT \n" +
            "    DISTINCT(EXPUGCF.ID_TIPO_EXPEDIENTE) IdTipoExpediente, \n" +
            "    EXPTEXP.DESCRIPCION \n" +
            "FROM \n" +
            "EXP_USUARIO_GRUPO_COSULTA EXPUGC, \n" +
            "EXP_GRUPO_CONSULTA_FASE EXPUGCF, \n" +
            "EXP_TIPOS_EXPEDIENTE EXPTEXP \n" +
            "WHERE \n" +
            "EXPTEXP.ID_TIPO_PROCESO <> 1 \n" +
            "AND \n" +
            "EXPTEXP.ID_TIPO_EXPEDIENTE = EXPUGCF.ID_TIPO_EXPEDIENTE \n" +
            "AND \n" +
            "EXPTEXP.ID_COMPANIA = EXPUGCF.ID_COMPANIA \n" +
            "AND \n" +
            "EXPUGC.ID_GRUPO = EXPUGCF.ID_GRUPO \n" +
            "AND \n" +
            "EXPUGC.ID_COMPANIA=:idCompania \n" +
            "AND \n" +
            "EXPUGC.ID_USUARIO = :idUsuario \n" +
            "ORDER BY IdTipoExpediente";

            using var conn = await _connectionProvider.OpenAsync();

            try
            {
                var usuarioActual = _usuarioActual.Get();

                if (usuarioActual == null || usuarioActual.Nit == null)
                {
                    throw new ResponseException("Usuario no autenticado", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.Unauthorized);
                }

                var param = new DynamicParameters();
                //param.Add("@IdUsuario", usuarioActual.Nit);
                param.Add("@IdUsuario", "72176504");

                var IdCompania = await conn.QueryFirstOrDefaultAsync<int>(sqlSelectInfoUsuario, param);

                if (IdCompania < 1 )
                {
                    throw new ResponseException("Usuario no asignado a una compañia.", EstadoSolicitudHttp.warning, CodigoEstadoRespuestaHttp.BadRequest);
                }

                param.Add("@IdCompania", IdCompania);
                var lista = await conn.QueryAsync<TipoExpedienteModelo>(sqlSelect, param);
                conn.Close();
                return lista.AsList();

            }
            catch(ResponseException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<FasesConExpedientesWorkFlowModelo>> ObtenerFasesTipoExpediente(ExpedientesPorTipoModelo filtrosModelo, int idCompania)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try 
            {

                var param = new DynamicParameters();
                param.Add("@IdCompania", idCompania);
                param.Add("@IdTipoExpediente", filtrosModelo.IdTipoExpediente);
                param.Add("@FasesMostrar", filtrosModelo.FasesMostrar);

                string sqlSelectFases =
                "SELECT \n" +
                "    DISTINCT(EXPGRPCN.ID_FASE) IdFase, \n" +
                "    EXPF.DESCRIPCION Descripcion, \n" +
                "    EXPGRPCN.ID_TIPO_EXPEDIENTE IdTipoExpediente \n" +
                "FROM \n" +
                " \n" +
                "    EXP_GRUPO_CONSULTA_FASE EXPGRPCN, \n" +
                "    EXP_FASES EXPF \n" +
                "WHERE EXPF.ID_FASE = EXPGRPCN.ID_FASE \n" +
                "AND EXPF.ID_COMPANIA = EXPGRPCN.ID_COMPANIA \n" +
                "AND EXPGRPCN.ID_COMPANIA = :IdCompania \n" +
                "AND EXPGRPCN.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n" +
                "AND EXPF.TIPO_FASE<> 'F' \n";

                if (filtrosModelo.FasesMostrar?.Length > 0)
                {
                    sqlSelectFases += "AND EXPF.ID_FASE IN :FasesMostrar \n";
                }

                sqlSelectFases +=  "ORDER BY EXPGRPCN.ID_FASE ASC ";

                var lista = await conn.QueryAsync<FasesConExpedientesWorkFlowModelo>(sqlSelectFases, param);
                return lista;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "FasesExpedienteCotroller",
                    Repositorio = "TipoExpedienteRepositorio",
                    Metodo = "ObtenerFasesTipoExpediente"
                }).AddObject(ex).Submit();

                throw new ResponseException("Ocurrio un error al cargar las fases del tipo de expediente solicitado.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }
        /// <summary>
        /// ************************* Historial de Modificacion *************************
        /// ----dev-----    ---fecha---     ---------------- Descripcion ----------------
        /// Luis Bautista    02/10/2023     retorna los expedientes por fases
        /// </summary>
        /// <param name="filtrosModelo"></param>
        /// <param name="idCompania"></param>
        /// <param name="idFase"></param>
        /// <returns></returns>
        /// <exception cref="ResponseException"></exception>
        public async Task<IEnumerable<ExpedientesWorkFlowModelo>> ObtenerExpedientesPorFase(ExpedientesPorTipoModelo filtrosModelo, int idCompania, string? idFase)
        {
            using var conn = await _connectionProvider.OpenAsync();
            try
            {
                var param = new DynamicParameters();
                param.Add("@IdCompania", idCompania);
                param.Add("@IdTipoExpediente", filtrosModelo.IdTipoExpediente);
                param.Add("@IdFase", idFase);

                string sqlSelectExpedientes =
                    "SELECT \n" +
                    "    EXPEJEEXPE.ID_EJERCICIO || '-' || EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL NoExpedienteMinfin,  \n" +
                    "    EXPEXPE.NO_EXPEDIENTE NoExpediente,  \n" +
                    "	 TO_CHAR(EXPEXPE.FECHA_GRABACION, 'DD/MM/YYYY HH24:MI:SS') FechaGrabacion, \n" +
                    "    TRUNC((SYSDATE - EXPEXPE.FECHA_GRABACION)) || ' días' TiempoTranscurrido,  \n" +
                    "    EXPEXPE.ID_TIPO_EXPEDIENTE IdTipoExpediente,  \n" +
                    "    EXPEXPE.DESCRIPCION Descripcion,  \n" +
                    "    EXPEXPE.ID_FASE IdFase, \n" +
                    "    DECODE(EXPF.TIPO_FASE, 'F', NULL, GENU.ID_USUARIO || ' - ' || GENU.DESCRIPCION) UsuarioAsignado \n" +
                    "FROM \n" +
                    " \n" +
                    "    EXP_TIPOS_EXPEDIENTE EXPTEXP, \n" +
                    "    EXP_EXPEDIENTES EXPEXPE,  \n" +
                    "    EXP_EJERCICIO_EXPEDIENTES EXPEJEEXPE, \n" +
                    "    EXP_FASES EXPF,  \n" +
                    "    GEN_USUARIOS GENU, \n" +
                    "    EXP_ORIGENES EXPORI,  \n" +
                    "    PLA_UNIDADES_ADMINISTRATIVAS PLA, \n" +
                    "    EXP_USUARIOS_FASE_EXPEDIENTE EXPUFE \n" +
                    "WHERE \n" +
                    " \n" +
                    "    EXPEXPE.CORRELATIVO_ASIGNACION = EXPUFE.CORRELATIVO \n" +
                    "AND \n" +
                    "    EXPF.TIPO_FASE<> 'F' \n" +
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
                    "    EXPEXPE.ID_TIPO_EXPEDIENTE = :IdTipoExpediente \n" +
                    "AND  \n" +
                    "	EXPEXPE.ID_FASE = :IdFase \n";


                if (filtrosModelo.NoExpediente != null)
                {
                    param.Add("@Ejercicio", filtrosModelo.Ejercicio);
                    param.Add("@NoExpediente", filtrosModelo.NoExpediente);
                    sqlSelectExpedientes +=
                        "AND  \n" +
                        "	EXPEJEEXPE.ID_EJERCICIO = :Ejercicio \n" +
                        "AND  \n" +
                        "	EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL = :NoExpediente \n";
                }

                if (filtrosModelo.FechaInicial != null)
                {
                    param.Add("@fechaInicial", filtrosModelo.FechaInicial);
                    param.Add("@fechaFinal", filtrosModelo.FechaFinal);
                    sqlSelectExpedientes +=
                        "AND  \n" +
                        "	EXPEXPE.FECHA_GRABACION >= TO_DATE(:fechaInicial, 'DD/MM/YYYY' ) \n" +
                        "AND  \n" +
                        "	EXPEXPE.FECHA_GRABACION <= TO_DATE(:fechaFinal, 'DD/MM/YYYY') \n";
                }
                else
                {
                    var year = DateTime.Now.Year;
                    param.Add("@ejercicioActual", year);
                    sqlSelectExpedientes +=
                        "AND \n" +
                        "    TO_CHAR(EXPEXPE.FECHA_GRABACION, 'YYYY') = :ejercicioActual \n";
                }

                sqlSelectExpedientes += "ORDER BY NoExpedienteMinfin DESC ";
                var lista = await conn.QueryAsync<ExpedientesWorkFlowModelo>(sqlSelectExpedientes, param);
                return lista;
            }
            catch (Exception ex)
            {
                ex.ToExceptionless().AddObject(new
                {
                    controlador = "FasesExpedienteCotroller",
                    Repositorio = "TipoExpedienteRepositorio",
                    Metodo = "ObtenerExpedientesPorFase"
                }).AddObject(ex).Submit();

                throw new ResponseException("Ocurrio un error al cargar los expedientes por fases.", EstadoSolicitudHttp.error, CodigoEstadoRespuestaHttp.BadRequest);
            }
        }

        private const string sqlSelectInfoUsuario =
                "SELECT ID_COMPANIA  " +
                "FROM GEN_USUARIOS gu " +
                "WHERE gu.ID_USUARIO  = :IdUsuario";


        private const string sqlSelectExpedienteFase =
            "SELECT \n" +
            "	EXPEJEEXPE.ID_EJERCICIO || '-' || EXPEJEEXPE.NO_EXPEDIENTE_GLOBAL ExpedienteGlobal, \n" +
            "	TO_CHAR(EXPEXPE.FECHA_GRABACION, 'DD/MM/YYYY HH24:MI:SS') FechaGrabacion, \n" +
            "   TRUNC((SYSDATE - EXPEXPE.FECHA_GRABACION))|| ' días'   TiempoTranscurrido," +
            "	EXPEXPE.RECIBIDO_DE RecibidoDe, \n" +
            "	EXPORI.DESCRIPCION Origen, \n" +
            "	PLA.ID_UNIDAD_ADMINISTRATIVA || ' - ' || PLA.DESCRIPCION UnidadAdministrativa, \n" +
            "	EXPEXPE.DESCRIPCION DescripcionIngreso, \n" +
            "	DECODE(EXPF.TIPO_FASE, 'F', NULL, GENU.ID_USUARIO || ' - ' || GENU.DESCRIPCION) UsuarioAsignado \n" +
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
            "AND EXPEXPE.ID_FASE = :IdFase \n" +
            "AND EXPEJEEXPE.ID_EJERCICIO >= 2022"; 
    }
}
