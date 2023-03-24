using Furion.DataEncryption;
using SimpleTool;

namespace SimpleAdmin.Plugin.SignalR;

/// <summary>
/// 用户ID提供器
/// </summary>
public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var token = connection.GetHttpContext().Request.Query["access_token"];//获取token
        var claims = JWTEncryption.ReadJwtToken(token)?.Claims;//解析token
        var userId = claims.FirstOrDefault(u => u.Type == ClaimConst.UserId)?.Value;//获取用户ID
        if (!string.IsNullOrEmpty(userId))//如果不为空
            return $"{userId}_{RandomHelper.CreateLetterAndNumber(5)}";//返回用户ID

        else
            return connection.ConnectionId;

    }
}
