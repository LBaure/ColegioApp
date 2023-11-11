using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Sso
{
    public class SsoAutModelo
    {
        [Required(ErrorMessage = "La private key es requerida")]
        public string PrivateKeyXml;
        [Required(ErrorMessage = "El ticket de autorización es requerido")]
        public string TicketAut;
    }
}
