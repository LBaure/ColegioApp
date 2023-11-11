using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryScripts
{
    public class UsuariosQueryScript
    {

        public readonly string _obtenerUsuarios =
        "SELECT \n" +
        "   * \n" +
        "FROM \n" +
        "   DUAL";
    }

}
