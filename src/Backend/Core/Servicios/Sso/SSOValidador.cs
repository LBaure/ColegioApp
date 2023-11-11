using Core.Models.Sso;
using Core.Validadores;

namespace Core.Servicios.Sso
{
    public class SSOValidador : ISsoServicio
    {
        private readonly ISsoServicio _servicio;
        private readonly IValidador _validador;
        public SSOValidador(IValidador validador, ISsoServicio servicio)
        {
            _validador = validador;
            _servicio = servicio;
        }
        public UsuarioSsoModelo Autenticar(SsoAutModelo SSOReq)
        {
            _validador.Validar(SSOReq);
            return _servicio.Autenticar(SSOReq);
        }
    }
}
