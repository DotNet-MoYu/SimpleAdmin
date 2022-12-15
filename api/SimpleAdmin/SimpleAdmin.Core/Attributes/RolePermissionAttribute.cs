using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core;

/// <summary>
/// 需要角色授权权限
/// </summary>
public class RolePermissionAttribute : Attribute
{

}

/// <summary>
/// 忽略角色授权权限
/// </summary>
public class IgnoreRolePermissionAttribute : Attribute
{


}
