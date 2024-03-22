// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.System;

/// <summary>
/// 字典常量
/// </summary>
public class SysDictConst
{
    #region 系统字典

    /// <summary>
    /// 性别
    /// </summary>
    public const string GENDER = "GENDER";

    /// <summary>
    /// 名族
    /// </summary>
    public const string NATION = "NATION";

    /// <summary>
    /// 用户证件类型
    /// </summary>
    public const string ID_CARD_TYPE = "ID_CARD_TYPE";

    /// <summary>
    /// 通用文化程度
    /// </summary>
    public const string CULTURE_LEVEL = "CULTURE_LEVEL";

    /// <summary>
    /// 系统职位分类
    /// </summary>
    public const string POSITION_CATEGORY = "POSITION_CATEGORY";

    #region 在线用户状态

    /// <summary>
    /// 在线
    /// </summary>
    public const string ONLINE_STATUS_ONLINE = "ONLINE";

    /// <summary>
    /// 离线
    /// </summary>
    public const string ONLINE_STATUS_OFFLINE = "OFFLINE";

    #endregion 在线用户状态

    #region 上传文件引擎

    /// <summary>
    /// 本地
    /// </summary>
    public const string FILE_ENGINE_LOCAL = "LOCAL";

    /// <summary>
    /// MINIO
    /// </summary>
    public const string FILE_ENGINE_MINIO = "MINIO";

    #endregion 上传文件引擎

    #region 系统字典分类

    /// <summary>
    /// 系统
    /// </summary>
    public const string DICT_CATEGORY_FRM = "FRM";

    /// <summary>
    /// 业务
    /// </summary>
    public const string DICT_CATEGORY_BIZ = "BIZ";

    #endregion 系统字典分类

    #region 多租户选项

    /// <summary>
    /// 关闭
    /// </summary>
    public const string TENANT_OPTIONS_CLOSE = "TENANT_OPTIONS_CLOSE";

    /// <summary>
    /// 手动选择
    /// </summary>
    public const string TENANT_OPTIONS_CHOSE = "TENANT_OPTIONS_CHOSE";

    /// <summary>
    /// 根据域名
    /// </summary>
    public const string TENANT_OPTIONS_DOMAIN = "TENANT_OPTIONS_DOMAIN";

    #endregion

    #endregion 系统字典
}
