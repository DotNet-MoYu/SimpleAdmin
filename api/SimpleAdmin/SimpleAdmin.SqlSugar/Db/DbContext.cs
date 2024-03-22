// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using DbType = SqlSugar.DbType;

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// 数据库上下文对象
/// </summary>
public static class DbContext
{
    /// <summary>
    /// 读取配置文件中的 ConnectionStrings:SqlSugar 配置节点
    /// </summary>
    public static readonly List<SqlSugarConfig> DB_CONFIGS = App.GetConfig<List<SqlSugarConfig>>("SqlSugarSettings:ConnectionStrings");

    /// <summary>
    /// SqlSugar 数据库实例
    /// </summary>
    public static readonly SqlSugarScope DB = new SqlSugarScope(DB_CONFIGS.Adapt<List<ConnectionConfig>>(), db =>
    {
        //遍历配置的数据库
        DB_CONFIGS.ForEach(it =>
        {
            var sqlSugarScope = db.GetConnectionScope(it.ConfigId);//获取当前库
            MoreSetting(sqlSugarScope, it.DbType);//更多设置
            ExternalServicesSetting(sqlSugarScope, it);//实体拓展配置
            AopSetting(sqlSugarScope);//aop配置
            FilterSetting(sqlSugarScope);//过滤器配置
        });
    });

    /// <summary>
    /// 实体拓展配置,自定义类型多库兼容
    /// </summary>
    /// <param name="db"></param>
    /// <param name="config"></param>
    private static void ExternalServicesSetting(SqlSugarScopeProvider db, SqlSugarConfig config)
    {
        db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices
        {
            // 处理表
            EntityNameService = (type, entity) =>
            {
                if (config.IsUnderLine && !entity.DbTableName.Contains('_'))
                    entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName);// 驼峰转下划线
            },
            //自定义类型多库兼容
            EntityService = (c, p) =>
            {
                //如果是mysql并且是varchar(max) 已弃用
                //if (config.DbType == SqlSugar.DbType.MySql && (p.DataType == SqlSugarConst.NVarCharMax))
                //{
                //    p.DataType = SqlSugarConst.LongText;//转成mysql的longtext
                //}
                //else if (config.DbType == SqlSugar.DbType.Sqlite && (p.DataType == SqlSugarConst.NVarCharMax))
                //{
                //    p.DataType = SqlSugarConst.Text;//转成sqlite的text
                //}
                //默认不写IsNullable为非必填
                //if (new NullabilityInfoContext().Create(c).WriteState is NullabilityState.Nullable)
                //    p.IsNullable = true;
                if (config.IsUnderLine && !p.IsIgnore && !p.DbColumnName.Contains('_'))
                    p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName);// 驼峰转下划线
            }
        };
    }

    /// <summary>
    /// Aop设置
    /// </summary>
    /// <param name="db"></param>
    public static void AopSetting(SqlSugarScopeProvider db)
    {
        var config = db.CurrentConnectionConfig;

        // 设置超时时间
        db.Ado.CommandTimeOut = 30;

        // 打印SQL语句
        db.Aop.OnLogExecuting = (sql, pars) =>
        {
            //如果不是开发环境就打印sql
            if (App.HostEnvironment.IsDevelopment())
            {
                if (sql.StartsWith("SELECT"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    WriteSqlLog($"查询{config.ConfigId}库操作");
                }
                if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    WriteSqlLog($"修改{config.ConfigId}库操作");
                }
                if (sql.StartsWith("DELETE"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    WriteSqlLog($"删除{config.ConfigId}库操作");
                }
                Console.WriteLine(UtilMethods.GetSqlString(config.DbType, sql, pars));
                WriteSqlLog($"{config.ConfigId}库操作结束");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
            }
        };
        //异常
        db.Aop.OnError = ex =>
        {
            //如果不是开发环境就打印日志
            if (App.WebHostEnvironment.IsDevelopment())
            {
                if (ex.Parametres == null) return;
                Console.ForegroundColor = ConsoleColor.Red;
                var pars = db.Utilities.SerializeObject(((SugarParameter[])ex.Parametres).ToDictionary(it => it.ParameterName, it => it.Value));
                WriteSqlLog($"{config.ConfigId}库操作异常");
                Console.WriteLine(UtilMethods.GetSqlString(config.DbType, ex.Sql, (SugarParameter[])ex.Parametres) + "\r\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
        };
        //插入和更新过滤器
        db.Aop.DataExecuting = (oldValue, entityInfo) =>
        {
            // 新增操作
            if (entityInfo.OperationType == DataFilterType.InsertByObject)
            {
                // 主键(long类型)且没有值的---赋值雪花Id
                if (entityInfo.EntityColumnInfo.IsPrimarykey && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(long))
                {
                    var id = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
                    if (id == null || (long)id == 0)
                        entityInfo.SetValue(CommonUtils.GetSingleId());
                }
                if (entityInfo.PropertyName == nameof(BaseEntity.CreateTime))
                    entityInfo.SetValue(DateTime.Now);

                if (App.User != null)
                {
                    //创建人和创建机构ID
                    if (entityInfo.PropertyName == nameof(BaseEntity.CreateUserId))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.USER_ID)?.Value);
                    if (entityInfo.PropertyName == nameof(BaseEntity.CreateUser))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.ACCOUNT)?.Value);
                    if (entityInfo.PropertyName == nameof(DataEntityBase.CreateOrgId))
                        entityInfo.SetValue(App.User.FindFirst(ClaimConst.ORG_ID)?.Value);
                }
            }
            // 更新操作
            if (entityInfo.OperationType == DataFilterType.UpdateByObject)
            {
                //更新时间
                if (entityInfo.PropertyName == nameof(BaseEntity.UpdateTime))
                    entityInfo.SetValue(DateTime.Now);
                //更新人
                if (App.User != null)
                {
                    if (entityInfo.PropertyName == nameof(BaseEntity.UpdateUserId))
                        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.USER_ID)?.Value);
                    if (entityInfo.PropertyName == nameof(BaseEntity.UpdateUser))
                        entityInfo.SetValue(App.User?.FindFirst(ClaimConst.ACCOUNT)?.Value);
                }
            }
        };

        //查询数据转换
        db.Aop.DataExecuted = (value, entity) =>
        {
        };
    }

    /// <summary>
    /// 实体更多配置
    /// </summary>
    /// <param name="db">db</param>
    /// <param name="dbType">数据库类型</param>
    private static void MoreSetting(SqlSugarScopeProvider db, DbType dbType)
    {
        db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings
        {
            SqlServerCodeFirstNvarchar = dbType == DbType.SqlServer//设置默认nvarchar
        };
    }

    /// <summary>
    /// 过滤器设置
    /// </summary>
    /// <param name="db"></param>
    public static void FilterSetting(SqlSugarScopeProvider db)
    {
        // 假删除过滤器
        //LogicDeletedEntityFilter(db);
    }

    /// <summary>
    /// 假删除过滤器
    /// </summary>
    /// <param name="db"></param>
    private static void LogicDeletedEntityFilter(SqlSugarScopeProvider db)
    {
    }

    private static void WriteSqlLog(string msg)
    {
        Console.WriteLine($"=============={msg}==============");
    }
}
