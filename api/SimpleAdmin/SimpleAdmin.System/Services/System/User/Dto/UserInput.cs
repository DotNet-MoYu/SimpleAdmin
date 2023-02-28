using System.Linq.Expressions;

namespace SimpleAdmin.System;

/// <summary>
/// 用户选择器参数
/// </summary>
public class UserSelectorInput
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
    /// 关键字
    /// </summary>
    public virtual string SearchKey { get; set; }


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

    public string UserStatus { get; set; }
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
    [MinValue(1, ErrorMessage = "OrgId不能为空")]
    public override long OrgId { get; set; }

    /// <summary>
    /// 职位id
    /// </summary>
    [MinValue(1, ErrorMessage = "PositionId不能为空")]
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
    [MinValue(1, ErrorMessage = "Id不能为空")]
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
    [Required(ErrorMessage = "Id不能为空")]
    public long? Id { get; set; }

    /// <summary>
    /// 授权权限信息
    /// </summary>
    [Required(ErrorMessage = "RoleIdList不能为空")]
    public List<long> RoleIdList { get; set; }
}

/// <summary>
/// 用户导入
/// </summary>

public class SysUserImportInput : BaseImportTemplateInput
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
    [InDict(DevDictConst.GENDER)]
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
    [InDict(DevDictConst.NATION)]
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
    [InDict(DevDictConst.IDCARD_TYPE)]
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
    [InDict(DevDictConst.CULTURE_LEVEL)]
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