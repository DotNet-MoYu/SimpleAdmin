using Masuit.Tools;

namespace SimpleAdmin.System;


/// <inheritdoc cref="ISysOrgService"/>
public class SysOrgService : DbRepository<SysOrg>, ISysOrgService
{
    private readonly ISimpleRedis _simpleRedis;


    public SysOrgService(ISimpleRedis simpleRedis)
    {
        _simpleRedis = simpleRedis;
    }



    /// <inheritdoc />
    public override async Task<List<SysOrg>> GetListAsync()
    {
        //先从Redis拿
        var sysOrgs = _simpleRedis.Get<List<SysOrg>>(RedisConst.Redis_SysOrg);
        if (sysOrgs == null)
        {
            //redis没有就去数据库拿
            sysOrgs = await base.GetListAsync();
            if (sysOrgs.Count > 0)
            {
                //插入Redis
                _simpleRedis.Set(RedisConst.Redis_SysOrg, sysOrgs);

            }
        }
        return sysOrgs;
    }

    /// <inheritdoc />
    public async Task<SysOrg> GetSysOrgById(long id)
    {
        var sysOrg = await GetListAsync();
        var result = sysOrg.Where(it => it.Id == id).FirstOrDefault();
        return result;
    }

    /// <inheritdoc />
    public async Task<List<SysOrg>> GetChildListById(long orgId, bool isContainOneself = true)
    {
        //获取所有组织
        var sysOrgs = await GetListAsync();
        //查找下级
        var childLsit = GetSysOrgChilden(sysOrgs, orgId);
        if (isContainOneself)//如果包含自己
        {
            //获取自己的组织信息
            var self = sysOrgs.Where(it => it.Id == orgId).FirstOrDefault();
            if (self != null) childLsit.Insert(0, self);//如果组织不为空就插到第一个
        }
        return childLsit;
    }

