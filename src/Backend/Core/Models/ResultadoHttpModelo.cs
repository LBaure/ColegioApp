using Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ResultadoHttpModelo
    {
        public string? Estado { get; }
        public string? Mensaje { get; }
        public string? Titulo { get; }
        public dynamic? Resultado { get; }

        public ResultadoHttpModelo()
        {
            Estado = "";
            Mensaje = "";
            Titulo = "";
        }

        public ResultadoHttpModelo(EstadoSolicitudHttp estado, string mensaje, string titulo, dynamic? resultado = null)
        {
            Estado = estado.ToString();
            Mensaje = mensaje;
            Titulo = titulo;
            Resultado = resultado;
        }
    }
}
