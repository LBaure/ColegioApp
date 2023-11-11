using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Sso
{
    public class UsuarioSsoModelo
    {
        public string Nit { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string Correo { get; set; }
    }
}
