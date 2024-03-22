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
/// 配置分页参数
/// </summary>
public class ConfigPageInput : BasePageInput
{
}

/// <summary>
/// 添加配置参数
/// </summary>
public class ConfigAddInput : SysConfig
{
    /// <summary>
    /// 配置键
    /// </summary>
    [Required(ErrorMessage = "configKey不能为空")]
    public override string ConfigKey { get; set; }

    /// <summary>
    /// 配置值
    /// </summary>

    [Required(ErrorMessage = "ConfigValue不能为空")]
    public override string ConfigValue { get; set; }
}

/// <summary>
/// 编辑配置参数
/// </summary>
public class ConfigEditInput : ConfigAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 删除配置参数
/// </summary>
public class ConfigDeleteInput : BaseIdInput
{
}

/// <summary>
/// 批量修改输入参数
/// </summary>
public class ConfigEditBatchInput
{
    /// <summary>
    /// 分类
    /// </summary>
    public string CateGory { get; set; }

    public List<SysConfig> DevConfigs { get; set; }
}
