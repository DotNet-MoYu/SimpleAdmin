namespace SimpleAdmin.Core;

/// <summary>
///  SqlSugar设置启动
/// </summary>
public static class SqlsugarSetup
{
    /// <summary>
    /// 注入Sqlsugar
    /// </summary>
    /// <param name="services"></param>
    public static void AddSqlSugar(this IServiceCollection services)
    {

        //services.AddSingleton<ISqlSugarClient>(DbContext.Db); // 单例注册,不用工作单元不需要注入
        //services.AddUnitOfWork<SqlSugarUnitOfWork>(); // 事务与工作单元注册

        //检查ConfigId
        CheckSameConfigId();

    }

    /// <summary>
    /// 检查是否有相同的ConfigId
    /// </summary>
    /// <returns></returns>
    private static void CheckSameConfigId()
    {
        var configIdGroup = DbContext.DbConfigs.GroupBy(it => it.ConfigId).ToList();
        foreach (var configId in configIdGroup)
        {
            if (configId.ToList().Count > 1) throw Oops.Oh($"Sqlsugar连接配置ConfigId:{configId.Key}重复了");
        }
    }
}