    /// <inheritdoc />
    public async Task<List<long>> GetOrgChildIds(long orgId, bool isContainOneself = true)
    {
        var orgIds = new List<long>();//组织列表
        if (orgId > 0)//如果orgid有值
        {
            //获取所有子集
            var sysOrgs = await GetChildListById(orgId, isContainOneself);
            orgIds = sysOrgs.Select(x => x.Id).ToList();//提取ID列表
        }
        return orgIds;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<SysOrg>> Page(OrgPageInput input)
    {
        var query = Context.Queryable<SysOrg>()
                         .WhereIF(input.ParentId > 0, it => it.ParentId == input.ParentId)//父级
                         .WhereIF(input.OrgIds != null, it => input.OrgIds.Contains(it.Id))//机构ID查询
                         .WhereIF(!string.IsNullOrEmpty(input.SearchKey), it => it.Name.Contains(input.SearchKey))//根据关键字查询
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")
                         .OrderBy(it => it.SortCode);//排序
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }


    /// <inheritdoc />
    public async Task<List<SysOrg>> Tree(List<long> orgIds = null)
    {
        //获取所有组织
        var sysOrgs = await GetListAsync();
        if (orgIds != null)
            sysOrgs = GetParentListByIds(sysOrgs, orgIds);//如果组织ID不为空则获取组织ID列表的所有父节点
        sysOrgs = sysOrgs.OrderBy(it => it.SortCode).ToList();//排序
        //构建组织树
        var result = ConstrucOrgTrees(sysOrgs);
        return result;
    }

    /// <inheritdoc />
    public async Task Add(OrgAddInput input, string name = SimpleAdminConst.SysOrg)
    {
        await CheckInput(input, name);//检查参数
        var sysOrg = input.Adapt<SysOrg>();//实体转换
        sysOrg.Code = RandomHelper.CreateRandomString(10);//赋值Code
        if (await InsertAsync(sysOrg))//插入数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task Edit(OrgEditInput input, string name = SimpleAdminConst.SysOrg)
    {
        await CheckInput(input, name);//检查参数
        var sysOrg = input.Adapt<SysOrg>();//实体转换
        if (await UpdateAsync(sysOrg))//更新数据
            await RefreshCache();//刷新缓存
    }

    /// <inheritdoc />
    public async Task<SysOrg> Detail(BaseIdInput input)
    {

        var sysOrgs = await GetListAsync();
        var orgDetail = sysOrgs.Where(it => it.Id == input.Id).FirstOrDefault();
        return orgDetail;

    }

    /// <inheritdoc />
    public async Task Delete(List<BaseIdInput> input, string name = SimpleAdminConst.SysOrg)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            var sysOrgs = await GetListAsync();//获取所有组织
            var sysDeleteOrgList = new List<long>();//需要删除的组织ID集合
            ids.ForEach(it =>
            {
                var childen = GetSysOrgChilden(sysOrgs, it);//查找下级组织
                sysDeleteOrgList.AddRange(childen.Select(it => it.Id).ToList());
                sysDeleteOrgList.Add(it);
            });
            //如果组织下有用户则不能删除
            if (await Context.Queryable<SysUser>().AnyAsync(it => sysDeleteOrgList.Contains(it.OrgId)))
            {
                throw Oops.Bah($"请先删除{name}下的用户");
            }
            //获取用户表有兼任组织的信息
            var positionJsons = await Context.Queryable<SysUser>().Where(it => !SqlFunc.IsNullOrEmpty(it.PositionJson)).Select(it => it.PositionJson).ToListAsync();
            if (positionJsons.Count > 0)
            {
                //去一次空
                positionJsons.Where(it => it != null).ToList().ForEach(it =>
                {

                    //获取组织列表
                    var orgIds = it.Select(it => it.OrgId).ToList();
                    //获取交集
                    var sameOrgIds = sysDeleteOrgList.Intersect(orgIds).ToList();
                    if (sameOrgIds.Count > 0)
                    {
                        throw Oops.Bah($"请先删除{name}下的兼任用户");
                    }
                });
            }
            //判断组织下是否有角色
            var hasRole = await Context.Queryable<SysRole>().Where(it => sysDeleteOrgList.Contains(it.OrgId.Value)).CountAsync() > 0;
            if (hasRole) throw Oops.Bah($"请先删除{name}下的角色");
            // 判断组织下是否有职位
            var hasPosition = await Context.Queryable<SysPosition>().Where(it => sysDeleteOrgList.Contains(it.OrgId)).CountAsync() > 0;
            if (hasRole) throw Oops.Bah($"请先删除{name}下的职位");
            //删除组织
            if (await DeleteByIdsAsync(sysDeleteOrgList.Cast<object>().ToArray()))
                await RefreshCache();//刷新缓存
        }
    }



