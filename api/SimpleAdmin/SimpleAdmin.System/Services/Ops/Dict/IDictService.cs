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
/// 字典服务
/// </summary>
public interface IDictService : ITransient
{
    /// <summary>
    /// 添加字典
    /// </summary>
    /// <param name="input">输入参数</param>
    /// <returns></returns>
    Task Add(DictAddInput input);

    /// <summary>
    /// 构建字典树形结构
    /// </summary>
    /// <param name="dictList">字典列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns>字典树形结构</returns>
    List<SysDict> ConstructResourceTrees(List<SysDict> dictList, long parentId = 0);

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(DictDeleteInput input);

    /// <summary>
    /// 编辑字典
    /// </summary>
    /// <param name="input">输入参数</param>
    /// <returns></returns>
    Task Edit(DictAddInput input);

    /// <summary>
    /// 获取字典
    /// </summary>
    /// <param name="dictValue">字典</param>
    /// <returns></returns>
    Task<SysDict> GetDict(string dictValue);

    /// <summary>
    /// 获取所有
    /// </summary>
    /// <returns>字典列表</returns>
    Task<List<SysDict>> GetListAsync();

    /// <summary>
    /// 根据字典DictValue获取字典值列表
    /// </summary>
    /// <param name="dictValue">字典值</param>
    /// <param name="devDictList">字典列表</param>
    /// <returns>字典值列表</returns>
    Task<List<string>> GetValuesByDictValue(string dictValue, List<SysDict> devDictList = null);

    /// <summary>
    /// 根据字典DictValue列表获取对应字典值列表
    /// </summary>
    /// <param name="dictValues">字典值列表</param>
    /// <returns></returns>
    Task<Dictionary<string, List<string>>> GetValuesByDictValue(string[] dictValues);

    /// <summary>
    /// 字典分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>字典分页列表</returns>
    Task<SqlSugarPagedList<SysDict>> Page(DictPageInput input);

    /// <summary>
    /// 获取字典树形结构
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>字典树形结构</returns>
    Task<List<SysDict>> Tree(DictTreeInput input);

    /// <summary>
    /// 根据字典值获取子级字典
    /// </summary>
    /// <param name="dictValue">字典值</param>
    /// <returns></returns>
    Task<List<SysDict>> GetChildrenByDictValue(string dictValue);
}
