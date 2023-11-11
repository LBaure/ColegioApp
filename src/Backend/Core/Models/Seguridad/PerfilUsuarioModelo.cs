using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class PerfilUsuarioModelo
    {
        public IEnumerable<OpcionesMenuUsuarioModelo> Menu;
        public IEnumerable<RolUsuarioModelo> Roles;
    }
}
