// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.Aop;

/// <summary>
/// Aop
/// 这里没有继承IGlobalDispatchProxy是因为IGlobalDispatchProxy会把一些没有必要的方法也aop了
/// </summary>
public class GlobalDispatchProxy : AspectDispatchProxy, IDispatchProxy
{
    /// <summary>
    /// 当前服务实例
    /// </summary>
    public object Target { get; set; }

    /// <summary>
    /// 服务提供器，可以用来解析服务，如：Services.GetService()
    /// </summary>
    public IServiceProvider Services { get; set; }

    /// <summary>
    /// 方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override object Invoke(MethodInfo method, object[] args)
    {
        //如果不带返回值
        if (method.ReturnType == typeof(void))
        {
            After(method, null, args);
            return method.Invoke(Target, args);//直接返回
        }
        var result = Before(method, args);//方法执行之前判断是否有缓存的数据
        if (result == null) result = method.Invoke(Target, args);//如果没有缓存就执行方法返回数据
        After(method, result, args);//方法执行之后干的事
        return result;//返回结果
    }

    /// <summary>
    /// 异步无返回值
    /// </summary>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task InvokeAsync(MethodInfo method, object[] args)
    {
        var task = method.Invoke(Target, args) as Task;
        await task;
        After(method, null, args);
    }

    /// <summary>
    /// 异步带返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
    {
        var result = Before(method, args);//方法执行之前判断是否有缓存的数据
        if (result == null)
        {
            var taskT = method.Invoke(Target, args) as Task<T>;
            result = await taskT;//如果没有缓存就执行方法返回数据
        }
        After(method, result, args);//方法执行之后干的事
        return result;//返回结果
    }

    /// <summary>
    /// 方法执行之后
    /// </summary>
    /// <param name="method">方法</param>
    /// <param name="returnValue">返回值</param>
    /// <param name="args">参数列表</param>
    private void After(MethodInfo method, object returnValue, object[] args)
    {
        RecordCache(method, args, returnValue);//记录返回值到缓存
    }

    /// <summary>
    /// 方法执行之前
    /// </summary>
    /// <param name="method">方法</param>
    /// <param name="args">参数列表</param>
    /// <returns></returns>
    private dynamic Before(MethodInfo method, object[] args)
    {
        var cacheData = CheckCache(method, args);//检查缓存
        if (cacheData != null) { return cacheData; }//如果缓存有数据直接返回
        return null;//默认返回空
    }

    /// <summary>
    /// 检查缓存里是否有数据
    /// </summary>
    /// <param name="method">方法</param>
    /// <param name="args">参数列表</param>
    private dynamic CheckCache(MethodInfo method, object[] args)
    {
        var cacheAttribute = method.GetActualCustomAttribute<CacheAttribute>(Target);//读取缓存特性
        //判断需不需要读取缓存
        if (cacheAttribute != null)
        {
            var redisManager = Services.GetService<ISimpleCacheService>();// 获取redis服务
            var cacheKey = cacheAttribute.CustomKeyValue
                ?? CustomCacheKey(cacheAttribute.KeyPrefix, method, args);//如果redisKey值，如果有自定义值就用自定义Key，否则以前缀+系统自动生成的Key
            string cacheValue;
            if (cacheAttribute.StoreType == CacheConst.CACHE_HASH)//如果存的是Hash值
            {
                cacheValue = redisManager.HashGet<string>(cacheKey, args[0].ToString())[0];//从redis获取Hash数据取第一个,注意是 string 类型
            }
            else
            {
                cacheValue = redisManager.Get<string>(cacheKey);//注意是 string 类型，方法GetValue
            }
            if (!string.IsNullOrEmpty(cacheValue))//如果返回值不是空
            {
                //if (cacheAttribute.IsDelete)//判断是否是删除操作
                //{
                //    if (cacheAttribute.StoreType == RedisConst.Cache_Hash)//如果是Hash
                //    {
                //        _redisManager.HashDel<string>(cacheKey, new string[] { args[0].ToString() });
                //    }
                //    else
                //    {
                //        //删除Redis整个KEY
                //        _redisManager.Remove(cacheKey);
                //    }

                //}
                //将当前获取到的缓存值，赋值给当前执行方法
                Type returnType;
                if (typeof(Task).IsAssignableFrom(method.ReturnType))
                {
                    returnType = method.ReturnType.GenericTypeArguments.FirstOrDefault();
                }
                else
                {
                    returnType = method.ReturnType;
                }
                if (!returnType.IsPrimitive && returnType != typeof(string))//判断返回类型是否原生类型并且不是string
                {
                    dynamic result = JsonConvert.DeserializeObject(cacheValue, returnType);//序列化数据
                    return result;
                }
                return cacheValue;
            }
        }
        return null;
    }

