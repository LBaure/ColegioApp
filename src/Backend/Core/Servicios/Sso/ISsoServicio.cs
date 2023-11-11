
using Core.Models.Sso;

namespace Core.Servicios.Sso
{
    public interface ISsoServicio
    {
        UsuarioSsoModelo Autenticar(SsoAutModelo SSOReq);
    }
}
