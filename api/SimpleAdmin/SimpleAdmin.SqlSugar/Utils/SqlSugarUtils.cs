// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// SqlSugar通用功能
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
        var tables = new List<SqlSugarTableInfo>();//结果集

        // 获取实体表
        var entityTypes = App.EffectiveTypes
            .Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false))//有SugarTable特性
            .Where(u => u.IsDefined(typeof(T), false));//具有指定特性

        foreach (var entityType in entityTypes)
        {
            var tenantAttr = entityType.GetCustomAttribute<TenantAttribute>();//获取多租户特性
            var configId = tenantAttr.configId.ToString();//获取租户Id
            var connection = DbContext.DB.GetConnection(tenantAttr.configId.ToString());//根据租户ID获取连接信息
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
        return tables;
    }

    /// <summary>
    /// 获取字段信息
    /// </summary>
    /// <param name="configId"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static List<SqlSugarColumnInfo> GetTableColumns(string configId, string tableName)
    {
        var columns = new List<SqlSugarColumnInfo>();//结果集
        var connection = DbContext.DB.GetConnection(configId);
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
                columns.Add(new SqlSugarColumnInfo
                {
                    ColumnName = it.DbColumnName,
                    IsPrimaryKey = it.IsPrimarykey,
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
            "int" => EffTypeConst.INPUT_NUMBER,
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
        var columnList = new List<string>
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
