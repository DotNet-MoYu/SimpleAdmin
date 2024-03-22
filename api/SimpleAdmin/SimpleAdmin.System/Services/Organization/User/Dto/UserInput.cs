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
/// 用户选择器参数
/// </summary>
public class UserSelectorInput : BasePageInput
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
    /// 机构ID
    /// </summary>
    public long PositionId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string Account { get; set; }
}

/// <summary>
/// 用户分页查询参数
/// </summary>
public class UserPageInput : BasePageInput
{
    /// <summary>
    /// 所属组织
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 动态查询条件
    /// </summary>
    public Expressionable<SysUser> Expression { get; set; }

    /// <summary>
    /// 用户状态
    /// </summary>

    public string Status { get; set; }
}

/// <summary>
/// 添加用户参数
/// </summary>
public class UserAddInput : SysUser
{
    /// <summary>
    /// 账号
    /// </summary>
    [Required(ErrorMessage = "Account不能为空")]
    public override string Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public override string Name { get; set; }

    /// <summary>
    /// 组织id
    /// </summary>
    [IdNotNull(ErrorMessage = "OrgId不能为空")]
    public override long OrgId { get; set; }

    /// <summary>
    /// 职位id
    /// </summary>
    [IdNotNull(ErrorMessage = "PositionId不能为空")]
    public override long PositionId { get; set; }
}

/// <summary>
/// 编辑用户参数
/// </summary>
public class UserEditInput : UserAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 用户授权角色参数
/// </summary>
public class UserGrantRoleInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public long Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "RoleIdList不能为空")]
    public List<long> RoleIdList { get; set; }
}

public class UserGrantResourceInput : GrantResourceInput
{
    /// <summary>
    /// 默认数据权限
    /// </summary>
    [Required(ErrorMessage = "DefaultDataScope不能为空")]
    public DefaultDataScope DefaultDataScope { get; set; }
}

/// <summary>
/// 用户导入
/// </summary>
public class SysUserImportInput : ImportTemplateInput
{
    /// <summary>
    /// 账号
    /// </summary>
    [ImporterHeader(Name = "账号")]
    [Required(ErrorMessage = "账号不能为空")]
    public string Account { get; set; }

    /// <summary>
    /// 姓名
    ///</summary>
    [ImporterHeader(Name = "姓名")]
    [Required(ErrorMessage = "姓名不能为空")]
    public virtual string Name { get; set; }

    /// <summary>
    /// 性别
    ///</summary>
    [ImporterHeader(Name = "性别")]
    [Required(ErrorMessage = "性别不能为空")]
    public string Gender { get; set; }

    /// <summary>
    /// 昵称
    ///</summary>
    [ImporterHeader(Name = "昵称")]
    public string Nickname { get; set; }

    /// <summary>
    /// 手机
    /// 这里使用了SM4自动加密解密
    ///</summary>
    [ImporterHeader(Name = "手机号")]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    ///</summary>
    [ImporterHeader(Name = "邮箱")]
    [EmailAddress(ErrorMessage = "邮箱格式错误")]
    public string Email { get; set; }

    /// <summary>
    /// 所属机构
    /// </summary>
    [ImporterHeader(Name = "所属机构(子机构用/隔开)")]
    [Required(ErrorMessage = "所属部门不能为空")]
    [AntTable(Width = 200)]
    public string OrgName { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [ImporterHeader(Name = "职位")]
    [Required(ErrorMessage = "职位不能为空")]
    public string PositionName { get; set; }

    /// <summary>
    /// 出生日期
    ///</summary>
    [ImporterHeader(Name = "出生日期")]
    [AntTable(IsDate = true)]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 民族
    ///</summary>
    [ImporterHeader(Name = "民族")]
    public string Nation { get; set; }

    /// <summary>
    /// 籍贯
    ///</summary>
    [ImporterHeader(Name = "籍贯")]
    public string NativePlace { get; set; }

    /// <summary>
    /// 家庭住址
    ///</summary>
    [ImporterHeader(Name = "家庭住址")]
    public string HomeAddress { get; set; }

    /// <summary>
    /// 通信地址
    ///</summary>
    [ImporterHeader(Name = "通信地址")]
    public string MailingAddress { get; set; }

    /// <summary>
    /// 证件类型
    ///</summary>
    [ImporterHeader(Name = "证件类型")]
    public string IdCardType { get; set; }

    /// <summary>
    /// 证件号码
    ///</summary>
    [ImporterHeader(Name = "证件号码")]
    public string IdCardNumber { get; set; }

    /// <summary>
    /// 文化程度
    ///</summary>
    [ImporterHeader(Name = "文化程度")]
    public string CultureLevel { get; set; }

    /// <summary>
    /// 政治面貌
    ///</summary>
    [ImporterHeader(Name = "政治面貌")]
    public string PoliticalOutlook { get; set; }

    /// <summary>
    /// 毕业院校
    ///</summary>
    [ImporterHeader(Name = "毕业院校")]
    public string College { get; set; }

    /// <summary>
    /// 学历
    ///</summary>
    [ImporterHeader(Name = "学历")]
    public string Education { get; set; }

    /// <summary>
    /// 学制
    ///</summary>
    [ImporterHeader(Name = "学制")]
    public string EduLength { get; set; }

    /// <summary>
    /// 学位
    ///</summary>
    [ImporterHeader(Name = "学位")]
    public string Degree { get; set; }

    /// <summary>
    /// 家庭电话
    ///</summary>
    [ImporterHeader(Name = "家庭电话")]
    public string HomeTel { get; set; }

    /// <summary>
    /// 办公电话
    ///</summary>
    [ImporterHeader(Name = "办公电话")]
    public string OfficeTel { get; set; }

    /// <summary>
    /// 紧急联系人
    ///</summary>
    [ImporterHeader(Name = "紧急联系人")]
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系人电话
    ///</summary>
    [ImporterHeader(Name = "紧急联系人电话")]
    [Phone(ErrorMessage = "电话号码格式错误")]
    public string EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系人地址
    ///</summary>
    [ImporterHeader(Name = "紧急联系人地址")]
    public string EmergencyAddress { get; set; }

    /// <summary>
    /// 员工编号
    ///</summary>
    [ImporterHeader(Name = "员工编号")]
    public string EmpNo { get; set; }

    /// <summary>
    /// 入职日期
    ///</summary>
    [ImporterHeader(Name = "入职日期")]
    [AntTable(IsDate = true)]
    public DateTime? EntryDate { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    public long OrgId { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    [ImporterHeader(IsIgnore = true)]
    public long PositionId { get; set; }
}
