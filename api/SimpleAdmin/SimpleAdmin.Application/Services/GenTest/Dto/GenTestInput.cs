using System;
using System.ComponentModel.DataAnnotations;
using Masuit.Tools.Core.Validator;
using Yitter.IdGenerator;

namespace SimpleAdmin.Application;

/// <summary>
/// 测试分页查询参数
/// </summary>
public class GenTestPageInput : BasePageInput
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Sex { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? StartBir { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? EndBir { get; set; }

    /// <summary>
    /// 扩展信息
    /// </summary>
    public string ExtJson { get; set; }

}

/// <summary>
/// 添加测试参数
/// </summary>
public class GenTestAddInput
{
    /// <summary>
    /// 姓名
    /// </summary>
    [Required(ErrorMessage = "Name不能为空")]
    public string Name { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Required(ErrorMessage = "Sex不能为空")]
    public string Sex { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    [Required(ErrorMessage = "Nation不能为空")]
    public string Nation { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [Required(ErrorMessage = "Age不能为空")]
    public int? Age { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [Required(ErrorMessage = "Bir不能为空")]
    public DateTime? Bir { get; set; }

    /// <summary>
    /// SortCode
    /// </summary>
    public int? SortCode { get; set; }

    /// <summary>
    /// 存款
    /// </summary>
    public decimal? Money { get; set; }

    /// <summary>
    /// 扩展信息
    /// </summary>
    public string ExtJson { get; set; }

}

/// <summary>
/// 修改测试参数
/// </summary>
public class GenTestEditInput : GenTestAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [MinValue(1, ErrorMessage = "Id不能为空")]
    public long Id { get; set; }
}

public class GenTestImportInput : BaseImportTemplateInput
{

    /// <summary>
    /// 姓名
    /// </summary>
    [ImporterHeader(Name = "姓名")]
    [Required(ErrorMessage = "学生姓名不能为空")]
    public string Name { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [ImporterHeader(Name = "性别")]
    [Required(ErrorMessage = "学生性别不能为空")]
    //[InDict(DevDictConst.GENDER)]
    public string Sex { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    [ImporterHeader(Name = "年龄")]
    [Range(1, 200, ErrorMessage = "年龄不符合要求")]
    [Required(ErrorMessage = "学生年龄不能为空")]
    public int? Age { get; set; }


    [ImporterHeader(Name = "民族")]
    public string Nation { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [ImporterHeader(Name = "出生日期")]
    [AntTable(IsDate = true)]
    public DateTime? Bir { get; set; }


    /// <summary>
    /// 籍贯 
    ///</summary>
    [ImporterHeader(Name = "籍贯")]
    public string NativePlace { get; set; }
    /// <summary>
    /// 家庭住址 
    ///</summary>
    [ImporterHeader(Name = "家庭住址")]
    [AntTable(Width = 200, Ellipsis = true)]
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
}


