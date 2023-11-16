using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ResponseHttpModel
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string? Title { get; set; }
        public dynamic? Result { get; set; }
    }
}
