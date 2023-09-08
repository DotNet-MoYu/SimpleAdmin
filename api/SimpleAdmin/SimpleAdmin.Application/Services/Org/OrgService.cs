// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Application;

/// <inheritdoc cref="IOrgService"/>
public class OrgService : DbRepository<SysOrg>, IOrgService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysUserService _sysUserService;

    public OrgService(ISimpleCacheService simpleCacheService, ISysOrgService sysOrgService,
        ISysUserService sysUserService)
    {
        _simpleCacheService = simpleCacheService;
        _sysOrgService = sysOrgService;
        _sysUserService = sysUserService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysOrg>> Page(SysOrgPageInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        input.OrgIds = dataScope;
        //分页查询
        var pageInfo = await _sysOrgService.Page(input);
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task<List<SysOrg>> Tree(SysOrgTreeInput input = null)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        //构建机构树
        var result = await _sysOrgService.Tree(dataScope, input);
        return result;
    }

    /// <inheritdoc />
    public async Task Add(SysOrgAddInput input)
    {
        await CheckInput(input, SystemConst.ADD);//检查参数
        await _sysOrgService.Add(input, ApplicationConst.BIZ_ORG);
    }

    /// <inheritdoc />
    public async Task Edit(SysOrgEditInput input)
    {
        await CheckInput(input, SystemConst.EDIT);//检查参数
        await _sysOrgService.Edit(input, ApplicationConst.BIZ_ORG);
    }

    /// <inheritdoc />
    public async Task Copy(SysOrgCopyInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope == null || dataScope.Count > 0)//如果有机构
        {
            if (dataScope is { Count: > 0 } && (!dataScope.ContainsAll(input.Ids)
                || !dataScope.Contains(input.TargetId)))//判断目标机构和需要复制的机构是否都在数据范围里面
                throw Oops.Bah($"您没有权限复制这些机构");
            await _sysOrgService.Copy(input);//复制操作
        }
    }

    /// <inheritdoc />
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids;
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope is { Count: > 0 })//如果有机构
        {
            if (!dataScope.ContainsAll(ids))//判断要删除的id列表是否都包含在数据范围内
                throw Oops.Bah($"您没有权限删除这些机构");
        }
        else if (dataScope is { Count: 0 })//表示仅自己
        {
            //获取要删除的机构列表
            var orgs = (await _sysOrgService.GetListAsync()).Where(it => ids.Contains(it.Id))
                .ToList();
            //如果机构列表里有任何不是自己创建的机构
            if (orgs.Any(it => it.CreateUserId != UserManager.UserId))
                throw Oops.Bah($"只能删除自己创建的机构");
        }
        await _sysOrgService.Delete(input, ApplicationConst.BIZ_ORG);//删除操作
    }

    /// <inheritdoc />
    public async Task<SysOrg> Detail(BaseIdInput input)
    {
        var sysOrg = await _sysOrgService.GetSysOrgById(input.Id);
        return sysOrg;
    }

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysOrg">参数</param>
    /// <param name="operate">操作名称</param>
    private async Task CheckInput(SysOrg sysOrg, string operate)
    {
        var errorMessage = $"您没有权限{operate}该机构";
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope is { Count: > 0 })//如果有机构
        {
            if (sysOrg.Id > 0 && !dataScope.Contains(sysOrg.Id))//如果id不为0判断是否在数据范围
                throw Oops.Bah(errorMessage);
            if (!dataScope.Contains(sysOrg.ParentId))//判断父ID是否在数据范围
                throw Oops.Bah($"{errorMessage}下的机构");
        }
        else if (dataScope is { Count: 0 })
        {
            //如果id大于0表示编辑
            if (sysOrg.Id > 0)
            {
                var org = await _sysOrgService.GetSysOrgById(sysOrg.Id);//获取机构
                if (org.CreateUserId != UserManager.UserId)
                    throw Oops.Bah(errorMessage);//机构的创建人不是自己则报错
            }
            else
            {
                throw Oops.Bah(errorMessage);
            }
        }
    }

    #endregion 方法
}
