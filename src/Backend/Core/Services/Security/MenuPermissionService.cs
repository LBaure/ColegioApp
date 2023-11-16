using Core.Models;
using Core.Models.Security;
using Core.Repositories.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Security
{
    public class MenuPermissionService : IMenuPermissionService
    {
        private readonly IMenuPermissionRepository _menuPermissionRepository;
        public MenuPermissionService(IMenuPermissionRepository menuPermissionRepository)
        {
            _menuPermissionRepository = menuPermissionRepository;            
        }

        public async Task<ResponseHttpModel> DeleteMenuPermissions(int id)
        {
            return await _menuPermissionRepository.DeleteMenuPermissions(id);
        }

        public async Task<ResponseHttpModel> GetListMenuPermissions()
        {
            return await _menuPermissionRepository.GetListMenuPermissions();
        }

        public async Task<ResponseHttpModel> InsertMenuPermissions(ListMenuPermissionModel model)
        {
            return await _menuPermissionRepository.InsertMenuPermissions(model);
        }

        public async Task<ResponseHttpModel> UpdateMenuPermissions(ListMenuPermissionModel permissionModel)
        {
            return await _menuPermissionRepository.UpdateMenuPermissions(permissionModel);
        }
    }
}
