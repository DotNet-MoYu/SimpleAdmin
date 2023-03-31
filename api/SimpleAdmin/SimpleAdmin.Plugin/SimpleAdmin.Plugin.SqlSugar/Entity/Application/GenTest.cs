

namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// 代码生成基础
///</summary>
[SugarTable("gen_test", TableDescription = "代码生成测试")]
[Tenant(SqlsugarConst.DB_Default)]
[CodeGen]
[BatchEdit]
public class GenTest : DataEntityBase
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

    public int Age { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime Bir { get; set; }


    /// <summary>
    /// 存款
    /// </summary>
    public decimal Money { get; set; }


    /// <summary>
    /// 排序码 
    ///</summary>
    public int SortCode { get; set; }


}
