using Core.Models;
using Core.Models.Seguridad;
using Core.Models.Sso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servicios
{
    public interface IUsuarioActualServicio
    {
        UsuarioSsoModelo Get();
    }
}
