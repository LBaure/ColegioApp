using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Excepciones
{
    public class SsoException : Exception
    {
        public SsoException(string message) : base(message)
        {

        }
    }
}
