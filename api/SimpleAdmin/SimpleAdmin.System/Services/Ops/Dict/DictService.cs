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
/// <inheritdoc cref="IDictService"/>
/// </summary>
public class DictService : DbRepository<SysDict>, IDictService
{
    private readonly ISimpleCacheService _simpleCacheService;

    public DictService(ISimpleCacheService simpleCacheService)
    {
        _simpleCacheService = simpleCacheService;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysDict>> Page(DictPageInput input)
    {
        var query = Context.Queryable<SysDict>().Where(it => it.Category == input.Category)//根据分类查询
            .Where(it => it.ParentId == input.ParentId)//根据父ID查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey),
                it => it.DictLabel.Contains(input.SearchKey) || it.DictValue.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
            .OrderBy(it => it.SortCode)//排序
            .OrderBy(it => it.CreateTime);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(DictAddInput input)
    {
        await CheckInput(input);//检查参数
        var devDict = input.Adapt<SysDict>();//实体转换
        if (await InsertAsync(devDict))//插入数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(DictAddInput input)
    {
        await CheckInput(input);//检查参数
        var devDict = input.Adapt<SysDict>();//实体转换
        if (await UpdateAsync(devDict))//更新数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(DictDeleteInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            //获取所有字典
            var dictList = await GetListAsync();
            //判断是否有系统字典
            var frm = dictList.Any(it => ids.Contains(it.Id) && it.Category == CateGoryConst.DICT_FRM);

            //如果是系统字典提示不可删除
            if (frm) throw Oops.Bah("不可删除系统内置字典");
            var deleteIds = new List<long>();//要删除的id列表
            deleteIds.AddRange(ids);//
            ids.ForEach(it =>
            {
                //获取子节点
                var children = GetDevDictChildren(dictList, it);
                //提取ID
                var childrenIds = children.Select(c => c.Id).ToList();
                deleteIds.AddRange(childrenIds);
            });
            //删除数据
            if (await DeleteByIdsAsync(deleteIds.Cast<object>().ToArray()))
                await RefreshCache();//刷新缓存
        }
    }

    /// <summary>
    /// 获取全部
    /// </summary>
    /// <returns></returns>
    public override async Task<List<SysDict>> GetListAsync()
    {
        //先从redis拿
        var sysDictList = _simpleCacheService.Get<List<SysDict>>(SystemConst.CACHE_DEV_DICT);
        if (sysDictList == null)
        {
            sysDictList = await base.GetListAsync();//去数据库拿
            if (sysDictList.Count > 0)
            {
                _simpleCacheService.Set(SystemConst.CACHE_DEV_DICT, sysDictList);//如果数据库有数,更新redis
                return sysDictList;
            }
        }
        return sysDictList;
    }

    /// <inheritdoc />
    public async Task<SysDict> GetDict(string dictValue)
    {
        var sysDictList = await GetListAsync();
        var devDict = sysDictList.Where(it => it.DictValue == dictValue).FirstOrDefault();
        return devDict;
    }

    /// <inheritdoc />
    public async Task<List<SysDict>> GetChildrenByDictValue(string dictValue)
    {
        var sysDictList = await GetListAsync();
        var devDict = sysDictList.Where(it => it.DictValue == dictValue).FirstOrDefault();
        if (devDict != null)
        {
            var children = GetDevDictChildren(sysDictList, devDict.Id);
            return children;
        }
        return new List<SysDict>();
    }

    /// <inheritdoc />
    public async Task<List<string>> GetValuesByDictValue(string dictValue, List<SysDict> devDictList = null)
    {
        var sysDictList = devDictList == null ? await GetListAsync() : devDictList;//获取全部
        var id = sysDictList.Where(it => it.DictValue == dictValue).Select(it => it.Id).FirstOrDefault();//根据value找到父节点
        if (id > 0)
            return sysDictList.Where(it => it.ParentId == id).Select(it => it.DictValue).ToList();//拿到字典值
        return new List<string>();
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, List<string>>> GetValuesByDictValue(string[] dictValues)
    {
        var result = new Dictionary<string, List<string>>();
        var sysDictList = await GetListAsync();//获取全部
        foreach (var dictValue in dictValues)
        {
            var data = await GetValuesByDictValue(dictValue, sysDictList);
            result.Add(dictValue, data);
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<List<SysDict>> Tree(DictTreeInput input)
    {
        var sysDictList = await GetListAsync();//获取字典列表
        var devList = sysDictList.WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category).OrderBy(it => it.SortCode)
            .ToList();
        return ConstructResourceTrees(devList);
    }

    /// <inheritdoc />
    public List<SysDict> ConstructResourceTrees(List<SysDict> dictList, long parentId = 0)
    {
        //找下级字典ID列表
        var resources = dictList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<SysDict>();
            foreach (var item in resources)//遍历字典
            {
                item.Children = ConstructResourceTrees(dictList, item.Id);//添加子节点
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<SysDict>();
    }

    #region 方法

    /// <summary>
    /// 刷新缓存
    /// </summary>
    private async Task RefreshCache()
    {
        _simpleCacheService.Remove(SystemConst.CACHE_DEV_DICT);
        await GetListAsync();
    }

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysDict"></param>
    private async Task CheckInput(SysDict sysDict)
    {
        var dictList = await GetListAsync();//获取全部字典
        //判断是否从存在重复字典名
        var hasSameLabel = dictList.Any(it =>
            it.ParentId == sysDict.ParentId && it.Category == sysDict.Category && it.DictLabel == sysDict.DictLabel && it.Id != sysDict.Id);
        if (hasSameLabel)
        {
            throw Oops.Bah($"存在重复的字典名称:{sysDict.DictLabel}");
        }
        //判断是否存在重复字典值
        var hasSameValue = dictList.Any(it => it.ParentId == sysDict.ParentId && it.DictValue == sysDict.DictValue && it.Id != sysDict.Id);
        if (hasSameValue)
        {
            throw Oops.Bah($"存在重复的字典值:{sysDict.DictValue}");
        }
    }

    /// <summary>
    /// 获取字典所有下级
    /// </summary>
    /// <param name="dictList">字典列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns></returns>
    public List<SysDict> GetDevDictChildren(List<SysDict> dictList, long parentId)
    {
        //找下级ID列表
        var dicts = dictList.Where(it => it.ParentId == parentId).ToList();
        if (dicts.Count > 0)//如果数量大于0
        {
            var data = new List<SysDict>();
            foreach (var item in dicts)//遍历机构
            {
                var devDictChildren = GetDevDictChildren(dictList, item.Id);
                data.AddRange(devDictChildren);//添加子节点);
                data.Add(item);////添加到列表
            }
            return data;//返回结果
        }
        return new List<SysDict>();
    }

    #endregion 方法
}
