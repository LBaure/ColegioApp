using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Seguridad
{
    public class ConfiguracionUsuarioModelo
    {
        [Required]
        public string? NitUsuario { get; set; }
        public bool DobleFactorAuth { get; set; }
        public int IntentosLogin { get; set; }
        public int IdPolitica { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public string Etiqueta { get; set; }
        public int MinLength { get; set; }
        public string Pattern { get; set; }
        public string MessagePattern { get; set; }
        public string MessageRequired { get; set; }
        public string MessageMinlength { get; set; }
        public int MaxLength { get; set; }
        public string Mask { get; set; }
        public string Class { get; set; }
        public bool SesionActiva { get; set; }
        public string DescriptionLogin { get; set; }
        public bool DobleFactorObligatorio { get; set; }
}
}
