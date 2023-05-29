using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Models;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISessionService"/>
/// </summary>
public class SessionService : DbRepository<SysUser>, ISessionService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly IEventPublisher _eventPublisher;

    public SessionService(ISimpleCacheService simpleCacheService, IEventPublisher eventPublisher)
    {
        _simpleCacheService = simpleCacheService;
        _eventPublisher = eventPublisher;
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
        var tokenDic = GetTokenDicFromRedis();//redistoken会话字典信息
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
    public async Task ExitSession(BaseIdInput input)
    {
        var userId = input.Id.ToString();
        //token列表
        var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.Cache_UserToken, userId);
        //从列表中删除
        _simpleCacheService.HashDel<List<TokenInfo>>(CacheConst.Cache_UserToken, new string[] { userId });
        await NoticeUserLoginOut(userId, tokenInfos);
    }

    /// <inheritdoc/>
    public async Task ExitToken(ExitTokenInput input)
    {
        var userId = input.Id.ToString();
        //获取该用户的token信息
        var tokenInfos = _simpleCacheService.HashGetOne<List<TokenInfo>>(CacheConst.Cache_UserToken, userId);
        //当前需要踢掉用户的token
        var deleteTokens = tokenInfos.Where(it => input.Tokens.Contains(it.Token)).ToList();
        //踢掉包含token列表的token信息
        tokenInfos = tokenInfos.Where(it => !input.Tokens.Contains(it.Token)).ToList();
        if (tokenInfos.Count > 0)
            _simpleCacheService.HashAdd(CacheConst.Cache_UserToken, userId, tokenInfos);//如果还有token则更新token
        else
            _simpleCacheService.HashDel<List<TokenInfo>>(CacheConst.Cache_UserToken, new string[] { userId });//否则直接删除key
        await NoticeUserLoginOut(userId, deleteTokens);
    }

    #region 方法

    /// <summary>
    /// 获取redis中token信息列表
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, List<TokenInfo>> GetTokenDicFromRedis()
    {
        var clockSkew = App.GetConfig<int>("JWTSettings:ClockSkew");//获取过期时间容错值(秒)
        //redis获取token信息hash集合,并转成字典
        var bTokenDic = _simpleCacheService.HashGetAll<List<TokenInfo>>(CacheConst.Cache_UserToken).ToDictionary(u => u.Key, u => u.Value);
        if (bTokenDic != null)
        {
            bTokenDic.ForEach(it =>
            {
                var tokens = it.Value.Where(it => it.TokenTimeout.AddSeconds(clockSkew) > DateTime.Now).ToList();//去掉登录超时的
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
                _simpleCacheService.HashSet(CacheConst.Cache_UserToken, bTokenDic);//如果还有token则更新token
            }
            else
            {
                _simpleCacheService.Remove(CacheConst.Cache_UserToken);//否则直接删除key
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
            var tokenRemainPercent = 1 - (now.ConvertDateTimeToLong() - tokenSecond) * 1.0 / (timeoutSecond - tokenSecond);//求百分比,用现在时间-token颁布时间除以超时时间-token颁布时间
            it.TokenRemainPercent = tokenRemainPercent;
        });
    }

    /// <summary>
    /// 通知用户下线
    /// </summary>
    /// <returns></returns>
    private async Task NoticeUserLoginOut(string userId, List<TokenInfo> tokenInfos)
    {
        await _eventPublisher.PublishAsync(EventSubscriberConst.UserLoginOut, new UserLoginOutEvent
        {
            Message = "您已被强制下线!",
            TokenInfos = tokenInfos,
            UserId = userId
        });//通知用户下线
    }

    #endregion 方法
}