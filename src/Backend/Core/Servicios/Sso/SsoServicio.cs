


using Core.Excepciones;
using Core.Models.Sso;
using Core.Repositorios.Sso;

namespace Core.Servicios.Sso
{
    public class SsoServicio : ISsoServicio
    {
        IUserProvider userProvider;
        //  ConfiguracionSsoModelo configuracion;

        //        public SsoServicio(IUserProvider userProvider, ConfiguracionSsoModelo configuracion)
        public SsoServicio(IUserProvider userProvider)
        {
            this.userProvider = userProvider;
            //this.configuracion = configuracion;
        }

        public UsuarioSsoModelo Autenticar(SsoAutModelo SSOReq)
        {
            if (string.IsNullOrWhiteSpace(SSOReq.PrivateKeyXml))
            {
                throw new SsoException("La llave privada se encuentra vacía");
            }

            if (string.IsNullOrWhiteSpace(SSOReq.TicketAut))
            {
                throw new SsoException("No se especificó ningun ticket de autenticación.");
            }

            return userProvider.Autenticar(SSOReq);
        }
    }
}