    /// <inheritdoc />
    public async Task Copy(OrgCopyInput input)
    {
        var orgList = await GetListAsync();//获取所有
        HashSet<long> ids = new HashSet<long>();//定义不重复Id集合
        var addOrgList = new List<SysOrg>();//添加机构列表
        HashSet<long> alreadyIds = new HashSet<long>();//定义已经复制过得组织Id
        ids.AddRange(input.Ids);//加到集合
        if (ids.Contains(input.TargetId))
            throw Oops.Bah($"不能包含自己");

        if (1 == 2)
        {
            //是否包含下级
            if (input.ContainsChild)
            {

                input.Ids.ForEach(async it =>
                {
                    var childIds = await GetOrgChildIds(it, false);//获取下级id列表

                    ids.AddRange(childIds);//添加到id列表
                });
            }
            var neeedCopyOrgs = orgList.Where(it => ids.Contains(it.Id)).ToList();//获取需要复制的机构列表

            neeedCopyOrgs.ForEach(it =>
            {
                var newId = YitIdHelper.NextId();
                var childList = neeedCopyOrgs.Where(c => c.ParentId == it.Id).ToList();//获取下级
                if (it.ParentId == 0)//如果是顶级
                    it.ParentId = input.TargetId;
                else if (childList.Count > 0)//如果有下级
                    childList.ForEach(it => it.ParentId = newId);//父Id重新赋值
                else if (!input.Ids.Contains(it.ParentId))//如果当前组织的父Id不在选中的组织中
                    it.ParentId = input.TargetId;

                //else if (!input.Ids.Contains(it.ParentId))//如果当前组织的父Id不在选中的组织中
                //    it.ParentId = input.TargetId;
                //addOrgList.Add(new SysOrg
                //{
                //    Id= newId, Code= RandomHelper.CreateRandomString(10),  Category=it.Category, Name=it.Name, DirectorId=it.DirectorId, TargetId=it.TargetId, 
                //});
                //重新赋值
                it.Id = newId;
                it.Code = RandomHelper.CreateRandomString(10);
                it.CreateTime = DateTime.Now;
                it.CreateUser = UserManager.UserAccount;
                it.CreateUserId = UserManager.UserId;
            });

            if (await InsertRangeAsync(neeedCopyOrgs))//插入数据
                await RefreshCache();//刷新缓存
        }
        input.Ids.ForEach(async it =>
         {
             var org = orgList.Where(o => o.Id == it).FirstOrDefault();//获取下级
             if (org != null && !alreadyIds.Contains(it))
             {
                 alreadyIds.Add(it);//添加到已复制列表
                 RedirectOrg(org);//生成新的实体
                 org.ParentId = input.TargetId;//父id为目标Id
                 addOrgList.Add(org);
                 //是否包含下级
                 if (input.ContainsChild)
                 {
                     var childIds = await GetOrgChildIds(it, false);//获取下级id列表
                     alreadyIds.AddRange(childIds);//添加到已复制id
                     var childList = orgList.Where(c => childIds.Contains(c.Id)).ToList();//获取下级
                     var addOrgs = CopySysOrgChilden(childList, it, org.Id);//赋值下级组织
                     addOrgList.AddRange(addOrgs);
                 }
             }

         });

        if (await InsertRangeAsync(addOrgList))//插入数据
            await RefreshCache();//刷新缓存
    }



    /// <inheritdoc />
    public async Task RefreshCache()
    {
        _simpleRedis.Remove(RedisConst.Redis_SysOrg);//从redis删除
        _simpleRedis.Remove(RedisConst.Redis_SysUser);//清空redis所有的用户信息
        await GetListAsync();//刷新缓存
    }

