using Masuit.Tools;
using Masuit.Tools.DateTimeExt;
using Masuit.Tools.Models;
using Shiny.Helper.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="ISessionService"/>
/// </summary>
public class SessionService : DbRepository<SysUser>, ISessionService
{
    private readonly IRedisCacheManager _redisCacheManager;

    public SessionService(IRedisCacheManager redisCacheManager)
    {
        this._redisCacheManager = redisCacheManager;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SessionOutput>> PageB(SessionPageInput input)
    {
        //获取b端token列表
        var bTokenInfoDic = GetTokenDicFromRedis(RedisConst.Redis_UserTokenB);
        //获取用户ID列表
        var userIds = bTokenInfoDic.Keys.Select(it => it.ToLong()).ToList();
        var query = Context.Queryable<SysUser>().Where(it => userIds.Contains(it.Id))//根据ID查询
              .WhereIF(!string.IsNullOrEmpty(input.Name), it => it.Name.Contains(input.Name))//根据姓名查询
              .WhereIF(!string.IsNullOrEmpty(input.Account), it => it.Name.Contains(input.Account))//根据账号查询
              .WhereIF(!string.IsNullOrEmpty(input.LatestLoginIp), it => it.Name.Contains(input.LatestLoginIp))//根据IP查询
              .OrderBy(it => it.LatestLoginTime, OrderByType.Desc)
              .Select<SessionOutput>()
              .Mapper(it =>
              {
                  var tokenInfos = bTokenInfoDic[it.Id.ToString()];//获取用户token信息
                  GetTokenInfos(ref tokenInfos);//获取剩余时间
                  it.TokenCount = tokenInfos.Count;//令牌数量
                  it.TokenSignList = tokenInfos;//令牌列表
              });

        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SessionOutput>> PageC(SessionPageInput input)
    {

        return new SqlSugarPagedList<SessionOutput>();
    }


    /// <inheritdoc/>
    public SessionAnalysisOutPut Analysis()
    {

        var redisB = GetTokenDicFromRedis(RedisConst.Redis_UserTokenB);//b端会话信息
        var redisC = GetTokenDicFromRedis(RedisConst.Redis_UserTokenB);//c端会话信息
        var tokenB = redisB.Values.ToList();//b端token列表
        int maxCountB = 0, maxCountC = 0;
        if (tokenB.Count > 0)
            maxCountB = tokenB.OrderByDescending(it => it.Count).Take(1).First().Count();//b端最大会话数

        var tokenC = redisC.Values.ToList();//c端token列表
        if (tokenC.Count > 0)
            maxCountC = tokenC.OrderByDescending(it => it.Count).Take(1).First().Count();//C端最大会话数
        return new SessionAnalysisOutPut()
        {
            OnLineCount = 1,
            CurrentSessionTotalCount = tokenB.Count + tokenC.Count,
            MaxTokenCount = maxCountB > maxCountC ? maxCountB : maxCountC,
            ProportionOfBAndC = $"{tokenB.Count}/{tokenC.Count}"
        };
    }

    /// <inheritdoc/>
    public void ExitSession(BaseIdInput input, LoginClientTypeEnum loginClientType)
    {
        //获取key
        var key = GetRedisKeyByLoginClientType(loginClientType);
        //从列表中删除
        _redisCacheManager.HashDel<List<TokenInfo>>(key, new string[] { input.Id.ToString() });
    }

    /// <inheritdoc/>
    public void ExitToken(ExitTokenInput input, LoginClientTypeEnum loginClientType)
    {
        //获取key
        var key = GetRedisKeyByLoginClientType(loginClientType);
        //获取该用户的token信息
        var tokenInfos = _redisCacheManager.HashGetOne<List<TokenInfo>>(key, input.Id.ToString());
        //踢掉包含token列表的token信息
        tokenInfos = tokenInfos.Where(it => !input.Tokens.Contains(it.Token)).ToList();
        if (tokenInfos.Count > 0)
        {
            _redisCacheManager.HashAdd(key, input.Id.ToString(), tokenInfos);//如果还有token则更新token
        }
        else
        {
            _redisCacheManager.Remove(key);//否则直接删除key
        }

    }
    #region 方法

    /// <summary>
    /// 根据登录类型获取key
    /// </summary>
    /// <param name="loginClientType">登录类型</param>
    /// <returns></returns>
    private string GetRedisKeyByLoginClientType(LoginClientTypeEnum loginClientType)
    {
        //获取key
        var key = loginClientType == LoginClientTypeEnum.B ? RedisConst.Redis_UserTokenB : RedisConst.Redis_UserTokenC;
        return key;
    }

    /// <summary>
    /// 获取redis中token信息列表
    /// </summary>
    /// <param name="key">redis键</param>
    /// <returns></returns>
    public Dictionary<string, List<TokenInfo>> GetTokenDicFromRedis(string key)
    {
        //redis获取token信息hash集合,并转成字典
        var bTokenDic = _redisCacheManager.HashGetAll<List<TokenInfo>>(key).ToDictionary(u => u.Key, u => u.Value);
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
                _redisCacheManager.HashSet(key, bTokenDic);//如果还有token则更新token
            }
            else
            {
                _redisCacheManager.Remove(key);//否则直接删除key
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
    public void GetTokenInfos(ref List<TokenInfo> tokenInfos)
    {
        tokenInfos.ForEach(it =>
        {
            var now = DateTime.Now;
            it.TokenRemain = now.GetDiffTime(it.TokenTimeout);//获取时间差
            var tokenSecond = it.TokenTimeout.AddMinutes(-it.Expire).ConvertDateTimeToLong();//颁发时间转为时间戳
            var timeoutSecond = it.TokenTimeout.ConvertDateTimeToLong();//过期时间转为时间戳
            var tokenRemainPercent = ((timeoutSecond - tokenSecond) * 1.0 / (now.ConvertDateTimeToLong() - tokenSecond));//求百分比,用超时时间-token颁布时间除以现在时间-token颁布时间
            it.TokenRemainPercent = tokenRemainPercent;
        });

    }
    #endregion
}
