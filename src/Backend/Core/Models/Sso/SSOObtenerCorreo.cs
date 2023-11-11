using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Sso
{
    public class SSOObtenerCorreo
    {
        [Required(ErrorMessage = "El Nit es requerido")]
        public string Nit;

        [Required(ErrorMessage = "La private key es requerida")]
        public string PrivateKeyXml;
    }
}
