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
/// 字典查询参数
/// </summary>
public class DictInput
{
}

/// <summary>
/// 字典树参数
/// </summary>
public class DictTreeInput
{
    /// <summary>
    /// 字典分类
    /// </summary>
    public string Category { get; set; }
}

/// <summary>
/// 字典查询参数
/// </summary>
public class DictPageInput : BasePageInput
{
    /// <summary>
    /// 父id
    ///</summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 分类
    ///</summary>
    public string Category { get; set; } = SysDictConst.DICT_CATEGORY_FRM;
}

/// <summary>
/// 添加字典参数
/// </summary>
public class DictAddInput : SysDict
{
    /// <summary>
    /// 父ID
    /// </summary>
    public override long ParentId { get; set; } = 0;

    /// <summary>
    /// 字典名称
    /// </summary>
    [Required(ErrorMessage = "DictLabel不能为空")]
    public override string DictLabel { get; set; }

    /// <summary>
    /// 字典值
    /// </summary>

    [Required(ErrorMessage = "DictValue不能为空")]
    public override string DictValue { get; set; }
}

/// <summary>
/// 编辑字典参数
/// </summary>
public class DictEditInput : DictAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 删除字典参数
/// </summary>
public class DictDeleteInput : BaseIdListInput
{
}
