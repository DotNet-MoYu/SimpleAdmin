// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Core.Utils;

/// <summary>
/// 密码相关通用类
/// </summary>
public class PwdUtil
{
    /// <summary>
    /// 密码相似度
    /// </summary>
    /// <param name="oldPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public static double Similarity(string oldPassword, string newPassword)
    {
        var editDistance = LevenshteinDistance(oldPassword, newPassword);
        var similarity = 1.0 - editDistance / (double)Math.Max(oldPassword.Length, newPassword.Length);
        return similarity * 100;
    }

    /// <summary>
    /// 计算莱文斯坦距离算法
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static int LevenshteinDistance(string s1, string s2)
    {
        var distance = new int[s1.Length + 1, s2.Length + 1];

        for (var i = 0; i <= s1.Length; i++)
        {
            distance[i, 0] = i;
        }

        for (var j = 0; j <= s2.Length; j++)
        {
            distance[0, j] = j;
        }

        for (var i = 1; i <= s1.Length; i++)
        {
            for (var j = 1; j <= s2.Length; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;

                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }
        return distance[s1.Length, s2.Length];
    }
}
