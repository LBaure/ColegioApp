using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CatalogoModelo
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class CatalogoOpcionMenuModelo
    {
        public int IdOpcionMenu { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? Path { get; set; }
    }

}
