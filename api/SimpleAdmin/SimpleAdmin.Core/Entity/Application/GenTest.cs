namespace SimpleAdmin.Core;

/// <summary>
/// 代码生成基础
///</summary>
[SugarTable("gen_test", TableDescription = "代码生成测试")]
[Tenant(SqlsugarConst.DB_Default)]
[CodeGen]
public class GenTest : DataEntityBase
{

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }


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

}
