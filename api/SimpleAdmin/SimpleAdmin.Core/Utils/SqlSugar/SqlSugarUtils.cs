
using SimpleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core.Utils
{
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
                        tables.Add(new SqlSugarTableInfo
                        {
                            TableName = entityInfo.DbTableName,
                            EntityName = entityInfo.EntityName,
                            TableDescription = entityInfo.TableDescription,
                            ConfigId = configId
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

    }
}
