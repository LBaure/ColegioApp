using Core.Models;
using Core.Models.Seguridad;
using Core.Repositorios.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios.Seguridad
{
    public class PoliticaPrivacidadServicio : IPoliticaPrivacidadServicio
    {
        public readonly IPoliticaPrivacidadRepositorio _politicaPrivacidad;
        public PoliticaPrivacidadServicio(IPoliticaPrivacidadRepositorio politicaPrivacidadRepositorio)
        {
            _politicaPrivacidad = politicaPrivacidadRepositorio;            
        }

        public async Task<ResultadoHttpModelo> ActualizarPoliticaUsuario(CambioCredencialesModelo cambioCredenciales)
        {
            return await _politicaPrivacidad.ActualizarPoliticaUsuario(cambioCredenciales);
        }

        public async Task<ResultadoHttpModelo> EliminarMFA(string valor)
        {
            return await _politicaPrivacidad.EliminarMFA(valor);
        }

        public async Task<ResultadoHttpModelo> InsertarPoliticaUsuario(PoliticaUsuarioModelo politicaUsuarioModelo)
        {
            return await _politicaPrivacidad.InsertarPoliticaUsuario(politicaUsuarioModelo);            
        }

        public async Task<ResultadoHttpModelo> ObtenerConfiguracionMFA()
        {
            return await _politicaPrivacidad.ObtenerConfiguracionMFA();
        }

        public async Task<IEnumerable<PoliticaPrivacidadModelo>> ObtenerPoliticaPrivacidad()
        {
            return await _politicaPrivacidad.ObtenerPoliticaPrivacidad();
        }


    }
}
