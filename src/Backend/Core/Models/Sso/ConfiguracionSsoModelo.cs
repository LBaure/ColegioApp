using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Sso
{
    public class ConfiguracionSsoModelo
    {
        public const string Sso = "sso";
        public string ServiceEndpoint { get; set; }
        public string LoginUrl { get; set; }
        public string PrivateKeyFile { get; set; }
        public string RedirectUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string UsuarioRegistranteSso { get; set; }
    }
}
