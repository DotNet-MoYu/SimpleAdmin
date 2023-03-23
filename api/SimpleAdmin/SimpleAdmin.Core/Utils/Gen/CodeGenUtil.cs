namespace SimpleAdmin.Core.Utils;

/// <summary>
/// 代码生成工具类
/// </summary>
public class CodeGenUtil
{
    /// <summary>
    /// 数据库字段类型转.NET类型
    /// </summary>
    /// <param name="dataType">字段类型</param>
    /// <returns></returns>
    public static string ConvertDataType(string dataType)
    {

        switch (dataType)
        {
            case "text":
            case "varchar":
            case "char":
            case "nvarchar":
            case "nchar":
            case "blob":
            case "longtext":
            case "nclob":
                return "string";

            case "int":
            case "mediumint":
                return "int";

            case "smallint":
                return "Int16";

            case "tinyint":
                return "byte";

            case "bigint":
            case "integer"://sqlite数据库
                return "long";

            case "bit":
            case "boolean":
                return "bool";

            case "money":
            case "smallmoney":
            case "numeric":
            case "decimal":
                return "decimal";

            case "real":
                return "Single";

            case "datetime":
            case "smalldatetime":
            case "timestamp":
            case "date":
            case "year":
            case "time":
                return "DateTime";

            case "float":
            case "double":
                return "double";

            case "image":
            case "binary":
            case "varbinary":
                return "byte[]";

            case "uniqueidentifier":
                return "Guid";

            default:
                return "string";
        }
    }


    /// <summary>
    /// 数据类型转显示类型
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static string DataTypeToEff(string dataType)
    {
        return dataType switch
        {
            "string" => GenConst.INPUT,
            "int" => GenConst.INPUTNUMBER,
            "long" => GenConst.INPUT,
            "float" => GenConst.INPUT,
            "double" => GenConst.INPUT,
            "decimal" => GenConst.INPUT,
            "bool" => GenConst.SWITCH,
            "Guid" => GenConst.INPUT,
            "DateTime" => GenConst.DATEPICKER,
            _ => GenConst.INPUT,
        };
    }

    /// <summary>
    /// 是否通用字段
    /// </summary>
    /// <param name="columnName">字段名</param>
    /// <returns></returns>
    public static bool IsCommonColumn(string columnName)
    {
        var columnList = new List<string>()
        {
        nameof(BaseEntity.CreateTime) , nameof(BaseEntity.UpdateTime),
        nameof(BaseEntity.CreateUserId),nameof(BaseEntity.CreateUser),
        nameof(BaseEntity.UpdateUserId), nameof(BaseEntity.UpdateUser),
        nameof(BaseEntity.IsDelete),nameof(DataEntityBase.CreateOrgId),
        nameof(PrimaryKeyEntity.ExtJson)
        };
        return columnList.Contains(columnName);
    }
}
