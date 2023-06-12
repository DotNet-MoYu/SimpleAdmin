using System;
using System.ComponentModel.DataAnnotations;
using Masuit.Tools.Core.Validator;
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
  /// 组织机构Id
  /// </summary>
  public long OrgId { get; set; }
}


/// <summary>
/// 添加测试参数
/// </summary>
public class GenTestAddInput
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
  /// 民族
  /// </summary>
  public string Nation { get; set; }
  
  /// <summary>
  /// 年龄
  /// </summary>
  public int? Age { get; set; }
  
  /// <summary>
  /// 生日
  /// </summary>
  public DateTime? Bir { get; set; }
  
  /// <summary>
  /// 排序码
  /// </summary>
  public int? SortCode { get; set; }
  
  /// <summary>
  /// 存款
  /// </summary>
  public decimal? Money { get; set; }
  
}

/// <summary>
/// 修改测试参数
/// </summary>
public class GenTestEditInput : GenTestAddInput
{
    /// <summary>
    /// Id
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public long Id { get; set; }
}


/// <summary>
/// 测试导入
/// </summary>
public class GenTestImportInput : ImportTemplateInput
{
  /// <summary>
  /// 姓名
  /// </summary>
  [ImporterHeader(Name = "姓名")]
  [Required(ErrorMessage = "姓名不能为空")]
  public string Name { get; set; }
  
  /// <summary>
  /// 性别
  /// </summary>
  [ImporterHeader(Name = "性别")]
  [Required(ErrorMessage = "性别不能为空")]
  public string Sex { get; set; }
  
  /// <summary>
  /// 民族
  /// </summary>
  [ImporterHeader(Name = "民族")]
  [Required(ErrorMessage = "民族不能为空")]
  public string Nation { get; set; }
  
  /// <summary>
  /// 年龄
  /// </summary>
  [ImporterHeader(Name = "年龄")]
  [Required(ErrorMessage = "年龄不能为空")]
  public int? Age { get; set; }
  
  /// <summary>
  /// 生日
  /// </summary>
  [ImporterHeader(Name = "生日")]
  [Required(ErrorMessage = "生日不能为空")]
  public DateTime? Bir { get; set; }
  
  /// <summary>
  /// 排序码
  /// </summary>
  [ImporterHeader(Name = "排序码")]
  [Required(ErrorMessage = "排序码不能为空")]
  public int? SortCode { get; set; }
  
  /// <summary>
  /// 存款
  /// </summary>
  [ImporterHeader(Name = "存款")]
  [Required(ErrorMessage = "存款不能为空")]
  public decimal? Money { get; set; }
  
}



