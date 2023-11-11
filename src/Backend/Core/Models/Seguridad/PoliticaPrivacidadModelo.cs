using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class PoliticaPrivacidadModelo
    {
        public int IdPolitica { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Etiqueta { get; set; }
        public string? Pattern { get; set; }
        public int Minlength { get; set; }
        public string? MensajePattern { get; set; }
        public string? MensajeRequired { get; set; }
        public string? MensajeMinlength { get; set; }
        public bool Activo { get; set; }
        public int Maxlength { get; set; }
        public string? Mascara { get; set; }
        public string? Clase { get; set; }
        public string? DescripcionInicioSesion { get; set; }
    }

    public class PoliticaUsuarioModelo
    {
        public string? NitUsuario { get; set; }
        public int IdPolitica { get; set; }
        public string? Valor { get; set; }
        public int IntentosLogin { get; set; }
    }

    public class CambioCredencialesModelo
    {
        public string? ValorActual { get; set; }
        public string? ValorNuevo { get; set; }
        public int? IdPolitica { get; set; }
        public int IntentosLogin { get; set; }
    }
}
