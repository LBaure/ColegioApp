using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryScripts;

namespace Repositorios
{
    public class UsuarioRepositorio
    {
        //private UsuariosQueryScript usuariosQueryScript = new UsuariosQueryScript();
        //bool test;



        private readonly UsuariosQueryScript _usuariosQueryScript;

        public UsuarioRepositorio(UsuariosQueryScript usuariosQueryScript)
        {
            this._usuariosQueryScript = usuariosQueryScript;
            
        }
        public string ObtenerUsuarios()
        {
            string sql = this._usuariosQueryScript._obtenerUsuarios;



            return "";
        }
    }
}
