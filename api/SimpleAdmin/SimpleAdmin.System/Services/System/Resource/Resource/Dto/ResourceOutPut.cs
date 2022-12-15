namespace SimpleAdmin.System;

/// <summary>
/// 角色授权资源树输出
/// </summary>
public class ResTreeSelector
{

    /// <summary>
    /// 模块id
    /// </summary>
    public long Id { get; set; }


    /// <summary>
    /// 模块名称
    /// </summary>
    public string Title { get; set; }


    /// <summary>
    /// 模块图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 模块下菜单集合
    /// </summary>
    public List<RoleGrantResourceMenu> Menu { get; set; }

    /// <summary>
    /// 授权菜单类
    /// </summary>
    public class RoleGrantResourceMenu
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// 父id
        /// </summary>
        public long ParentId { get; set; }


        /// <summary>
        /// 父名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 模块id
        /// </summary>
        public long Module { get; set; }


        /// <summary>
        /// 菜单下按钮集合
        /// </summary>
        public List<RoleGrantResourceButton> Button { get; set; } = new List<RoleGrantResourceButton>();

    }

    public class RoleGrantResourceButton
    {
        /// <summary>
        /// 按钮id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

    }
}


public class PermissionTreeSelector
{

    /// <summary>
    /// 接口描述
    /// </summary>
    public string ApiName { get; set; }


    /// <summary>
    /// 路由名称
    /// </summary>
    public string ApiRoute { get; set; }


    /// <summary>
    /// 权限名称
    /// </summary>
    public string PermissionName { get; set; }

}
