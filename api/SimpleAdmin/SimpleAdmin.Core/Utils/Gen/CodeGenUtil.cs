namespace SimpleAdmin.Core.Utils
{
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
                    return "String";

                case "int":
                case "mediumint":
                    return "Int";

                case "smallint":
                    return "Int16";

                case "tinyint":
                    return "Byte";

                case "bigint":
                case "integer"://sqlite数据库
                    return "Long";

                case "bit":
                case "boolean":
                    return "Bool";

                case "money":
                case "smallmoney":
                case "numeric":
                case "decimal":
                    return "Decimal";

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
                    return "Double";

                case "image":
                case "binary":
                case "varbinary":
                    return "Byte[]";

                case "uniqueidentifier":
                    return "Guid";

                default:
                    return "String";
            }
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
}
