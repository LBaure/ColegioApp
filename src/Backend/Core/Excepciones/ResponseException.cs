using Core.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Excepciones
{
    public class ResponseException : Exception
    {
        public string mensaje;
        public string estado;
        public int codigo;

        public ResponseException(string mensaje, EstadoSolicitudHttp estado, CodigoEstadoRespuestaHttp codigo) : base(mensaje)
        { 
            this.mensaje = mensaje;
            this.estado = estado.ToString();
            this.codigo = (int)codigo;
        }
    }
}
