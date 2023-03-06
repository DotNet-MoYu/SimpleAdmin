namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IDictService"/>
/// </summary>
public class DictService : DbRepository<DevDict>, IDictService
{

    private readonly ISimpleRedis _simpleRedis;

    public DictService(ISimpleRedis simpleRedis)
    {
        _simpleRedis = simpleRedis;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<DevDict>> Page(DictPageInput input)
    {
        var query = Context.Queryable<DevDict>()
                           .WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category)//根据分类查询
                           .WhereIF(input.ParentId != null, it => it.ParentId == input.ParentId)//根据父ID查询
                           .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.DictLabel.Contains(input.SearchKey) || it.DictValue.Contains(input.SearchKey))//根据关键字查询
                           .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                           .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(DictAddInput input)
    {
        await CheckInput(input);//检查参数
        var devDict = input.Adapt<DevDict>();//实体转换
        if (await InsertAsync(devDict))//插入数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(DictAddInput input)
    {
        await CheckInput(input);//检查参数
        var devDict = input.Adapt<DevDict>();//实体转换
        if (await UpdateAsync(devDict))//更新数据
            await RefreshCache();//刷新缓存
    }


    /// <inheritdoc />
    public async Task Delete(DictDeleteInput input)
    {
        //获取所有字典
        var dicts = await GetListAsync();
        //获取当前字典分类
        var category = dicts.Where(it => it.Id == input.Id).Select(it => it.Category).FirstOrDefault();
        //如果是系统字典提示不可删除
        //if (category == CateGoryConst.Dict_FRM) throw Oops.Bah("不可删除系统内置字典");
        //获取子节点
        var childens = GetDevDictChilden(dicts, input.Id);
        //提取ID
        var ids = childens.Select(it => it.Id).ToList();
        ids.Add(input.Id);//加上自己的
        //删除数据
        if (await DeleteByIdsAsync(ids.Cast<object>().ToArray()))
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public override async Task<List<DevDict>> GetListAsync()
    {
        //先从redis拿
        var devDicts = _simpleRedis.Get<List<DevDict>>(RedisConst.Redis_DevDict);
        if (devDicts == null)
        {
            devDicts = await base.GetListAsync();//去数据库拿
            if (devDicts.Count > 0)
            {
                _simpleRedis.Set(RedisConst.Redis_DevDict, devDicts);//如果数据库有数,更新redis
                return devDicts;
            }
        }
        return devDicts;
    }

    /// <inheritdoc />
    public async Task<DevDict> GetDict(string DictValue)
    {
        var devDicts = await GetListAsync();
        var devDict = devDicts.Where(it => it.DictValue == DictValue).FirstOrDefault();
        return devDict;
    }

    /// <inheritdoc />
    public async Task<List<string>> GetValuesByDictValue(string DictValue, List<DevDict> devDictList = null)
    {
        var devDicts = devDictList == null ? await GetListAsync() : devDictList;//获取全部
        var id = devDicts.Where(it => it.DictValue == DictValue).Select(it => it.Id).FirstOrDefault();//根据value找到父节点
        if (id > 0)
            return devDicts.Where(it => it.ParentId == id).Select(it => it.DictValue).ToList();//拿到字典值
        else
            return new List<string>();
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, List<string>>> GetValuesByDictValue(string[] DictValues)
    {
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        var devDicts = await GetListAsync();//获取全部
        var ids = devDicts.Where(it => DictValues.Contains(it.DictValue)).Select(it => it.Id).ToList();//根据value找到父节点
        foreach (var dictValue in DictValues)
        {
            var data = await GetValuesByDictValue(dictValue, devDicts);
            result.Add(dictValue, data);
        }
        return result;
    }


    /// <inheritdoc />
    public async Task<List<DevDict>> Tree(DictTreeInput input)
    {
        var devDicts = await GetListAsync();//获取字典列表
        var devList = devDicts.WhereIF(!string.IsNullOrEmpty(input.Category), it => it.Category == input.Category).OrderBy(it => it.SortCode).ToList();
        return ConstructResourceTrees(devList);
    }


    /// <inheritdoc />
    public List<DevDict> ConstructResourceTrees(List<DevDict> dictList, long parentId = 0)
    {
        //找下级字典ID列表
        var resources = dictList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<DevDict>();
            foreach (var item in resources)//遍历字典
            {
                item.Children = ConstructResourceTrees(dictList, item.Id);//添加子节点
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<DevDict>();
    }

    #region 方法

    /// <summary>
    /// 刷新缓存
    /// </summary>
    private async Task RefreshCache()
    {
        _simpleRedis.Remove(RedisConst.Redis_DevDict);
        await GetListAsync();
    }

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="devDict"></param>
    private async Task CheckInput(DevDict devDict)
    {
        var dicts = await GetListAsync();//获取全部字典
        //判断是否从存在重复字典名
        var hasSameLable = dicts.Any(it => it.ParentId == devDict.ParentId && it.Category == devDict.Category && it.DictLabel == devDict.DictLabel && it.Id != devDict.Id);
        if (hasSameLable)
        {
            throw Oops.Bah($"存在重复的字典名称:{devDict.DictLabel}");
        }
        //判断是否存在重复字典值
        var hasSameValue = dicts.Any(it => it.ParentId == devDict.ParentId && it.DictValue == devDict.DictValue && it.Id != devDict.Id);
        if (hasSameValue)
        {
            throw Oops.Bah($"存在重复的字典值:{devDict.DictValue}");
        }
    }


    /// <summary>
    /// 获取字典所有下级
    /// </summary>
    /// <param name="dictList">字典列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns></returns>
    public List<DevDict> GetDevDictChilden(List<DevDict> dictList, long parentId)
    {
        //找下级ID列表
        var resources = dictList.Where(it => it.ParentId == parentId).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<DevDict>();
            foreach (var item in resources)//遍历机构
            {
                var orgs = GetDevDictChilden(dictList, item.Id);
                data.AddRange(orgs);//添加子节点);
                data.Add(item);////添加到列表
            }
            return data;//返回结果
        }
        return new List<DevDict>();
    }

    #endregion
}