    /// <summary>
    /// 写数据到缓存
    /// </summary>
    /// <param name="method">方法</param>
    /// <param name="args">参数列表</param>
    /// <param name="returnValue">返回值</param>
    private void RecordCache(MethodInfo method, object[] args, object returnValue)
    {
        var cacheAttribute = method.GetActualCustomAttribute<CacheAttribute>(Target);
        if (cacheAttribute != null)
        {
            //获取自定义缓存键
            var cacheKey = cacheAttribute.CustomKeyValue ?? CustomCacheKey(cacheAttribute.KeyPrefix, method, args);
            if (!string.IsNullOrWhiteSpace(cacheKey))//如果有key
            {
                var redisManager = Services.GetService<ISimpleCacheService>();// 获取redis服务
                if (cacheAttribute.IsDelete)//判断是否是删除操作
                {
                    //删除Redis整个KEY
                    redisManager.Remove(cacheKey);
                }
                else
                {
                    if (returnValue == null) { return; }
                    if (cacheAttribute.StoreType == CacheConst.CACHE_HASH)//如果是hash类型的
                    {
                        //插入到hash，这里规定是方法的第一个参数
                        redisManager.HashAdd(cacheKey, args[0].ToString(), returnValue);
                    }
                    else
                    {
                        if (cacheAttribute.AbsoluteExpiration != null)//如果有超时时间
                        {
                            redisManager.Set(cacheKey, returnValue, cacheAttribute.AbsoluteExpiration.Value);//插入redis
                        }
                        else
                        {
                            redisManager.Set(cacheKey, returnValue);//插入redis
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 自定义缓存Key
    /// </summary>
    /// <param name="prefix">前缀</param>
    /// <param name="method">方法</param>
    /// <param name="args">参数</param>
    /// <returns></returns>
    protected string CustomCacheKey(string prefix, MethodInfo method, object[] args)
    {
        var key = prefix;//前缀
        var methodArguments = args.Select(GetCacheKey.GetArgumentValue).Take(3).ToList();//获取参数列表，最多三个
        if (!string.IsNullOrEmpty(key))//如果制定了前缀
        {
            foreach (var param in methodArguments)//遍历参数列表
            {
                key = $"{key}{param}:";//生成KEY
            }
        }
        else
        {
            var typeName = Target.GetType().Name;//获取实例名
            var methodName = method.Name;//获取方法名
            key = $"{CacheConst.CACHE_PREFIX_WEB}{typeName}:{methodName}:";//生成Key
            foreach (var param in methodArguments)//遍历参数列表
            {
                key = $"{key}{param}:";//生成加上参数的KEY
            }
        }
        return key.TrimEnd(':');
    }
}

/// <summary>
/// 获取缓存Key
/// </summary>
internal static class GetCacheKey
{
    /// <summary>
    /// object 转 string
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string GetArgumentValue(object arg)
    {
        if (arg is DateTime || arg is DateTime)
            return ((DateTime)arg).ToString("yyyyMMddHHmmss");

        if (arg is string || arg is ValueType || arg is Nullable)
            return arg.ToString();

        if (arg != null)
        {
            if (arg is Expression)
            {
                var obj = arg as Expression;
                var result = Resolve(obj);
                return MD5Encryption.Encrypt(result);
            }
            if (arg.GetType().IsClass)
            {
                return MD5Encryption.Encrypt(JsonConvert.SerializeObject(arg));
            }
        }
        return string.Empty;
    }

    private static string Resolve(Expression expression)
    {
        if (expression is LambdaExpression)
        {
            var lambda = expression as LambdaExpression;
            expression = lambda.Body;
            return Resolve(expression);
        }
        if (expression is BinaryExpression)
        {
            var binary = expression as BinaryExpression;
            if (binary.Left is MemberExpression && binary.Right is ConstantExpression)//解析x=>x.Name=="123" x.Age==123这类
                return ResolveFunc(binary.Left, binary.Right, binary.NodeType);
            if (binary.Left is MethodCallExpression && binary.Right is ConstantExpression)//解析x=>x.Name.Contains("xxx")==false这类的
            {
                var value = (binary.Right as ConstantExpression).Value;
                return ResolveLinqToObject(binary.Left, value, binary.NodeType);
            }
            if (binary.Left is MemberExpression && binary.Right is MemberExpression
                || binary.Left is MemberExpression && binary.Right is UnaryExpression)//解析x=>x.Date==DateTime.Now这种
            {
                var lambda = Expression.Lambda(binary.Right);
                var fn = lambda.Compile();
                var value = Expression.Constant(fn.DynamicInvoke(null), binary.Right.Type);
                return ResolveFunc(binary.Left, value, binary.NodeType);
            }
        }
        if (expression is UnaryExpression)
        {
            var unary = expression as UnaryExpression;
            if (unary.Operand is MethodCallExpression)//解析!x=>x.Name.Contains("xxx")或!array.Contains(x.Name)这类
                return ResolveLinqToObject(unary.Operand, false);
            if (unary.Operand is MemberExpression && unary.NodeType == ExpressionType.Not)//解析x=>!x.isDeletion这样的
            {
                var constant = Expression.Constant(false);
                return ResolveFunc(unary.Operand, constant, ExpressionType.Equal);
            }
        }
        if (expression is MemberExpression && expression.NodeType == ExpressionType.MemberAccess)//解析x=>x.isDeletion这样的
        {
            var member = expression as MemberExpression;
            var constant = Expression.Constant(true);
            return ResolveFunc(member, constant, ExpressionType.Equal);
        }
        if (expression is MethodCallExpression)//x=>x.Name.Contains("xxx")或array.Contains(x.Name)这类
        {
            var methodcall = expression as MethodCallExpression;
            return ResolveLinqToObject(methodcall, true);
        }
        //已经修改过代码body应该不会是null值了
        if (!(expression is BinaryExpression body))
            return string.Empty;
        var @operator = GetOperator(body.NodeType);
        var left = Resolve(body.Left);
        var right = Resolve(body.Right);
        var result = string.Format("({0} {1} {2})", left, @operator, right);
        return result;
    }

    private static string ResolveFunc(Expression left, Expression right, ExpressionType expressiontype)
    {
        var name = (left as MemberExpression).Member.Name;
        var value = (right as ConstantExpression).Value;
        var @operator = GetOperator(expressiontype);
        return name + @operator + value;
    }

    private static string ResolveLinqToObject(Expression expression, object value, ExpressionType? expressiontype = null)
    {
        var methodCall = expression as MethodCallExpression;
        var methodName = methodCall.Method.Name;
        switch (methodName)
        {
            case "Contains":
                return methodCall.Object != null ? Like(methodCall) : In(methodCall, value);

            case "Count":
                return Len(methodCall, value, expressiontype.Value);

            case "LongCount":
                return Len(methodCall, value, expressiontype.Value);

            default:
                throw new Exception(string.Format("不支持{0}方法的查找！", methodName));
        }
    }

    private static string GetOperator(ExpressionType expressiontype)
    {
        return expressiontype switch
        {
            ExpressionType.And => "and",
            ExpressionType.AndAlso => "and",
            ExpressionType.Or => "or",
            ExpressionType.OrElse => "or",
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "<>",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            _ => throw new Exception(string.Format("不支持{0}此种运算符查找！" + expressiontype))
        };
    }

    private static string In(MethodCallExpression expression, object isTrue)
    {
        var argument1 = (expression.Arguments[0] as MemberExpression).Expression as ConstantExpression;
        var argument2 = expression.Arguments[1] as MemberExpression;
        var fieldArray = argument1.Value.GetType().GetFields().First();
        var array = fieldArray.GetValue(argument1.Value) as object[];
        var setInPara = new List<string>();
        for (var i = 0; i < array.Length; i++)
        {
            var value = array[i].ToString();
            setInPara.Add(value);
        }
        var name = argument2.Member.Name;
        var @operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
        var compName = string.Join(",", setInPara);
        var result = string.Format("{0} {1} ({2})", name, @operator, compName);
        return result;
    }

    private static string Like(MethodCallExpression expression)
    {
        var temp = expression.Arguments[0];
        var lambda = Expression.Lambda(temp);
        var fn = lambda.Compile();
        var tempValue = Expression.Constant(fn.DynamicInvoke(null), temp.Type);
        var value = string.Format("%{0}%", tempValue);
        var name = (expression.Object as MemberExpression).Member.Name;
        var result = string.Format("{0} like {1}", name, value);
        return result;
    }

    private static string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
    {
        object name = (expression.Arguments[0] as MemberExpression).Member.Name;
        var @operator = GetOperator(expressiontype);
        var result = string.Format("len({0}){1}{2}", name, @operator, value);
        return result;
    }
}