    /// <inheritdoc />
    public List<SysOrg> ConstrucOrgTrees(List<SysOrg> orgList, long parentId = 0)
    {
        //找下级字典ID列表
        var orgs = orgList.Where(it => it.ParentId == parentId).OrderBy(it => it.SortCode).ToList();
        if (orgs.Count > 0)//如果数量大于0
        {
            var data = new List<SysOrg>();
            foreach (var item in orgs)//遍历字典
            {
                item.Children = ConstrucOrgTrees(orgList, item.Id);//添加子节点
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<SysOrg>();
    }

    /// <inheritdoc />
    public List<SysOrg> GetOrgParents(List<SysOrg> allOrgList, long orgId, bool includeSelf = true)
    {
        //找到组织
        var sysOrgs = allOrgList.Where(it => it.Id == orgId).FirstOrDefault();
        if (sysOrgs != null)//如果组织不为空
        {
            var data = new List<SysOrg>();
            var parents = GetOrgParents(allOrgList, sysOrgs.ParentId, includeSelf);//递归获取父节点
            data.AddRange(parents);//添加父节点;
            if (includeSelf)
                data.Add(sysOrgs);//添加到列表
            return data;//返回结果
        }
        return new List<SysOrg>();
    }

    #region 方法
    /// <summary>
    /// 检查输入参数
    /// </summary>
    /// <param name="sysOrg"></param>
    /// <param name="name">名称</param>
    private async Task CheckInput(SysOrg sysOrg, string name)
    {
        //判断分类是否正确
        if (sysOrg.Category != CateGoryConst.Org_COMPANY && sysOrg.Category != CateGoryConst.Org_DEPT)
            throw Oops.Bah($"{name}所属分类错误:{sysOrg.Category}");

        var sysOrgs = await GetListAsync();//获取全部
        if (sysOrgs.Any(it => it.ParentId == sysOrg.ParentId && it.Name == sysOrg.Name && it.Id != sysOrg.Id))//判断同级是否有名称重复的
            throw Oops.Bah($"存在重复的同级{name}:{sysOrg.Name}");
        if (sysOrg.ParentId != 0)
        {
            //获取父级,判断父级ID正不正确
            var parent = sysOrgs.Where(it => it.Id == sysOrg.ParentId).FirstOrDefault();
            if (parent != null)
            {
                if (parent.Id == sysOrg.Id)
                    throw Oops.Bah($"上级{name}不能选择自己");
            }
            else
            {
                throw Oops.Bah($"上级{name}不存在:{sysOrg.ParentId}");
            }
        }
    }

    /// <summary>
    /// 根据组织Id列表获取所有父级组织
    /// </summary>
    /// <param name="allOrgList">所有机构列表</param>
    /// <param name="orgIds">机构Id列表</param>
    /// <returns></returns>

    public List<SysOrg> GetParentListByIds(List<SysOrg> allOrgList, List<long> orgIds)
    {
        HashSet<SysOrg> sysOrgs = new HashSet<SysOrg>();//结果列表
                                                        //遍历组织ID
        orgIds.ForEach(it =>
        {
            //获取该组织ID的所有父级
            var parents = GetOrgParents(allOrgList, it);
            sysOrgs.AddRange(parents);//添加到结果
        });
        return sysOrgs.ToList();
    }


    /// <summary>
    /// 获取组织所有下级
    /// </summary>
    /// <param name="orgList">组织列表</param>
    /// <param name="parentId">父ID</param>
    /// <returns></returns>
    public List<SysOrg> GetSysOrgChilden(List<SysOrg> orgList, long parentId)
    {
        //找下级组织ID列表
        var orgs = orgList.Where(it => it.ParentId == parentId).ToList();
        if (orgs.Count > 0)//如果数量大于0
        {
            var data = new List<SysOrg>();
            foreach (var item in orgs)//遍历组织
            {
                var childen = GetSysOrgChilden(orgList, item.Id);//获取子节点
                data.AddRange(childen);//添加子节点);
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<SysOrg>();
    }


    /// <summary>
    /// 赋值组织的所有下级
    /// </summary>
    /// <param name="orgList">组织列表</param>
    /// <param name="parentId">父Id</param>
    /// <param name="newParentId">新父Id</param>
    /// <returns></returns>
    public List<SysOrg> CopySysOrgChilden(List<SysOrg> orgList, long parentId, long newParentId)
    {
        //找下级组织列表
        var orgs = orgList.Where(it => it.ParentId == parentId).ToList();
        if (orgs.Count > 0)//如果数量大于0
        {
            var data = new List<SysOrg>();
            var newId = YitIdHelper.NextId();
            foreach (var item in orgs)//遍历组织
            {
                var childen = CopySysOrgChilden(orgList, item.Id, newId);//获取子节点
                data.AddRange(childen);//添加子节点);
                RedirectOrg(item);//实体重新赋值
                item.ParentId = newParentId;//赋值父Id
                data.Add(item);//添加到列表
            }
            return data;//返回结果
        }
        return new List<SysOrg>();
    }

    /// <summary>
    /// 重新生成组织实体
    /// </summary>
    /// <param name="org"></param>
    private void RedirectOrg(SysOrg org)
    {
        //重新生成ID并赋值
        var newId = YitIdHelper.NextId();
        org.Id = newId;
        org.Code = RandomHelper.CreateRandomString(10);
        org.CreateTime = DateTime.Now;
        org.CreateUser = UserManager.UserAccount;
        org.CreateUserId = UserManager.UserId;

    }
    #endregion



}
