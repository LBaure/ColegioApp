using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class CredencialesUsuarioModelo
    {
        public string NitUsuario { get; set; }
        public string Valor { get; set; }
        public bool DobleFactor { get; set; }
    }
}
