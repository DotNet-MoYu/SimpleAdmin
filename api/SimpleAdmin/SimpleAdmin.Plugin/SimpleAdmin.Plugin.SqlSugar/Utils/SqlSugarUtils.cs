namespace SimpleAdmin.Plugin.SqlSugar;

/// <summary>
/// Sqlusgar通用功能
/// </summary>
public static class SqlSugarUtils
{
    /// <summary>
    /// 根据特性获取所有表信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<SqlSugarTableInfo> GetTablesByAttribute<T>()
    {
        List<SqlSugarTableInfo> tables = new List<SqlSugarTableInfo>();//结果集

        // 获取实体表
        var entityTypes = App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false))//有SugarTable特性
            .Where(u => u.IsDefined(typeof(T), false));//具有代码生成特性

        foreach (var entityType in entityTypes)
        {
            var teanant = entityType.GetCustomAttribute<TenantAttribute>();//获取多租户特性
            var configId = teanant.configId.ToString();//获取租户Id
            if (teanant != null)
            {
                var connection = DbContext.Db.GetConnection(teanant.configId.ToString());//根据租户ID获取连接信息
                var entityInfo = connection.EntityMaintenance.GetEntityInfo(entityType);//获取实体信息
                if (entityInfo != null)
                {
                    var columns = GetTableColumns(configId, entityInfo.DbTableName);//获取字段信息
                    tables.Add(new SqlSugarTableInfo
                    {
                        TableName = entityInfo.DbTableName,
                        EntityName = entityInfo.EntityName,
                        TableDescription = entityInfo.TableDescription,
                        ConfigId = configId,
                        Columns = columns
                    });
                }
            }
        }
        return tables;
    }

    /// <summary>
    /// 获取字段信息
    /// </summary>
    /// <param name="configId"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static List<SqlsugarColumnInfo> GetTableColumns(string configId, string tableName)
    {
        List<SqlsugarColumnInfo> columns = new List<SqlsugarColumnInfo>();//结果集
        var connection = DbContext.Db.GetConnection(configId);
        var dbColumnInfos = connection.DbMaintenance.GetColumnInfosByTableName(tableName);//根据表名获取表信息
        if (dbColumnInfos != null)
        {
            //遍历字段获取信息
            dbColumnInfos.ForEach(it =>
            {
                if (it.DbColumnName.Contains("_"))//如果有下划线,转换一下
                {
                    var column = "";//新的字段值
                    var columnList = it.DbColumnName.Split('_');//根据下划线分割
                    columnList.ForEach(it =>
                    {
                        column += StringHelper.FirstCharToUpper(it);//首字母大写
                    });
                    it.DbColumnName = column;//赋值给数据库字段
                }
                else
                {
                    it.DbColumnName = StringHelper.FirstCharToUpper(it.DbColumnName);//首字母大写
                }
                columns.Add(new SqlsugarColumnInfo
                {
                    ColumnName = it.DbColumnName,
                    IsPrimarykey = it.IsPrimarykey,
                    ColumnDescription = it.ColumnDescription,
                    DataType = it.DataType
                });
            });
        }
        return columns;
    }

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
            "string" => EffTypeConst.INPUT,
            "int" => EffTypeConst.INPUTNUMBER,
            "long" => EffTypeConst.INPUT,
            "float" => EffTypeConst.INPUT,
            "double" => EffTypeConst.INPUT,
            "decimal" => EffTypeConst.INPUT,
            "bool" => EffTypeConst.SWITCH,
            "Guid" => EffTypeConst.INPUT,
            "DateTime" => EffTypeConst.DATEPICKER,
            _ => EffTypeConst.INPUT
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
            nameof(BaseEntity.CreateTime), nameof(BaseEntity.UpdateTime),
            nameof(BaseEntity.CreateUserId), nameof(BaseEntity.CreateUser),
            nameof(BaseEntity.UpdateUserId), nameof(BaseEntity.UpdateUser),
            nameof(BaseEntity.IsDelete), nameof(DataEntityBase.CreateOrgId),
            nameof(PrimaryKeyEntity.ExtJson)
        };
        return columnList.Contains(columnName);
    }
}