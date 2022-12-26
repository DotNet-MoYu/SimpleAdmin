using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Models;
using System.Linq;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISessionService"/>
/// </summary>
public class SessionService : DbRepository<SysUser>, ISessionService
{
    private readonly ISimpleRedis _simpleRedis;

    public SessionService(ISimpleRedis simpleRedis)
    {
        this._simpleRedis = simpleRedis;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SessionOutput>> PageB(SessionPageInput input)
    {
        //获取b端token列表
        var bTokenInfoDic = GetTokenDicFromRedis();

        //获取用户ID列表
        var userIds = bTokenInfoDic.Keys.Select(it => it.ToLong()).ToList();
        var query = Context.Queryable<SysUser>().Where(it => userIds.Contains(it.Id))//根据ID查询
              .WhereIF(!string.IsNullOrEmpty(input.Name), it => it.Name.Contains(input.Name))//根据姓名查询
              .WhereIF(!string.IsNullOrEmpty(input.Account), it => it.Account.Contains(input.Account))//根据账号查询
              .WhereIF(!string.IsNullOrEmpty(input.LatestLoginIp), it => it.LatestLoginIp.Contains(input.LatestLoginIp))//根据IP查询
              .OrderBy(it => it.LatestLoginTime, OrderByType.Desc)
              .Select<SessionOutput>()
              .Mapper(it =>
              {
                  var tokenInfos = bTokenInfoDic[it.Id.ToString()];//获取用户token信息
                  GetTokenInfos(ref tokenInfos, LoginClientTypeEnum.B);//获取剩余时间
                  it.TokenCount = tokenInfos.Count;//令牌数量
                  it.TokenSignList = tokenInfos;//令牌列表
                  //如果有mqtt客户端ID就是在线
                  it.OnlineStatus = tokenInfos.Any(it => it.ClientIds.Count > 0) ? DevDictConst.ONLINE_STATUS_ONLINE : DevDictConst.ONLINE_STATUS_OFFLINE;
              });

        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        pageInfo.Records.OrderByDescending(it => it.TokenCount);
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SessionOutput>> PageC(SessionPageInput input)
    {

        return new SqlSugarPagedList<SessionOutput>() { Current = 1, Size = 20, Total = 0, Pages = 1, HasNextPages = false };
    }


    /// <inheritdoc/>
    public SessionAnalysisOutPut Analysis()
    {

        Dictionary<string, List<TokenInfo>> tokenDic = GetTokenDicFromRedis();//redistoken会话字典信息
        var tokenInfosList = tokenDic.Values.ToList();//端token列表
        var dicB = new Dictionary<string, List<TokenInfo>>();
        var dicC = new Dictionary<string, List<TokenInfo>>();
        var onLineCount = 0;
        foreach (var token in tokenDic)
        {
            var b = token.Value.Where(it => it.LoginClientType == LoginClientTypeEnum.B).ToList();//获取该用户B端token
            var c = token.Value.Where(it => it.LoginClientType == LoginClientTypeEnum.C).ToList();//获取该用户C端token
            if (b.Count > 0)
                dicB.Add(token.Key, b);
            if (c.Count > 0)
                dicC.Add(token.Key, c);
            var count = token.Value.Count(it => it.ClientIds.Count > 0);//计算在线用户
            onLineCount += count;
        }
        var tokenB = dicB.Values.ToList();//b端token列表
        var tokenC = dicC.Values.ToList();//c端token列表
        int maxCountB = 0, maxCountC = 0;
        if (tokenB.Count > 0)
            maxCountB = tokenB.OrderByDescending(it => it.Count).Take(1).First().Count();//b端最大会话数

        if (tokenC.Count > 0)
            maxCountC = tokenC.OrderByDescending(it => it.Count).Take(1).First().Count();//C端最大会话数
        return new SessionAnalysisOutPut()
        {
            OnLineCount = onLineCount,
            CurrentSessionTotalCount = tokenB.Count + tokenC.Count,
            MaxTokenCount = maxCountB > maxCountC ? maxCountB : maxCountC,
            ProportionOfBAndC = $"{tokenB.Count}/{tokenC.Count}"
        };
    }

    /// <inheritdoc/>
    public void ExitSession(BaseIdInput input, LoginClientTypeEnum loginClientType)
    {

        //从列表中删除
        _simpleRedis.HashDel<List<TokenInfo>>(RedisConst.Redis_UserToken, new string[] { input.Id.ToString() });
    }

    /// <inheritdoc/>
    public void ExitToken(ExitTokenInput input, LoginClientTypeEnum loginClientType)
    {
        //获取该用户的token信息
        var tokenInfos = _simpleRedis.HashGetOne<List<TokenInfo>>(RedisConst.Redis_UserToken, input.Id.ToString());
        //踢掉包含token列表的token信息
        tokenInfos = tokenInfos.Where(it => !input.Tokens.Contains(it.Token)).ToList();
        if (tokenInfos.Count > 0)
        {
            _simpleRedis.HashAdd(RedisConst.Redis_UserToken, input.Id.ToString(), tokenInfos);//如果还有token则更新token
        }
        else
        {
            _simpleRedis.Remove(RedisConst.Redis_UserToken);//否则直接删除key
        }

    }
    #region 方法


    /// <summary>
    /// 获取redis中token信息列表
    /// </summary>
    /// <param name="key">redis键</param>
    /// <returns></returns>
    public Dictionary<string, List<TokenInfo>> GetTokenDicFromRedis()
    {
        //redis获取token信息hash集合,并转成字典
        var bTokenDic = _simpleRedis.HashGetAll<List<TokenInfo>>(RedisConst.Redis_UserToken).ToDictionary(u => u.Key, u => u.Value);
        if (bTokenDic != null)
        {
            bTokenDic.ForEach(it =>
            {
                var tokens = it.Value.Where(it => it.TokenTimeout > DateTime.Now).ToList();//去掉登录超时的
                if (tokens.Count == 0)
                {
                    //表示都过期了
                    bTokenDic.Remove(it.Key);
                }
                else
                {
                    bTokenDic[it.Key] = tokens;//重新赋值token
                }

            });
            if (bTokenDic.Count > 0)
            {
                _simpleRedis.HashSet(RedisConst.Redis_UserToken, bTokenDic);//如果还有token则更新token
            }
            else
            {
                _simpleRedis.Remove(RedisConst.Redis_UserToken);//否则直接删除key
            }
            return bTokenDic;
        }
        else
        {
            return new Dictionary<string, List<TokenInfo>>();
        }

    }


    /// <summary>
    /// 获取token剩余时间信息
    /// </summary>
    /// <param name="tokenInfos">token列表</param>
    /// <param name="loginClientType">登录类型</param>
    public void GetTokenInfos(ref List<TokenInfo> tokenInfos, LoginClientTypeEnum loginClientType)
    {
        tokenInfos = tokenInfos.Where(it => it.LoginClientType == loginClientType).ToList();
        tokenInfos.ForEach(it =>
        {
            var now = DateTime.Now;
            it.TokenRemain = now.GetDiffTime(it.TokenTimeout);//获取时间差
            var tokenSecond = it.TokenTimeout.AddMinutes(-it.Expire).ConvertDateTimeToLong();//颁发时间转为时间戳
            var timeoutSecond = it.TokenTimeout.ConvertDateTimeToLong();//过期时间转为时间戳
            var tokenRemainPercent = 1 - ((now.ConvertDateTimeToLong() - tokenSecond) * 1.0 / (timeoutSecond - tokenSecond));//求百分比,用现在时间-token颁布时间除以超时时间-token颁布时间
            it.TokenRemainPercent = tokenRemainPercent;
        });

    }
    #endregion
}
