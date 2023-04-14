namespace SimpleAdmin.Application;

/// <inheritdoc cref="IOrgService"/>
public class OrgService : DbRepository<SysOrg>, IOrgService
{
    private readonly ISimpleCacheService _simpleCacheService;
    private readonly ISysOrgService _sysOrgService;
    private readonly ISysUserService _sysUserService;

    public OrgService(ISimpleCacheService simpleCacheService, ISysOrgService sysOrgService, ISysUserService sysUserService)
    {
        _simpleCacheService = simpleCacheService;
        this._sysOrgService = sysOrgService;
        this._sysUserService = sysUserService;
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
    public async Task<List<SysOrg>> Tree()
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        //构建机构树
        var result = await _sysOrgService.Tree(dataScope);
        return result;
    }

    /// <inheritdoc />
    public async Task Add(SysOrgAddInput input)
    {
        await CheckInput(input, SimpleAdminConst.Add);//检查参数
        await _sysOrgService.Add(input, SimpleAdminConst.BizOrg);
    }

    /// <inheritdoc />
    public async Task Edit(SysOrgEditInput input)
    {
        await CheckInput(input, SimpleAdminConst.Edit);//检查参数
        await _sysOrgService.Edit(input, SimpleAdminConst.BizOrg);
    }

    /// <inheritdoc />
    public async Task Copy(SysOrgCopyInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope.Count > 0)//如果有机构
        {
            if (!dataScope.ContainsAll(input.Ids) || !dataScope.Contains(input.TargetId))//判断目标机构和需要复制的机构是否都在数据范围里面
                throw Oops.Bah($"您没有权限复制这些机构");
            await _sysOrgService.Copy(input);//复制操作
        }
    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope.Count > 0)//如果有机构
        {
            if (!dataScope.ContainsAll(ids))//判断要删除的id列表是否都包含在数据范围内
                throw Oops.Bah($"您没有权限删除这些机构");
        }
        else
        {
            //获取要删除的机构列表
            var orgs = (await _sysOrgService.GetListAsync()).Where(it => ids.Contains(it.Id)).ToList();
            //如果机构列表里有任何不是自己创建的机构
            if (orgs.Any(it => it.CreateUserId != UserManager.UserId))
                throw Oops.Bah($"只能删除自己创建的机构");
        }
        await _sysOrgService.Delete(input, SimpleAdminConst.BizOrg);//删除操作
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
        if (dataScope.Count > 0)//如果有机构
        {
            if (sysOrg.Id > 0 && !dataScope.Contains(sysOrg.Id))//如果id不为0判断是否在数据范围
                throw Oops.Bah(errorMessage);
            if (!dataScope.Contains(sysOrg.ParentId))//判断父ID是否在数据范围
                throw Oops.Bah($"{errorMessage}下的机构");
        }
        else
        {
            //如果id大于0表示编辑
            if (sysOrg.Id > 0)
            {
                var org = await _sysOrgService.GetSysOrgById(sysOrg.Id);//获取机构
                if (org.CreateUserId != UserManager.UserId) throw Oops.Bah(errorMessage);//机构的创建人不是自己则报错
            }
            else
            {
                throw Oops.Bah(errorMessage);
            }
        }
    }

    #endregion 方法
}