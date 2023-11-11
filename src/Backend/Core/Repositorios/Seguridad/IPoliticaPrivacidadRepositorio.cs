using Core.Models;
using Core.Models.Seguridad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositorios.Seguridad
{
    public interface IPoliticaPrivacidadRepositorio
    {
        Task<IEnumerable<PoliticaPrivacidadModelo>> ObtenerPoliticaPrivacidad();
        Task<ResultadoHttpModelo> ObtenerConfiguracionMFA();
        Task<ResultadoHttpModelo> EliminarMFA(string valor);
        Task<ResultadoHttpModelo> InsertarPoliticaUsuario(PoliticaUsuarioModelo politicaUsuarioModelo);
        Task<IntentosSesionModelo> ObtenerIntentosPermitidosLogin(string nitUsuario, IDbConnection _connection, IDbTransaction _transaction);
        Task CambiarIntentosSesion(IDbConnection _connection, IDbTransaction _transaction, string nitUsuario, int? intentosSesion);
        Task<bool> ValidarCredencialesUsuario(IDbConnection _connection, IDbTransaction _transaction, string nitUsuario, string valor);
        Task<ResultadoHttpModelo> ActualizarPoliticaUsuario(CambioCredencialesModelo cambioCredenciales);
    }
}
