// Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
// 
// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
// 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Plugin.Mobile;

/// <summary>
/// <inheritdoc cref="IMobileButtonService"/>
/// </summary>
public class MobileMobileButtonService : DbRepository<MobileResource>, IMobileButtonService
{
    private readonly ILogger<MobileMobileButtonService> _logger;
    private readonly IMobileResourceService _resourceService;
    private readonly IRelationService _relationService;

    public MobileMobileButtonService(ILogger<MobileMobileButtonService> logger, IMobileResourceService resourceService,
        IRelationService relationService)
    {
        _logger = logger;
        _resourceService = resourceService;
        _relationService = relationService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<MobileResource>> Page(ButtonPageInput input)
    {
        var query = Context.Queryable<MobileResource>()
            .Where(it => it.ParentId == input.ParentId && it.Category == CateGoryConst.RESOURCE_BUTTON)
            .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Title.Contains(input.SearchKey) || it.Path.Contains(input.SearchKey))//根据关键字查询
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
            .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(MobileButtonAddInput input)
    {
        await CheckInput(input);//检查参数
        var MobileResource = input.Adapt<MobileResource>();//实体转换
        if (await InsertAsync(MobileResource))//插入数据
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_BUTTON);//刷新缓存
    }

    /// <inheritdoc />
    public async Task<List<long>> AddBatch(MobileButtonAddInput input)
    {
        var MobileResources = new List<MobileResource>();//按钮列表
        var codeList = new List<string> { "Add", "Edit", "Delete", "BatchDelete", "Import", "Export", "BatchEdit" };//code后缀
        var titleList = new List<string> { "新增", "编辑", "删除", "批量删除", "导入", "导出", "批量编辑" };//title前缀
        var idList = new List<long>();//Id列表
        for (var i = 0; i < codeList.Count; i++)
        {
            var id = CommonUtils.GetSingleId();
            MobileResources.Add(new MobileResource
            {
                Id = id,
                Title = titleList[i] + input.Title,//标题等于前缀输入的值
                Code = input.Code + codeList[i],//code等于输入的值加后缀
                ParentId = input.ParentId,
                SortCode = i + 1
            });
            idList.Add(id);
        }
        //遍历列表
        foreach (var MobileResource in MobileResources)
        {
            await CheckInput(MobileResource);//检查按钮参数
        }
        //添加到数据库
        if (await InsertRangeAsync(MobileResources))//插入数据
        {
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_BUTTON);//刷新缓存
            return MobileResources.Select(it => it.Id).ToList();
        }
        else
        {
            return new List<long>();
        }
    }

    /// <inheritdoc />
    public async Task Edit(MobileButtonEditInput input)
    {
        await CheckInput(input);//检查参数
        var MobileResource = input.Adapt<MobileResource>();//实体转换
        //事务
        var result = await Tenant.UseTranAsync(async () =>
        {
            await UpdateAsync(MobileResource);//更新按钮
        });
        if (result.IsSuccess)//如果成功了
        {
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_BUTTON);//资源表按钮刷新缓存
        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0002);
        }
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        //获取所有按钮集合
        var buttonList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_BUTTON);

        #region 处理关系表角色资源信息

        //获取所有菜单集合
        var menuList = await _resourceService.GetListByCategory(CateGoryConst.RESOURCE_MENU);
        //获取按钮的父菜单id集合
        var parentIds = buttonList.Where(it => ids.Contains(it.Id)).Select(it => it.ParentId.Value.ToString()).ToList();
        //获取关系表分类为SYS_ROLE_HAS_RESOURCE数据
        var roleResources = await _relationService.GetRelationByCategory(CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);
        //获取相关关系表数据
        var relationList = roleResources
            .Where(it => parentIds.Contains(it.TargetId))//目标ID是父ID中
            .Where(it => it.ExtJson != null).ToList();//扩展信息不为空
        //遍历关系表
        relationList.ForEach(it =>
        {
            var relationRoleResuorce = it.ExtJson.ToJsonEntity<RelationRoleResource>();//拓展信息转实体
            var buttonInfo = relationRoleResuorce.ButtonInfo;//获取按钮信息
            if (buttonInfo.Count > 0)
            {
                var diffArr = buttonInfo.Where(it => !buttonInfo.Contains(it)).ToList();//找出不同的元素(即交集的补集)
                relationRoleResuorce.ButtonInfo = diffArr;//重新赋值按钮信息
                it.ExtJson = relationRoleResuorce.ToJson();//重新赋值拓展信息
            }
        });

        #endregion 处理关系表角色资源信息

        //事务
        var result = await Tenant.UseTranAsync(async () =>
        {
            await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除按钮
            if (relationList.Count > 0)
            {
                await Context.Updateable(relationList).UpdateColumns(it => it.ExtJson).ExecuteCommandAsync();//修改拓展信息
            }
        });
        if (result.IsSuccess)//如果成功了
        {
            await _resourceService.RefreshCache(CateGoryConst.RESOURCE_BUTTON);//资源表按钮刷新缓存
            await _relationService.RefreshCache(CateGoryConst.RELATION_SYS_ROLE_HAS_RESOURCE);//关系表刷新角色资源缓存
        }
        else
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0002);
        }
    }

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="MobileResource"></param>
    private async Task CheckInput(MobileResource MobileResource)
    {
        //获取所有按钮和菜单
        var buttonList = await _resourceService.GetListAsync(new List<string> { CateGoryConst.RESOURCE_BUTTON, CateGoryConst.RESOURCE_MENU });
        //判断code是否重复
        if (buttonList.Any(it => it.Code == MobileResource.Code && it.Id != MobileResource.Id))
            throw Oops.Bah($"存在重复的按钮编码:{MobileResource.Code}");
        //判断菜单是否存在
        if (!buttonList.Any(it => it.Id == MobileResource.ParentId))
            throw Oops.Bah($"不存在的父级菜单:{MobileResource.ParentId}");
        MobileResource.Category = CateGoryConst.RESOURCE_BUTTON;//设置分类为按钮
    }

    #endregion 方法
}
