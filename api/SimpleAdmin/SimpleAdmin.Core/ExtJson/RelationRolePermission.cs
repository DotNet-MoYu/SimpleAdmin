namespace SimpleAdmin.Core;

/// <summary>
/// SYS_ROLE_HAS_PERMISSION
/// 角色权限关系扩展
/// </summary>
public class RelationRolePermission
{

    /// <summary>
    /// 数据范围
    /// </summary>
    public string ScopeCategory { get; set; }

    /// <summary>
    /// 自定义机构范围列表
    /// </summary>
    public List<long> ScopeDefineOrgIdList { get; set; } = new List<long>();


    /// <summary>
    /// 接口Url
    /// </summary>
    public string ApiUrl { get; set; }


}
