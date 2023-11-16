using Core.Models;
using Core.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Security
{
    public interface IMenuPermissionService
    {
        Task<ResponseHttpModel> GetListMenuPermissions();
        Task<ResponseHttpModel> InsertMenuPermissions(ListMenuPermissionModel model);
        Task<ResponseHttpModel> UpdateMenuPermissions(ListMenuPermissionModel permissionModel);
        Task<ResponseHttpModel> DeleteMenuPermissions(int id);
    }
}
