using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IMenuService"/>
/// </summary>
public class MenuService : DbRepository<SysResource>, IMenuService
{
    private readonly ILogger<MenuService> _logger;
    private readonly IResourceService _resourceService;
    private readonly IRelationService _relationService;

    public MenuService(ILogger<MenuService> logger, IResourceService resourceService, IRelationService relationService)
    {
        this._logger = logger;
        this._resourceService = resourceService;
        this._relationService = relationService;
    }


    /// <inheritdoc/>
    public List<SysResource> ConstructMenuTrees(List<SysResource> resourceList, long parentId = 0)
    {
        //找下级资源ID列表
        var resources = resourceList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (resources.Count > 0)//如果数量大于0
        {
            var data = new List<SysResource>();
            foreach (var item in resources)//遍历资源
            {
                item.Children = ConstructMenuTrees(resourceList, item.Id);//添加子节点
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<SysResource>();
    }


    /// <inheritdoc />
    public async Task<List<SysResource>> Tree(MenuTreeInput input)
    {
        //获取所有菜单
        var sysResources = await _resourceService.GetListByCategory(CateGoryConst.Resource_MENU);
        sysResources = sysResources.WhereIF(input.Module != null, it => it.Module.Value == input.Module.Value)//根据模块查找
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title == input.SearchKey)//根据关键字查找
            .ToList();
        //构建菜单树
        var tree = ConstructMenuTrees(sysResources);
        return tree;

    }

    /// <inheritdoc />
    public async Task Add(MenuAddInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换

        if (await InsertAsync(sysResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.Resource_MENU);//刷新菜单缓存

    }

    /// <inheritdoc />
    public async Task Edit(MenuEditInput input)
    {
        await CheckInput(input);//检查参数
        var sysResource = input.Adapt<SysResource>();//实体转换
        if (await UpdateAsync(sysResource))//更新数据
            await _resourceService.RefreshCache(CateGoryConst.Resource_MENU);//刷新菜单缓存
    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //获取所有菜单和按钮
            var resourceList = await _resourceService.GetListAsync(new List<string> { CateGoryConst.Resource_MENU, CateGoryConst.Resource_BUTTON });
            //找到要删除的菜单
            var sysResources = resourceList.Where(it => ids.Contains(it.Id)).ToList();
            //查找内置菜单
            var system = sysResources.Where(it => it.Code == ResourceConst.System).FirstOrDefault();
            if (system != null)
                throw Oops.Bah($"不可删除系统菜单:{system.Title}");
            //需要删除的资源ID列表
            var resourceIds = new List<long>();
            //遍历菜单列表
            sysResources.ForEach(it =>
            {
                //获取菜单所有子节点
                var child = _resourceService.GetChildListById(resourceList, it.Id, false);
                //将子节点ID添加到删除资源ID列表
                resourceIds.AddRange(child.Select(it => it.Id).ToList());
                resourceIds.Add(it.Id);//添加到删除资源ID列表

            });
            ids.AddRange(resourceIds);//添加到删除ID列表
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {

                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除菜单和按钮
                await Context.Deleteable<SysRelation>()//关系表删除对应SYS_ROLE_HAS_RESOURCE
                 .Where(it => it.Category == CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE && resourceIds.Contains(SqlFunc.ToInt64(it.TargetId))).ExecuteCommandAsync();
            });
            if (result.IsSuccess)//如果成功了
            {
                await _resourceService.RefreshCache(CateGoryConst.Resource_MENU);//资源表菜单刷新缓存
                await _resourceService.RefreshCache(CateGoryConst.Resource_BUTTON);//资源表按钮刷新缓存
                await _relationService.RefreshCache(CateGoryConst.Relation_SYS_ROLE_HAS_RESOURCE);//关系表刷新缓存
            }
            else
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0002);
            }
        }
    }


    /// <inheritdoc />
    public async Task<SysResource> Detail(BaseIdInput input)
    {
        var sysResources = await _resourceService.GetListByCategory(CateGoryConst.Resource_MENU);
        var resource = sysResources.Where(it => it.Id == input.Id).FirstOrDefault();
        return resource;
    }

    /// <inheritdoc />
    public async Task ChangeModule(MenuChangeModuleInput input)
    {
        var sysResource = await Detail(new BaseIdInput { Id = input.Id });
        if (sysResource != null)
        {
            if (sysResource.Module == input.Module)//如果模块ID没变直接返回
                return;
            if (sysResource.ParentId != 0)
                throw Oops.Bah($"非顶级菜单不可修改所属模块");
            //获取所有菜单和模块
            var resourceList = await _resourceService.GetListAsync(new List<string> { CateGoryConst.Resource_MENU, CateGoryConst.Resource_MODULE });
            if (!resourceList.Any(it => it.Category == CateGoryConst.Resource_MODULE && it.Id == input.Module.Value))
                throw Oops.Bah($"不存在的模块");
            //获取所有菜单
            var menuList = resourceList.Where(it => it.Category == CateGoryConst.Resource_MENU).ToList();
            //获取需要改变模块菜单的所有子菜单
            var childList = _resourceService.GetChildListById(menuList, input.Id);
            childList.ForEach(it => it.Module = input.Module);
            //更新数据
            await UpdateRangeAsync(childList);
            //刷新菜单缓存
            await _resourceService.RefreshCache(CateGoryConst.Resource_MENU);
        }
    }


    #region 方法
    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysResource"></param>
    private async Task CheckInput(SysResource sysResource)
    {
        //获取所有菜单列表
        var menList = await _resourceService.GetListByCategory(CateGoryConst.Resource_MENU);
        //判断是否有同级且同名的菜单
        if (menList.Any(it => it.ParentId == sysResource.ParentId && it.Title == sysResource.Title && it.Id != sysResource.Id))
            throw Oops.Bah($"存在重复的菜单名称:{sysResource.Title}");
        if (sysResource.ParentId != 0)
        {
            //获取父级,判断父级ID正不正确
            var parent = menList.Where(it => it.Id == sysResource.ParentId).FirstOrDefault();
            if (parent != null)
            {
                if (parent.Module != sysResource.Module)//如果父级的模块和当前模块不一样
                    throw Oops.Bah($"模块与上级菜单不一致");
                if (parent.Id == sysResource.Id)
                    throw Oops.Bah($"上级菜单不能选择自己");
            }
            else
            {
                throw Oops.Bah($"上级菜单不存在:{sysResource.ParentId}");
            }
        }

    }

    #endregion
}
