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
/// 分类常量
/// </summary>
public class CateGoryConst
{
    #region 系统配置

    /// <summary>
    /// 系统基础
    /// </summary>
    public const string CONFIG_SYS_BASE = "SYS_BASE";

    /// <summary>
    /// 登录策略
    /// </summary>
    public const string CONFIG_LOGIN_POLICY = "LOGIN_POLICY";

    /// <summary>
    /// 密码策略
    /// </summary>
    public const string CONFIG_PWD_POLICY = "PWD_POLICY";

    /// <summary>
    /// 业务定义
    /// </summary>
    public const string CONFIG_BIZ_DEFINE = "BIZ_DEFINE";

    /// <summary>
    /// 文件-本地
    /// </summary>
    public const string CONFIG_FILE_LOCAL = "FILE_LOCAL";

    /// <summary>
    /// 文件-MINIO
    /// </summary>
    public const string CONFIG_FILE_MINIO = "FILE_MINIO";

    #endregion 系统配置

    #region Mqtt配置

    /// <summary>
    /// MQTT配置
    /// </summary>
    public const string CONFIG_MQTT_BASE = "MQTT_BASE";

    #endregion Mqtt配置

    #region 关系表

    /// <summary>
    /// 用户有哪些角色
    /// </summary>
    public const string RELATION_SYS_USER_HAS_ROLE = "SYS_USER_HAS_ROLE";

    /// <summary>
    /// 角色有哪些资源
    /// </summary>
    public const string RELATION_SYS_ROLE_HAS_RESOURCE = "SYS_ROLE_HAS_RESOURCE";

    /// <summary>
    /// 角色有哪些模块
    /// </summary>
    public const string RELATION_SYS_ROLE_HAS_MODULE = "SYS_ROLE_HAS_MODULE";

    /// <summary>
    /// 用户有哪些模块
    /// </summary>
    public const string RELATION_SYS_USER_HAS_MODULE = "SYS_USER_HAS_MODULE";

    /// <summary>
    ///用户有哪些资源
    /// </summary>
    public const string RELATION_SYS_USER_HAS_RESOURCE = "SYS_USER_HAS_RESOURCE";

    /// <summary>
    /// 角色有哪些权限
    /// </summary>
    public const string RELATION_SYS_ROLE_HAS_PERMISSION = "SYS_ROLE_HAS_PERMISSION";

    /// <summary>
    /// 用户有哪些权限
    /// </summary>
    public const string RELATION_SYS_USER_HAS_PERMISSION = "SYS_USER_HAS_PERMISSION";

    /// <summary>
    /// 用户工作台数据
    /// </summary>
    public const string RELATION_SYS_USER_WORKBENCH_DATA = "SYS_USER_WORKBENCH_DATA";

    /// <summary>
    /// 用户日程数据
    /// </summary>
    public const string RELATION_SYS_USER_SCHEDULE_DATA = "SYS_USER_SCHEDULE_DATA";

    /// <summary>
    /// 站内信与接收用户
    /// </summary>
    public const string RELATION_MSG_TO_USER = "MSG_TO_USER";

    #endregion 关系表

    #region 数据范围

    /// <summary>
    /// 本人
    /// </summary>
    public const string SCOPE_SELF = "SCOPE_SELF";

    /// <summary>
    /// 所有
    /// </summary>
    public const string SCOPE_ALL = "SCOPE_ALL";

    /// <summary>
    /// 仅所属组织
    /// </summary>
    public const string SCOPE_ORG = "SCOPE_ORG";

    /// <summary>
    /// 所属组织及以下
    /// </summary>
    public const string SCOPE_ORG_CHILD = "SCOPE_ORG_CHILD";

    /// <summary>
    /// 自定义
    /// </summary>
    public const string SCOPE_ORG_DEFINE = "SCOPE_ORG_DEFINE";

    #endregion 数据范围

    #region 资源表

    /// <summary>
    /// 模块
    /// </summary>
    public const string RESOURCE_MODULE = "MODULE";

    /// <summary>
    /// 菜单
    /// </summary>
    public const string RESOURCE_MENU = "MENU";

    /// <summary>
    /// 单页
    /// </summary>
    public const string RESOURCE_SPA = "SPA";

    /// <summary>
    /// 按钮
    /// </summary>
    public const string RESOURCE_BUTTON = "BUTTON";

    #endregion 资源表

    #region 日志表

    /// <summary>
    /// 登录
    /// </summary>
    public const string LOG_LOGIN = "LOGIN";

    /// <summary>
    /// 登出
    /// </summary>
    public const string LOG_LOGOUT = "LOGOUT";

    /// <summary>
    /// 操作
    /// </summary>
    public const string LOG_OPERATE = "OPERATE";

    /// <summary>
    /// 异常
    /// </summary>
    public const string LOG_EXCEPTION = "EXCEPTION";

    #endregion 日志表

    #region 字典表

    /// <summary>
    /// 框架
    /// </summary>
    public const string DICT_FRM = "FRM";

    /// <summary>
    /// 业务
    /// </summary>
    public const string DICT_BIZ = "BIZ";

    #endregion 字典表

    #region 组织表

    /// <summary>
    /// 部门
    /// </summary>
    public const string ORG_DEPT = "DEPT";

    /// <summary>
    /// 公司
    /// </summary>
    public const string ORG_COMPANY = "COMPANY";

    #endregion 组织表

    #region 职位表

    /// <summary>
    /// 高层
    /// </summary>
    public const string POSITION_HIGH = "HIGH";

    /// <summary>
    /// 中层
    /// </summary>
    public const string POSITION_MIDDLE = "MIDDLE";

    /// <summary>
    /// 基层
    /// </summary>
    public const string POSITION_LOW = "LOW";

    #endregion 职位表

    #region 角色表

    /// <summary>
    /// 全局
    /// </summary>
    public const string ROLE_GLOBAL = "GLOBAL";

    /// <summary>
    /// 机构
    /// </summary>
    public const string ROLE_ORG = "ORG";

    #endregion 角色表

    #region 站内信表

    /// <summary>
    /// 通知
    /// </summary>
    public const string MESSAGE_INFORM = "INFORM";

    /// <summary>
    /// 公告
    /// </summary>
    public const string MESSAGE_NOTICE = "NOTICE";

    #endregion 站内信表
}
