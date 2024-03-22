// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

using System.Collections;

namespace SimpleAdmin.SqlSugar;

/// <summary>
/// CodeFirst功能类
/// </summary>
[SuppressSniffer]
public static class CodeFirstUtils
{
    /// <summary>
    ///  CodeFirst生成数据库表结构和种子数据
    /// </summary>
    /// <param name="options">codefirst选项</param>
    /// <param name="assemblyName">程序集名称</param>
    public static void CodeFirst(BaseOptions options, string assemblyName)
    {
        var appName = assemblyName.Split(",")[0];
        if (options.InitTable)//如果需要初始化表结构
        {
            Console.WriteLine($"开始初始化{appName}数据库表结构");
            InitTable(assemblyName);
        }
        if (options.InitSeedData)
        {
            Console.WriteLine($"开始初始化{appName}数据库种子数据");
            InitSeedData(assemblyName);
        }
    }

    /// <summary>
    /// 初始化数据库表结构
    /// </summary>
    /// <param name="assemblyName">程序集名称</param>
    private static void InitTable(string assemblyName)
    {
        // 获取所有实体表-初始化表结构
        var entityTypes = App.EffectiveTypes.Where(u =>
            !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.Assembly.FullName == assemblyName);
        if (!entityTypes.Any()) return;//没有就退出
        foreach (var entityType in entityTypes)
        {
            var tenantAtt = entityType.GetCustomAttribute<TenantAttribute>();//获取SqlSugar多租户特性
            var ignoreInit = entityType.GetCustomAttribute<IgnoreInitTableAttribute>();//获取忽略初始化特性
            if (ignoreInit != null) continue;//如果有忽略初始化特性
            if (tenantAtt == null) continue;//如果没有租户特性就下一个
            var db = DbContext.DB.GetConnectionScope(tenantAtt.configId.ToString());//获取数据库对象
            var splitTable = entityType.GetCustomAttribute<SplitTableAttribute>();//获取自动分表特性
            if (splitTable == null)//如果特性是空
                db.CodeFirst.InitTables(entityType);//普通创建
            else
                db.CodeFirst.SplitTables().InitTables(entityType);//自动分表创建
        }
    }

    /// <summary>
    /// 初始化种子数据
    /// </summary>
    /// <param name="assemblyName">程序集名称</param>
    private static void InitSeedData(string assemblyName)
    {
        // 获取所有种子配置-初始化数据
        var seedDataTypes = App.EffectiveTypes.Where(u => !u.IsInterface && u is { IsAbstract: false, IsClass: true }
            && u.GetInterfaces().Any(i => i.HasImplementedRawGeneric(typeof(ISqlSugarEntitySeedData<>))) && u.Assembly.FullName == assemblyName);
        if (!seedDataTypes.Any()) return;
        foreach (var seedType in seedDataTypes)//遍历种子类
        {
            //使用与指定参数匹配程度最高的构造函数来创建指定类型的实例。
            var instance = Activator.CreateInstance(seedType);
            //获取SeedData方法
            var hasDataMethod = seedType.GetMethod("SeedData");
            //判断是否有种子数据
            var seedData = ((IEnumerable)hasDataMethod?.Invoke(instance, null))?.Cast<object>();
            if (seedData == null) continue;//没有种子数据就下一个
            var entityType = seedType.GetInterfaces().First().GetGenericArguments().First();//获取实体类型
            var tenantAtt = entityType.GetCustomAttribute<TenantAttribute>();//获取SqlSugar租户特性
            if (tenantAtt == null) continue;//如果没有租户特性就下一个
            var db = DbContext.DB.GetConnectionScope(tenantAtt.configId.ToString());//获取数据库对象
            var config = DbContext.DB_CONFIGS.FirstOrDefault(u => u.ConfigId == tenantAtt.configId.ToString());//获取数据库配置
            // var seedDataTable = seedData.ToList().ToDataTable();//获取种子数据:已弃用
            var entityInfo = db.EntityMaintenance.GetEntityInfo(entityType);
            // seedDataTable.TableName = db.EntityMaintenance.GetEntityInfo(entityType).DbTableName;//获取表名
            var ignoreAdd = hasDataMethod.GetCustomAttribute<IgnoreSeedDataAddAttribute>();//读取忽略插入特性
            var ignoreUpdate = hasDataMethod.GetCustomAttribute<IgnoreSeedDataUpdateAttribute>();//读取忽略更新特性
            if (entityInfo.Columns.Any(u => u.IsPrimarykey))//判断种子数据是否有主键
            {
                // 按主键进行批量增加和更新
                var storage = db.StorageableByObject(seedData.ToList()).ToStorage();
                if (ignoreAdd == null) storage.AsInsertable.ExecuteCommand();//执行插入
                if (ignoreUpdate == null) storage.AsUpdateable.ExecuteCommand();//只有没有忽略更新的特性才执行更新
            }
            else// 没有主键或者不是预定义的主键(有重复的可能)
            {
                //全量插入
                // 无主键则只进行插入
                if (!db.Queryable(entityInfo.DbTableName, entityInfo.DbTableName).Any() && ignoreAdd == null)
                    db.InsertableByObject(seedData.ToList()).ExecuteCommand();
            }
        }
    }

    /// <summary>
    /// 判断类型是否实现某个泛型
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="generic">泛型类型</param>
    /// <returns>bool</returns>
    public static bool HasImplementedRawGeneric(this Type type, Type generic)
    {
        // 检查接口类型
        var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
        if (isTheRawGenericType) return true;

        // 检查类型
        while (type != null && type != typeof(object))
        {
            isTheRawGenericType = IsTheRawGenericType(type);
            if (isTheRawGenericType) return true;
            type = type.BaseType ?? default;
        }

        return false;

        // 判断逻辑
        bool IsTheRawGenericType(Type type) => generic == (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
    }

    /// <summary>
    /// List转DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static DataTable ToDataTable<T>(this List<T> list)
    {
        var result = new DataTable();
        if (list.Count > 0)
        {
            // result.TableName = list[0].GetType().Name; // 表名赋值
            var propertys = list[0].GetType().GetProperties();
            foreach (var pi in propertys)
            {
                var colType = pi.PropertyType;
                if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    colType = colType.GetGenericArguments()[0];
                }
                if (IsIgnoreColumn(pi))
                    continue;
                if (IsJsonColumn(pi))//如果是json特性就是sting类型
                    colType = typeof(string);
                result.Columns.Add(pi.Name, colType);
            }
            for (var i = 0; i < list.Count; i++)
            {
                var tempList = new ArrayList();
                foreach (var pi in propertys)
                {
                    if (IsIgnoreColumn(pi))
                        continue;
                    var obj = pi.GetValue(list[i], null);
                    if (IsJsonColumn(pi))//如果是json特性就是转化为json格式
                        obj = obj?.ToJson();//如果json字符串是空就传null
                    tempList.Add(obj);
                }
                var array = tempList.ToArray();
                result.LoadDataRow(array, true);
            }
        }
        return result;
    }

    /// <summary>
    /// 排除SqlSugar忽略的列
    /// </summary>
    /// <param name="pi"></param>
    /// <returns></returns>
    private static bool IsIgnoreColumn(PropertyInfo pi)
    {
        var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsIgnore);
        return sc != null;
    }

    private static bool IsJsonColumn(PropertyInfo pi)
    {
        var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsJson);
        return sc != null;
    }
}
