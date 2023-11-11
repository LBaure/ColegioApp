using Core.Models;
using Core.Models.Seguridad;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Seguridad
{
    public interface IPoliticaPrivacidadServicio
    {
        Task<IEnumerable<PoliticaPrivacidadModelo>> ObtenerPoliticaPrivacidad();
        Task<ResultadoHttpModelo> ObtenerConfiguracionMFA();
        Task<ResultadoHttpModelo> EliminarMFA(string valor);
        Task<ResultadoHttpModelo> InsertarPoliticaUsuario(PoliticaUsuarioModelo politicaUsuarioModelo);
        Task<ResultadoHttpModelo> ActualizarPoliticaUsuario(CambioCredencialesModelo cambioCredenciales);

    }
}
