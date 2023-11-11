using Core.Models.Sso;
using Core.Servicios.Sso;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;
using IoFile = System.IO.File;
using Core.Servicios.Seguridad;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SsoController : Controller
    {

        private readonly ConfiguracionSsoModelo _configuracion;
        private readonly ISsoServicio _ssoServicio;
        private readonly IUsuarioServicio _usuarioServicio;

        public SsoController(ConfiguracionSsoModelo configuracionSsoModelo, ISsoServicio ssoServicio, IUsuarioServicio usuarioServicio)
        {
            _configuracion = configuracionSsoModelo;
            _ssoServicio = ssoServicio;
            _usuarioServicio = usuarioServicio;
        }


        [HttpGet("autenticar")]
        public IActionResult Autenticar(string ticketAut)
        {
            try
            {
                var privateKeyXml = GetPrivateKeyXml(_configuracion.PrivateKeyFile);
                SsoAutModelo SSOReq = new SsoAutModelo();
                SSOReq.TicketAut = ticketAut;
                SSOReq.PrivateKeyXml = privateKeyXml;
                var usuario = _ssoServicio.Autenticar(SSOReq);
                return RedirectAutenticado(usuario);
            }
            catch (Exception ex)
            {
                return RedirectError(ex.Message);
            }
        }
        /// <summary>
        /// Método que obtiene la llave privada para la conexión con Sso
        /// </summary>
        /// <param name="path">Path del archivo que contiene la llave privada</param>
        /// <returns>
        /// Llave privada de conexión con Sso
        /// </returns>
        private static string GetPrivateKeyXml(string path)
        {
            Console.WriteLine("***********  PATH  *************" + path);
            if (!IoFile.Exists(path))
            {
                throw new Exception("No existe una llave privada para validar la autenticación.");
            }
            return IoFile.ReadAllText(path);
        }

        /// <summary>
        /// Método que redirecciona al usaurio autenticado
        /// </summary>
        /// <param name="usuario">Información del usuario autenticado</param>
        /// <returns>
        /// Redirecciona al usuario a la página inicial de SRBM
        /// </returns>
        private IActionResult RedirectAutenticado(UsuarioSsoModelo usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Nit),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo)
            };
            var id = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(id);
            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;

            HttpContext.SignInAsync(scheme, principal).Wait();
            //ReqRegistraBitacoraLogin req = new ReqRegistraBitacoraLogin();
            //req.NitEmpresa = null;
            //req.NitUsuario = usuario.Nit;
            //req.Roles = null;
            //RegistrarBitacoraLogin.RegistraBitacora(req);

            return Redirect(_configuracion.RedirectUrl);
        }


        /// <summary>
        /// Método que redirecciona al usaurio a la página de Error
        /// </summary>
        /// <param name="mensaje">Mensaje de error a mostrar</param>
        /// <returns>
        /// Redirecciona al usuario a la página de Error
        /// </returns>
        private IActionResult RedirectError(string mensaje)
        {
            var encodedMessage = HttpUtility.UrlEncode(mensaje);
            return Redirect(string.Format(_configuracion.ErrorUrl, encodedMessage));
        }

        [HttpGet("usuario")]
        public async Task<IActionResult> Get()
        {
            if (User.Identity is ClaimsIdentity identity)
            {
                Console.WriteLine("***** aqui llego*****");
                if (identity.Claims.Any())
                {
                    var userClaims = identity.Claims;
                    var claimsDictionary = (User.Identity as ClaimsIdentity)?.Claims.ToDictionary(x => x.Type, x => x.Value);
                    string nit = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
                    var perfil = await _usuarioServicio.ObtenerPerfilUsuarioAsync(nit);
                    return Json(new
                    {
                        identity.IsAuthenticated,
                        identity.Name,
                        nit,
                        Perfil = new
                        {
                            perfil.Menu,
                            perfil.Roles
                        },
                        Claims = claimsDictionary
                    });
                }
                else
                {
                    return Json(new { User.Identity.IsAuthenticated });
                }
            }
            else
            {
                return Json(new { IsAuthenticated = false });
            }
        }
    }
}
