// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 角色查询参数
/// </summary>
public class RolePageInput : PositionPageInput
{
}

/// <summary>
/// 角色添加参数
/// </summary>
public class RoleAddInput : SysRole
{
    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public override string Name { get; set; }
}

/// <summary>
/// 角色编辑参数
/// </summary>
public class RoleEditInput : RoleAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 角色授权资源参数
/// </summary>
public class GrantResourceInput : RoleOwnResourceOutput
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

    /// <summary>
    /// 授权资源信息
    /// </summary>
    [Required(ErrorMessage = "GrantInfoList不能为空")]
    public override List<RelationRoleResource> GrantInfoList { get; set; }

    /// <summary>
    /// 是否代码生成
    /// </summary>
    public bool IsCodeGen { get; set; }
}

/// <summary>
/// 角色授权资源参数
/// </summary>
public class GrantPermissionInput : RoleOwnPermissionOutput
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "GrantInfoList不能为空")]
    public override List<RelationRolePermission> GrantInfoList { get; set; }
}

/// <summary>
/// 角色授权用户参数
/// </summary>
public class GrantUserInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "GrantInfoList不能为空")]
    public List<long> GrantInfoList { get; set; }
}

/// <summary>
/// 角色选择器参数
/// </summary>
public class RoleSelectorInput : BasePageInput
{
    /// <summary>
    /// 组织ID
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 机构ID列表
    /// </summary>
    public List<long> OrgIds { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }
}

public class RoleTreeInput
{
    /// <summary>
    /// 机构ID列表
    /// </summary>
    public List<long> OrgIds { get; set; }
}
