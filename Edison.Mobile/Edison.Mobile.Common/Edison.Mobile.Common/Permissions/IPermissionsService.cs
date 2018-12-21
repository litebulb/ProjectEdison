using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Mobile.Common
{
    public interface IPermissionsService
    {

        bool HasPermission(Permission permission);

        Task<bool> HasPermissionAsync(Permission permission);

        bool[] HasPermissions(Permission[] permission);

        Task<bool[]> HasPermissionsAsync(Permission[] permission);

        bool[] RequestPermissions(Permission[] permissions, int requestId, object activity = null);

        Task<bool[]> RequestPermissionsAsync(Permission[] permissions, int requestId, object activity = null);

    }
}
