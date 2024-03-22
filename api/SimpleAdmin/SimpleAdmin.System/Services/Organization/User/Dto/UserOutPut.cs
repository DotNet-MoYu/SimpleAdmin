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
/// 选择用户输出参数
/// </summary>
public class UserSelectorOutPut
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 组织ID
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 性别
    ///</summary>
    public string Gender { get; set; }
}

/// <summary>
/// 用户信息输出
/// </summary>
public class UserInfoOutPut : SysUser
{
}

/// <summary>
/// 用户信息
/// </summary>
[ExcelExporter(Name = "用户信息", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
public class SysUserExportOutput
{
    /// <summary>
    /// 账号
    ///</summary>
    [ExporterHeader(DisplayName = "账号")]
    public virtual string Account { get; set; }

    /// <summary>
    /// 姓名
    ///</summary>
    [ExporterHeader(DisplayName = "姓名")]
    public virtual string Name { get; set; }

    /// <summary>
    /// 昵称
    ///</summary>
    [ExporterHeader(DisplayName = "昵称")]
    public string Nickname { get; set; }

    /// <summary>
    /// 性别
    ///</summary>
    [ExporterHeader(DisplayName = "性别")]
    public string Gender { get; set; }

    /// <summary>
    /// 手机
    /// 这里使用了SM4自动加密解密
    ///</summary>
    [ExporterHeader(DisplayName = "手机号")]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    ///</summary>
    [ExporterHeader(DisplayName = "邮箱")]
    public string Email { get; set; }

    /// <summary>
    /// 所属机构
    /// </summary>
    [ExporterHeader(DisplayName = "所属机构(子机构用/隔开)")]
    public string OrgNames { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [ExporterHeader(DisplayName = "职位")]
    public string PositionName { get; set; }

    /// <summary>
    /// 出生日期
    ///</summary>
    [ExporterHeader(DisplayName = "出生日期")]
    public string Birthday { get; set; }

    /// <summary>
    /// 民族
    ///</summary>
    [ExporterHeader(DisplayName = "民族")]
    public string Nation { get; set; }

    /// <summary>
    /// 籍贯
    ///</summary>
    [ExporterHeader(DisplayName = "籍贯")]
    public string NativePlace { get; set; }

    /// <summary>
    /// 家庭住址
    ///</summary>
    [ExporterHeader(DisplayName = "家庭住址")]
    public string HomeAddress { get; set; }

    /// <summary>
    /// 通信地址
    ///</summary>
    [ExporterHeader(DisplayName = "通信地址")]
    public string MailingAddress { get; set; }

    /// <summary>
    /// 证件类型
    ///</summary>
    [ExporterHeader(DisplayName = "证件类型")]
    public string IdCardType { get; set; }

    /// <summary>
    /// 证件号码
    ///</summary>
    [ExporterHeader(DisplayName = "证件号码")]
    public string IdCardNumber { get; set; }

    /// <summary>
    /// 文化程度
    ///</summary>
    [ExporterHeader(DisplayName = "文化程度")]
    public string CultureLevel { get; set; }

    /// <summary>
    /// 政治面貌
    ///</summary>
    [ExporterHeader(DisplayName = "政治面貌")]
    public string PoliticalOutlook { get; set; }

    /// <summary>
    /// 毕业院校
    ///</summary>
    [ExporterHeader(DisplayName = "毕业院校")]
    public string College { get; set; }

    /// <summary>
    /// 学历
    ///</summary>
    [ExporterHeader(DisplayName = "学历")]
    public string Education { get; set; }

    /// <summary>
    /// 学制
    ///</summary>
    [ExporterHeader(DisplayName = "学制")]
    public string EduLength { get; set; }

    /// <summary>
    /// 学位
    ///</summary>
    [ExporterHeader(DisplayName = "学位")]
    public string Degree { get; set; }

    /// <summary>
    /// 家庭电话
    ///</summary>
    [ExporterHeader(DisplayName = "家庭电话")]
    public string HomeTel { get; set; }

    /// <summary>
    /// 办公电话
    ///</summary>
    [ExporterHeader(DisplayName = "办公电话")]
    public string OfficeTel { get; set; }

    /// <summary>
    /// 紧急联系人
    ///</summary>
    [ExporterHeader(DisplayName = "紧急联系人")]
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系人电话
    ///</summary>
    [ExporterHeader(DisplayName = "紧急联系人电话")]
    public string EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系人地址
    ///</summary>
    [ExporterHeader(DisplayName = "紧急联系人地址")]
    public string EmergencyAddress { get; set; }

    /// <summary>
    /// 员工编号
    ///</summary>
    [ExporterHeader(DisplayName = "员工编号")]
    public string EmpNo { get; set; }

    /// <summary>
    /// 入职日期
    ///</summary>
    [ExporterHeader(DisplayName = "入职日期")]
    public string EntryDate { get; set; }

    /// <summary>
    /// 职级
    ///</summary>
    [ExporterHeader(DisplayName = "职级")]
    public string PositionLevel { get; set; }
}

public class RoleSelectorOutPut
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 组织ID
    /// </summary>
    public long OrgId { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; }
}
