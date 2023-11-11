using Core.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public  class ResultadoHttpModelo
    {
        public string? Estado { get; private set; }
        public string? Icono { get; private set; }
        public string? Mensaje { get; set; }
        public string? Titulo { get; set; }
        public dynamic? Resultado { get; set; }

        public ResultadoHttpModelo()
        {
            Estado = "";
            Icono = "";
            Mensaje = "";
            Titulo = "";
        }
        public ResultadoHttpModelo(EstadoSolicitudHttp estado)
        {
            Estado = estado.ToString();
            Icono = estado.ToString();
        }
    }
}
