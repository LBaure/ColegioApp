using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Models.Security
{
    public class MenuPermissionModel
    {
        public int? IdPermission {get; set;}
        public int? IdParentPermission {get; set;}
        public string? Name {get; set;}
        public string? Description {get; set;}
        public string? Icon {get; set;}
        public string? Path {get; set;}
        public int? OrderMenu {get; set;}
        public bool? Active {get; set;}
        public bool? ShowMenu {get; set;}
    }

    public class ListMenuPermissionModel : MenuPermissionModel
    {
        public string? NameParentPermission { get; set; }
        public List<ListMenuPermissionModel>? MenuOptions { get; set; }
    }
}
