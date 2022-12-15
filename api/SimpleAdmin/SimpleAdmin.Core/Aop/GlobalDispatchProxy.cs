using Furion.DataEncryption;
using Furion.Reflection;
using Furion.Reflection.Extensions;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

namespace SimpleAdmin.Core;

/// <summary>
/// 全局异常处理
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
        else
        {
            var result = Before(method, args);//方法执行之前判断是否有缓存的数据
            if (result == null) result = method.Invoke(Target, args);//如果没有缓存就执行方法返回数据
            After(method, result, args);//方法执行之后干的事
            return result;//返回结果
        }

    }

    /// <summary>
    /// 异步无返回值
    /// </summary>
    /// <param name="method"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async override Task InvokeAsync(MethodInfo method, object[] args)
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
    public async override Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
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
            var _redisManager = Services.GetService<IRedisCacheManager>(); // 获取redis服务
            var cacheKey = cacheAttribute.CustomKeyValue ?? CustomCacheKey(cacheAttribute.KeyPrefix, method, args);//如果redisKey值，如果有自定义值就用自定义Key，否则以前缀+系统自动生成的Key
            var cacheValue = string.Empty;
            if (cacheAttribute.StoreType == RedisConst.Redis_Hash)//如果存的是Hash值
            {
                cacheValue = _redisManager.HashGet<string>(cacheKey, new string[] { args[0].ToString() })[0]; //从redis获取Hash数据取第一个,注意是 string 类型
            }
            else
            {
                cacheValue = _redisManager.Get<string>(cacheKey); //注意是 string 类型，方法GetValue
            }
            if (!string.IsNullOrEmpty(cacheValue))//如果返回值不是空
            {
                //if (cacheAttribute.IsDelete)//判断是否是删除操作
                //{
                //    if (cacheAttribute.StoreType == RedisConst.Redis_Hash)//如果是Hash
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
                else
                {
                    return cacheValue;
                }




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
                var _redisManager = Services.GetService<IRedisCacheManager>(); // 获取redis服务
                if (cacheAttribute.IsDelete)//判断是否是删除操作
                {
                    //删除Redis整个KEY
                    _redisManager.Remove(cacheKey);
                }
                else
                {
                    if (returnValue == null) { return; }
                    if (cacheAttribute.StoreType == RedisConst.Redis_Hash)//如果是hash类型的
                    {
                        //插入到hash，这里规定是方法的第一个参数
                        _redisManager.HashAdd(cacheKey, args[0].ToString(), returnValue);
                    }
                    else
                    {
                        if (cacheAttribute.AbsoluteExpiration != null)//如果有超时时间
                        {
                            _redisManager.Set(cacheKey, returnValue, cacheAttribute.AbsoluteExpiration.Value);//插入redis
                        }
                        else
                        {
                            _redisManager.Set(cacheKey, returnValue);//插入redis
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
            key = $"{RedisConst.Redis_Prefix_Web}{typeName}:{methodName}:";//生成Key
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
        if (arg is DateTime || arg is DateTime?)
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
            else if (arg.GetType().IsClass)
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
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            return Resolve(expression);
        }
        if (expression is BinaryExpression)
        {
            BinaryExpression binary = expression as BinaryExpression;
            if (binary.Left is MemberExpression && binary.Right is ConstantExpression)//解析x=>x.Name=="123" x.Age==123这类
                return ResolveFunc(binary.Left, binary.Right, binary.NodeType);
            if (binary.Left is MethodCallExpression && binary.Right is ConstantExpression)//解析x=>x.Name.Contains("xxx")==false这类的
            {
                object value = (binary.Right as ConstantExpression).Value;
                return ResolveLinqToObject(binary.Left, value, binary.NodeType);
            }
            if ((binary.Left is MemberExpression && binary.Right is MemberExpression)
                || (binary.Left is MemberExpression && binary.Right is UnaryExpression))//解析x=>x.Date==DateTime.Now这种
            {
                LambdaExpression lambda = Expression.Lambda(binary.Right);
                Delegate fn = lambda.Compile();
                ConstantExpression value = Expression.Constant(fn.DynamicInvoke(null), binary.Right.Type);
                return ResolveFunc(binary.Left, value, binary.NodeType);
            }
        }
        if (expression is UnaryExpression)
        {
            UnaryExpression unary = expression as UnaryExpression;
            if (unary.Operand is MethodCallExpression)//解析!x=>x.Name.Contains("xxx")或!array.Contains(x.Name)这类
                return ResolveLinqToObject(unary.Operand, false);
            if (unary.Operand is MemberExpression && unary.NodeType == ExpressionType.Not)//解析x=>!x.isDeletion这样的 
            {
                ConstantExpression constant = Expression.Constant(false);
                return ResolveFunc(unary.Operand, constant, ExpressionType.Equal);
            }
        }
        if (expression is MemberExpression && expression.NodeType == ExpressionType.MemberAccess)//解析x=>x.isDeletion这样的 
        {
            MemberExpression member = expression as MemberExpression;
            ConstantExpression constant = Expression.Constant(true);
            return ResolveFunc(member, constant, ExpressionType.Equal);
        }
        if (expression is MethodCallExpression)//x=>x.Name.Contains("xxx")或array.Contains(x.Name)这类
        {
            MethodCallExpression methodcall = expression as MethodCallExpression;
            return ResolveLinqToObject(methodcall, true);
        }
        //已经修改过代码body应该不会是null值了
        if (!(expression is BinaryExpression body))
            return string.Empty;
        var Operator = GetOperator(body.NodeType);
        var Left = Resolve(body.Left);
        var Right = Resolve(body.Right);
        string Result = string.Format("({0} {1} {2})", Left, Operator, Right);
        return Result;
    }

    private static string ResolveFunc(Expression left, Expression right, ExpressionType expressiontype)
    {
        var Name = (left as MemberExpression).Member.Name;
        var Value = (right as ConstantExpression).Value;
        var Operator = GetOperator(expressiontype);
        return Name + Operator + Value ?? "null";
    }

    private static string ResolveLinqToObject(Expression expression, object value, ExpressionType? expressiontype = null)
    {
        var MethodCall = expression as MethodCallExpression;
        var MethodName = MethodCall.Method.Name;
        switch (MethodName)
        {
            case "Contains":
                if (MethodCall.Object != null)
                    return Like(MethodCall);
                return In(MethodCall, value);
            case "Count":
                return Len(MethodCall, value, expressiontype.Value);
            case "LongCount":
                return Len(MethodCall, value, expressiontype.Value);
            default:
                throw new Exception(string.Format("不支持{0}方法的查找！", MethodName));
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
            _ => throw new Exception(string.Format("不支持{0}此种运算符查找！" + expressiontype)),
        };
    }

    private static string In(MethodCallExpression expression, object isTrue)
    {
        var Argument1 = (expression.Arguments[0] as MemberExpression).Expression as ConstantExpression;
        var Argument2 = expression.Arguments[1] as MemberExpression;
        var Field_Array = Argument1.Value.GetType().GetFields().First();
        object[] Array = Field_Array.GetValue(Argument1.Value) as object[];
        List<string> SetInPara = new List<string>();
        for (int i = 0; i < Array.Length; i++)
        {

            string Value = Array[i].ToString();
            SetInPara.Add(Value);
        }
        string Name = Argument2.Member.Name;
        string Operator = Convert.ToBoolean(isTrue) ? "in" : " not in";
        string CompName = string.Join(",", SetInPara);
        string Result = string.Format("{0} {1} ({2})", Name, Operator, CompName);
        return Result;
    }
    private static string Like(MethodCallExpression expression)
    {

        var Temp = expression.Arguments[0];
        LambdaExpression lambda = Expression.Lambda(Temp);
        Delegate fn = lambda.Compile();
        var tempValue = Expression.Constant(fn.DynamicInvoke(null), Temp.Type);
        string Value = string.Format("%{0}%", tempValue);
        string Name = (expression.Object as MemberExpression).Member.Name;
        string Result = string.Format("{0} like {1}", Name, Value);
        return Result;
    }
    private static string Len(MethodCallExpression expression, object value, ExpressionType expressiontype)
    {
        object Name = (expression.Arguments[0] as MemberExpression).Member.Name;
        string Operator = GetOperator(expressiontype);
        string Result = string.Format("len({0}){1}{2}", Name, Operator, value.ToString());
        return Result;
    }
}
