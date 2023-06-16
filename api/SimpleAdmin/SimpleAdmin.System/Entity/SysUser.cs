namespace SimpleAdmin.System;

/// <summary>
/// 用户信息表
///</summary>
[SugarTable("sys_user", TableDescription = "用户信息表")]
[Tenant(SqlsugarConst.DB_Default)]
[BatchEdit]
[CodeGen]
public class SysUser : BaseEntity
{
    /// <summary>
    /// 头像
    ///</summary>
    [SugarColumn(ColumnName = "Avatar", ColumnDescription = "头像", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public virtual string Avatar { get; set; }

    /// <summary>
    /// 签名
    ///</summary>
    [SugarColumn(ColumnName = "Signature", ColumnDescription = "签名", ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public string Signature { get; set; }

    /// <summary>
    /// 账号
    ///</summary>
    [SugarColumn(ColumnName = "Account", ColumnDescription = "账号", Length = 200, IsNullable = false)]
    public virtual string Account { get; set; }

    /// <summary>
    /// 密码
    ///</summary>
    [SugarColumn(ColumnName = "Password", ColumnDescription = "密码", Length = 200, IsNullable = false)]
    public string Password { get; set; }

    /// <summary>
    /// 姓名
    ///</summary>
    [SugarColumn(ColumnName = "Name", ColumnDescription = "姓名", Length = 200, IsNullable = true)]
    public virtual string Name { get; set; }

    /// <summary>
    /// 昵称
    ///</summary>
    [SugarColumn(ColumnName = "Nickname", ColumnDescription = "昵称", Length = 200, IsNullable = true)]
    public string Nickname { get; set; }

    /// <summary>
    /// 性别
    ///</summary>
    [SugarColumn(ColumnName = "Gender", ColumnDescription = "性别", Length = 200, IsNullable = true)]
    public string Gender { get; set; }

    /// <summary>
    /// 出生日期
    ///</summary>
    [SugarColumn(ColumnName = "Birthday", ColumnDescription = "出生日期", Length = 200, IsNullable = true)]
    public string Birthday { get; set; }

    /// <summary>
    /// 民族
    ///</summary>
    [SugarColumn(ColumnName = "Nation", ColumnDescription = "民族", Length = 200, IsNullable = true)]
    public string Nation { get; set; }

    /// <summary>
    /// 籍贯
    ///</summary>
    [SugarColumn(ColumnName = "NativePlace", ColumnDescription = "籍贯", Length = 200, IsNullable = true)]
    public string NativePlace { get; set; }

    /// <summary>
    /// 家庭住址
    ///</summary>
    [SugarColumn(ColumnName = "HomeAddress", ColumnDescription = "家庭住址", IsNullable = true)]
    public string HomeAddress { get; set; }

    /// <summary>
    /// 通信地址
    ///</summary>
    [SugarColumn(ColumnName = "MailingAddress", ColumnDescription = "通信地址", IsNullable = true)]
    public string MailingAddress { get; set; }

    /// <summary>
    /// 证件类型
    ///</summary>
    [SugarColumn(ColumnName = "IdCardType", ColumnDescription = "证件类型", Length = 200, IsNullable = true)]
    public string IdCardType { get; set; }

    /// <summary>
    /// 证件号码
    ///</summary>
    [SugarColumn(ColumnName = "IdCardNumber", ColumnDescription = "证件号码", Length = 200, IsNullable = true)]
    public string IdCardNumber { get; set; }

    /// <summary>
    /// 文化程度
    ///</summary>
    [SugarColumn(ColumnName = "CultureLevel", ColumnDescription = "文化程度", Length = 200, IsNullable = true)]
    public string CultureLevel { get; set; }

    /// <summary>
    /// 政治面貌
    ///</summary>
    [SugarColumn(ColumnName = "PoliticalOutlook", ColumnDescription = "政治面貌", Length = 200, IsNullable = true)]
    public string PoliticalOutlook { get; set; }

    /// <summary>
    /// 毕业院校
    ///</summary>
    [SugarColumn(ColumnName = "College", ColumnDescription = "毕业院校", Length = 200, IsNullable = true)]
    public string College { get; set; }

    /// <summary>
    /// 学历
    ///</summary>
    [SugarColumn(ColumnName = "Education", ColumnDescription = "学历", Length = 200, IsNullable = true)]
    public string Education { get; set; }

    /// <summary>
    /// 学制
    ///</summary>
    [SugarColumn(ColumnName = "EduLength", ColumnDescription = "学制", Length = 200, IsNullable = true)]
    public string EduLength { get; set; }

    /// <summary>
    /// 学位
    ///</summary>
    [SugarColumn(ColumnName = "Degree", ColumnDescription = "学位", Length = 200, IsNullable = true)]
    public string Degree { get; set; }

    /// <summary>
    /// 手机
    /// 这里使用了SM4自动加密解密
    ///</summary>
    [SugarColumn(ColumnName = "Phone", ColumnDescription = "手机", Length = 200, IsNullable = true)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    ///</summary>
    [SugarColumn(ColumnName = "Email", ColumnDescription = "邮箱", Length = 200, IsNullable = true)]
    public string Email { get; set; }

    /// <summary>
    /// 家庭电话
    ///</summary>
    [SugarColumn(ColumnName = "HomeTel", ColumnDescription = "家庭电话", Length = 200, IsNullable = true)]
    public string HomeTel { get; set; }

    /// <summary>
    /// 办公电话
    ///</summary>
    [SugarColumn(ColumnName = "OfficeTel", ColumnDescription = "办公电话", Length = 200, IsNullable = true)]
    public string OfficeTel { get; set; }

    /// <summary>
    /// 紧急联系人
    ///</summary>
    [SugarColumn(ColumnName = "EmergencyContact", ColumnDescription = "紧急联系人", Length = 200, IsNullable = true)]
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系人电话
    ///</summary>
    [SugarColumn(ColumnName = "EmergencyPhone", ColumnDescription = "紧急联系人电话", Length = 200, IsNullable = true)]
    public string EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系人地址
    ///</summary>
    [SugarColumn(ColumnName = "EmergencyAddress", ColumnDescription = "紧急联系人地址", IsNullable = true)]
    public string EmergencyAddress { get; set; }

    /// <summary>
    /// 员工编号
    ///</summary>
    [SugarColumn(ColumnName = "EmpNo", ColumnDescription = "员工编号", Length = 200, IsNullable = true)]
    public string EmpNo { get; set; }

    /// <summary>
    /// 入职日期
    ///</summary>
    [SugarColumn(ColumnName = "EntryDate", ColumnDescription = "入职日期", Length = 200, IsNullable = true)]
    public string EntryDate { get; set; }

    /// <summary>
    /// 机构id
    ///</summary>
    [SugarColumn(ColumnName = "OrgId", ColumnDescription = "机构id", IsNullable = false)]
    public virtual long OrgId { get; set; }

    /// <summary>
    /// 职位id
    ///</summary>
    [SugarColumn(ColumnName = "PositionId", ColumnDescription = "职位id", IsNullable = false)]
    public virtual long PositionId { get; set; }

    /// <summary>
    /// 职级
    ///</summary>
    [SugarColumn(ColumnName = "PositionLevel", ColumnDescription = "职级", Length = 200, IsNullable = true)]
    public string PositionLevel { get; set; }

    /// <summary>
    /// 主管id
    ///</summary>
    [SugarColumn(ColumnName = "DirectorId", ColumnDescription = "主管id", IsNullable = true)]
    public long? DirectorId { get; set; }

    /// <summary>
    /// 兼任信息
    ///</summary>
    [SugarColumn(ColumnName = "PositionJson", ColumnDescription = "兼任信息", IsJson = true, ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
    public List<PositionJson> PositionJson { get; set; }

    /// <summary>
    /// 上次登录ip
    ///</summary>
    [SugarColumn(ColumnName = "LastLoginIp", ColumnDescription = "上次登录ip", Length = 200, IsNullable = true)]
    public string LastLoginIp { get; set; }

    /// <summary>
    /// 上次登录地点
    ///</summary>
    [SugarColumn(ColumnName = "LastLoginAddress", ColumnDescription = "上次登录地点", Length = 200, IsNullable = true)]
    public string LastLoginAddress { get; set; }

    /// <summary>
    /// 上次登录时间
    ///</summary>
    [SugarColumn(ColumnName = "LastLoginTime", ColumnDescription = "上次登录时间", IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 上次登录设备
    ///</summary>
    [SugarColumn(ColumnName = "LastLoginDevice", ColumnDescription = "上次登录设备", IsNullable = true)]
    public string LastLoginDevice { get; set; }

    /// <summary>
    /// 最新登录ip
    ///</summary>
    [SugarColumn(ColumnName = "LatestLoginIp", ColumnDescription = "最新登录ip", Length = 200, IsNullable = true)]
    public string LatestLoginIp { get; set; }

    /// <summary>
    /// 最新登录地点
    ///</summary>
    [SugarColumn(ColumnName = "LatestLoginAddress", ColumnDescription = "最新登录地点", Length = 200, IsNullable = true)]
    public string LatestLoginAddress { get; set; }

    /// <summary>
    /// 最新登录时间
    ///</summary>
    [SugarColumn(ColumnName = "LatestLoginTime", ColumnDescription = "最新登录时间", IsNullable = true)]
    public DateTime? LatestLoginTime { get; set; }

    /// <summary>
    /// 最新登录设备
    ///</summary>
    [SugarColumn(ColumnName = "LatestLoginDevice", ColumnDescription = "最新登录设备", IsNullable = true)]
    public string LatestLoginDevice { get; set; }

    /// <summary>
    /// 用户状态
    ///</summary>
    [SugarColumn(ColumnName = "UserStatus", ColumnDescription = "用户状态", Length = 200, IsNullable = true)]
    public string UserStatus { get; set; }

    /// <summary>
    /// 排序码
    ///</summary>
    [SugarColumn(ColumnName = "SortCode", ColumnDescription = "排序码", IsNullable = true)]
    public int? SortCode { get; set; }

    /// <summary>
    /// 机构信息
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string OrgName { get; set; }

    /// <summary>
    /// 机构信息全称
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string OrgNames { get; set; }

    /// <summary>
    /// 职位信息
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public string PositionName { get; set; }

    /// <summary>
    /// 按钮码集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<string> ButtonCodeList { get; set; }

    /// <summary>
    /// 权限码集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<string> PermissionCodeList { get; set; }

    /// <summary>
    /// 角色码集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<string> RoleCodeList { get; set; }

    /// <summary>
    /// 角色ID集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<long> RoleIdList { get; set; }

    /// <summary>
    /// 数据范围集合
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public List<DataScope> DataScopeList { get; set; }

    /// <summary>
    /// 默认数据范围
    /// </summary>
    [SugarColumn(IsIgnore = true)]
    public DefaultDataScope DefaultDataScope { get; set; }
}

/// <summary>
/// 数据范围类
/// </summary>
public class DataScope
{
    /// <summary>
    /// API接口
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// 数据范围
    /// </summary>
    public List<long>? DataScopes { get; set; }
}

/// <summary>
/// 兼任信息
/// </summary>
public class PositionJson
{
    /// <summary>
    /// 组织ID
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 机构ID
    /// </summary>
    public long PositionId { get; set; }

    /// <summary>
    /// 主管ID
    /// </summary>
    public long? DirectorId { get; set; }
}