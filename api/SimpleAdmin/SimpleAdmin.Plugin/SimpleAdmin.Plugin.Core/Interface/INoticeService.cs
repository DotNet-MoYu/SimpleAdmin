
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Plugin.Core;


/// <summary>
/// 通知服务
/// </summary>
public interface INoticeService : ITransient
{

    /// <summary>
    /// 通知用户下线
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="clientIds">clientId列表</param>
    /// <param name="message">通知内容</param>
    /// <returns></returns>
    Task UserLoginOut(string userId, List<string> clientIds, string message);
}
