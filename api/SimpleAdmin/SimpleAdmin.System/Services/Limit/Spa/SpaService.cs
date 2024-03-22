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
/// <inheritdoc cref="ISpaService"/>
/// </summary>
public class SpaService : DbRepository<SysResource>, ISpaService
{
    private readonly IResourceService _resourceService;

    public SpaService(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysResource>> Page(SpaPageInput input)
    {
        var query = Context.Queryable<SysResource>().Where(it => it.Category == CateGoryConst.RESOURCE_SPA)//单页
            .WhereIF(!string.IsNullOrEmpty(input.MenuType), it => it.MenuType == input.MenuType)//根据菜单类型查询
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title.Contains(input.SearchKey) || it.Path.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}").OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(SpaAddInput input)
    {
        await CheckInput(input);//检查参数
        input.Code = RandomHelper.CreateRandomString(10);//code取随机值
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await InsertAsync(sysResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_SPA);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(SpaEditInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await UpdateAsync(sysResource))//更新数据
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_SPA);//刷新缓存
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        if (ids.Count > 0)
        {
            //获取所有
            var resourceList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_SPA);
            //找到要删除的
            var sysresources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置单页面
            var system = sysresources.Where(it => it.Code == SysResourceConst.SYSTEM).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统内置单页面:{system.Title}");
            //删除菜单
            await DeleteAsync(sysresources);
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_SPA);//刷新缓存
        }
    }

    /// <inheritdoc />
    public async Task<SysResource> Detail(BaseIdInput input)
    {
        var sysResources = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_SPA);
        var resource = sysResources.Where(it => it.Id == input.Id).FirstOrDefault();
        return resource;
    }


    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysResource"></param>
    private async Task CheckInput(SysResource sysResource)
    {
        //判断菜单类型
        if (sysResource.MenuType == SysResourceConst.MENU)//如果是菜单
        {
            if (string.IsNullOrEmpty(sysResource.Name))
            {
                throw Oops.Bah("单页名称不能为空");
            }
            if (string.IsNullOrEmpty(sysResource.Component))
            {
                throw Oops.Bah("组件地址不能为空");
            }
        }
        else if (sysResource.MenuType == SysResourceConst.IFRAME || sysResource.MenuType == SysResourceConst.LINK)//如果是内链或者外链
        {
            // sysResource.Name = RandomHelper.CreateNum(10);//设置name为随机数
            sysResource.Name = null;//设置name为标题
            sysResource.Component = null;
        }
        else
        {
            throw Oops.Bah($"单页类型错误:{sysResource.MenuType}");//都不是
        }
        if (sysResource.IsHome)
        {
            var spas = await _resourceService.GetListByCategory(SysResourceConst.SPA);//获取所有单页
            if (spas.Any(it => it.IsHome && it.Id != sysResource.Id))//如果有多个主页
            {
                throw Oops.Bah("已存在首页,请取消其他主页后再试");
            }
            sysResource.IsHide = false;//如果是主页,则不隐藏
            sysResource.IsAffix = true;//如果是主页,则固定
        }
        //设置为单页
        sysResource.Category = CateGoryConst.RESOURCE_SPA;
    }

    #endregion 方法
}
