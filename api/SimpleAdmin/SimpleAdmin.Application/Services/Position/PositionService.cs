// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

namespace SimpleAdmin.Application;

/// <summary>
/// <inheritdoc cref="IPositionService"/>
/// </summary>
public class PositionService : DbRepository<SysPosition>, IPositionService
{
    private readonly ISysUserService _sysUserService;
    private readonly ISysPositionService _sysPositionService;

    public PositionService(ISysUserService sysUserService, ISysPositionService sysPositionService)
    {
        _sysUserService = sysUserService;
        _sysPositionService = sysPositionService;
    }

    /// <inheritdoc />
    public async Task<SqlSugarPagedList<SysPosition>> Page(PositionPageInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        input.OrgIds = dataScope;
        //分页查询
        var pageInfo = await _sysPositionService.Page(input);
        return pageInfo;
    }

    /// <inheritdoc />
    public async Task Add(PositionAddInput input)
    {
        await CheckInput(input, SystemConst.ADD);//检查参数
        await _sysPositionService.Add(input, ApplicationConst.BIZ_POS);//添加岗位
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
            //获取职位下所有机构ID
            var orgIds = (await _sysPositionService.GetListAsync()).Where(it => ids.Contains(it.Id)).Select(it => it.OrgId).ToList();
            if (!dataScope.ContainsAll(orgIds))
                throw Oops.Bah("您没有权限删除这些岗位");
        }
        else if (dataScope is { Count: 0 })//表示仅自己
        {
            //获取要删除的岗位列表
            var positions = (await _sysPositionService.GetListAsync()).Where(it => ids.Contains(it.Id)).ToList();
            //如果岗位列表里有任何不是自己创建的岗位
            if (positions.Any(it => it.CreateUserId != UserManager.UserId))
                throw Oops.Bah("只能删除自己创建的岗位");
        }
        await _sysPositionService.Delete(input, ApplicationConst.BIZ_ORG);//删除岗位
    }

    /// <inheritdoc />
    public async Task Edit(PositionEditInput input)
    {
        await CheckInput(input, SystemConst.EDIT);//检查参数
        await _sysPositionService.Edit(input, ApplicationConst.BIZ_POS);//编辑
    }

    /// <inheritdoc/>
    public async Task<List<PositionSelectorOutput>> Selector(PositionSelectorInput input)
    {
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        input.OrgIds = dataScope;//赋值机构列表
        var result = await _sysPositionService.Selector(input);//查询
        return result;
    }

    /// <inheritdoc />
    public async Task<SysPosition> Detail(BaseIdInput input)
    {
        var position = await _sysPositionService.GetSysPositionById(input.Id);
        return position;
    }

    #region 方法

    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysPosition">参数</param>
    /// <param name="operate">操作名称</param>
    private async Task CheckInput(SysPosition sysPosition, string operate)
    {
        var errorMessage = $"您没有权限在该机构下{operate}岗位";
        //获取数据范围
        var dataScope = await _sysUserService.GetLoginUserApiDataScope();
        if (dataScope is { Count: > 0 })//如果有机构
        {
            if (!dataScope.Contains(sysPosition.OrgId))//判断机构ID是否在数据范围
                throw Oops.Bah(errorMessage);
        }
        else if (dataScope is { Count: 0 })// 仅自己
        {
            //如果id大于0表示编辑
            if (sysPosition.Id > 0)
            {
                var position = await _sysPositionService.GetSysPositionById(sysPosition.Id);//获取机构
                if (position.CreateUserId != UserManager.UserId)
                    throw Oops.Bah(errorMessage);//岗位的创建人不是自己则报错
            }
            else
            {
                throw Oops.Bah(errorMessage);
            }
        }
    }

    #endregion 方法
}
