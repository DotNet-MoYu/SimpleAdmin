using NewLife.Serialization;
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
        if (options.InitTable)//如果需要初始化表结构
        {
            InitTable(assemblyName);
        }
        if (options.InitSeedData)
        {
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
        var entityTypes = App.EffectiveTypes.Where(u => !u.IsInterface && !u.IsAbstract && u.IsClass && u.IsDefined(typeof(SugarTable), false) && u.Assembly.FullName == assemblyName);
        if (!entityTypes.Any()) return;//没有就退出
        foreach (var entityType in entityTypes)
        {
            var tenantAtt = entityType.GetCustomAttribute<TenantAttribute>();//获取Sqlsugar多租户特性
            var ignoreInit = entityType.GetCustomAttribute<IgnoreInitTableAttribute>();//获取忽略初始化特性
            if (ignoreInit != null) continue;//如果有忽略初始化特性
            if (tenantAtt == null) continue;//如果没有租户特性就下一个
            var db = DbContext.Db.GetConnectionScope(tenantAtt.configId.ToString());//获取数据库对象
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
            var tenantAtt = entityType.GetCustomAttribute<TenantAttribute>();//获取sqlsugar租户特性
            if (tenantAtt == null) continue;//如果没有租户特性就下一个
            var db = DbContext.Db.GetConnectionScope(tenantAtt.configId.ToString());//获取数据库对象
            var config = DbContext.DbConfigs.FirstOrDefault(u => u.ConfigId == tenantAtt.configId.ToString());//获取数据库配置
            var seedDataTable = seedData.ToList().ToDataTable();//获取种子数据
            seedDataTable.TableName = db.EntityMaintenance.GetEntityInfo(entityType).DbTableName;//获取表名
            if (config.IsUnderLine)// 驼峰转下划线
            {
                foreach (DataColumn col in seedDataTable.Columns)
                {
                    col.ColumnName = UtilMethods.ToUnderLine(col.ColumnName);
                }
            }
            var ignoreAdd = hasDataMethod.GetCustomAttribute<IgnoreSeedDataAddAttribute>();//读取忽略插入特性
            var ignoreUpdate = hasDataMethod.GetCustomAttribute<IgnoreSeedDataUpdateAttribute>();//读取忽略更新特性
            if (seedDataTable.Columns.Contains(SqlsugarConst.DB_PrimaryKey))//判断种子数据是否有主键
            {
                //根据判断主键插入或更新
                var storage = db.Storageable(seedDataTable).WhereColumns(SqlsugarConst.DB_PrimaryKey).ToStorage();
                if (ignoreAdd == null) storage.AsInsertable.ExecuteCommand();//执行插入
                if (ignoreUpdate == null) storage.AsUpdateable.ExecuteCommand();//只有没有忽略更新的特性才执行更新
            }
            else// 没有主键或者不是预定义的主键(有重复的可能)
            {
                //全量插入
                var storage = db.Storageable(seedDataTable).ToStorage();
                if (ignoreAdd == null) storage.AsInsertable.ExecuteCommand();
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
            type = type.BaseType;
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
                object[] array = tempList.ToArray();
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
        var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsIgnore == true);
        return sc != null;
    }


    private static bool IsJsonColumn(PropertyInfo pi)
    {
        var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsJson == true);
        return sc != null;
    }
}