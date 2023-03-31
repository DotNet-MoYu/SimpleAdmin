


namespace SimpleAdmin.Plugin.Gen;

/// <summary>
/// <inheritdoc cref="IGenbasicService"/>
/// </summary>
public class GenBasicService : DbRepository<GenBasic>, IGenbasicService
{
    private readonly ILogger<GenBasicService> _logger;
    private readonly IViewEngine _viewEngine;
    private readonly IGenConfigSerivce _genConfigSerivce;
    private readonly IMenuService _menuService;
    private readonly IButtonService _buttonService;
    private readonly IRoleService _roleService;
    private string _sqlDir = "Sql";
    private string _backendDir = "Backend";
    private string _frontDir = "Frontend";

    public GenBasicService(ILogger<GenBasicService> logger,
                           IViewEngine viewEngine,
                           IGenConfigSerivce genConfigSerivce,
                           IMenuService menuService,
                           IButtonService buttonService, IRoleService roleService)
    {
        this._logger = logger;
        this._viewEngine = viewEngine;
        this._genConfigSerivce = genConfigSerivce;
        this._menuService = menuService;
        this._buttonService = buttonService;
        this._roleService = roleService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<GenBasic>> Page(BasePageInput input)
    {

        var query = Context.Queryable<GenBasic>()
                         .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
                         .OrderBy(it => it.SortCode);
        var pageInfo = await query.ToPagedListAsync(input.Current, input.Size);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public List<SqlSugarTableInfo> GetTables()
    {
        return SqlSugarUtils.GetTablesByAttribute<CodeGenAttribute>();
    }


    /// <inheritdoc/>
    public List<string> GetAssemblies()
    {
        var names = new List<string>();//结果集
        var excludeKeys = new List<string>() { "Furion", "Core", "Plugin" };
        //获取所有程序集名称
        var assemblies = App.Assemblies
            .Where(it => !it.FullName.StartsWith("Furion"))
            .Select(it => it.FullName).ToList();
        assemblies.ForEach(it => names.Add(it.Split(",")[0]));//根据逗号分割取第一个
        return names;
    }


    /// <inheritdoc/>
    public async Task<GenBasic> Add(GenBasicAddInput input)
    {
        var entity = input.Adapt<GenBasic>();//输入参数转实体
        var tableColumns = SqlSugarUtils.GetTableColumns(input.ConfigId, input.DbTable);//获取表的字段信息
        List<GenConfig> genConfigs = new List<GenConfig>();//代码生成配置字段集合
        //遍历字段
        int sortCode = 0;
        //遍历字段
        tableColumns.ForEach(it =>
        {
            #region 判断是否想显示
            var yesOrNo = GenConst.Yes;
            if (it.IsPrimarykey || SqlSugarUtils.IsCommonColumn(it.ColumnName))//如果是主键或者是公共字段则不显示
                yesOrNo = GenConst.No;

            else
                yesOrNo = GenConst.Yes;
            #endregion
            //添加到字段集合
            genConfigs.Add(new GenConfig
            {
                IsPrimarykey = it.IsPrimarykey ? GenConst.Yes : GenConst.No,
                FieldName = it.ColumnName,
                FieldType = it.DataType,
                FieldNetType = SqlSugarUtils.ConvertDataType(it.DataType),
                FieldRemark = it.ColumnDescription ?? it.ColumnName,
                EffectType = EffTypeConst.INPUT,
                WhetherTable = yesOrNo,
                WhetherAddUpdate = yesOrNo,
                WhetherRequired = GenConst.No,
                WhetherRetract = GenConst.No,
                QueryWhether = GenConst.No,
                SortCode = yesOrNo == GenConst.No ? 99 : sortCode//如果是公共字段就排最后
            });
            sortCode++;
        });
        if (!genConfigs.Any(it => it.IsPrimarykey == GenConst.Yes))//判断是否有主键
        {
            throw Oops.Bah("要生成的表必须有主键");
        }
        //事务
        var result = await itenant.UseTranAsync(async () =>
        {
            entity = await InsertReturnEntityAsync(entity);//输入参数转实体并插入
            genConfigs.ForEach(it => { it.BasicId = entity.Id; });//遍历字段赋值基础Id
            await Context.Insertable(genConfigs).ExecuteCommandAsync();

        });
        if (!result.IsSuccess)//如果失败了
        {
            //写日志
            _logger.LogError(result.ErrorMessage, result.ErrorException);
            throw Oops.Oh(ErrorCodeEnum.A0003);
        }

        return entity;
    }


    /// <inheritdoc/>
    public async Task Delete(List<BaseIdInput> input)
    {
        //获取所有ID
        var ids = input.Select(it => it.Id).ToList();
        if (ids.Count > 0)
        {
            //事务
            var result = await itenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除基础表
                await Context.Deleteable<GenConfig>().Where(it => ids.Contains(it.BasicId)).ExecuteCommandAsync();//删除配置表

            });
            if (!result.IsSuccess)//如果失败了
            {
                //写日志
                _logger.LogError(result.ErrorMessage, result.ErrorException);
                throw Oops.Oh(ErrorCodeEnum.A0002);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<GenBasic> Edit(GenBasicEditInput input)
    {
        var entity = input.Adapt<GenBasic>();//输入参数转实体
        await UpdateAsync(entity);
        return entity;

    }

    /// <inheritdoc/>
    public async Task<GenBasePreviewOutput> PreviewGen(BaseIdInput input)
    {
        var genBasic = await GetGenBasic(input.Id);//获取代码生成基础
        return await PreviewGen(genBasic);
    }


    /// <inheritdoc/>
    public async Task ExecGenPro(BaseIdInput input)
    {
        var genBasic = await GetGenBasic(input.Id);//获取代码生成基础
        if (genBasic.GenerateType != GenConst.Pro) throw Oops.Bah("当前配置生成方式为：项目中");
        if (await CreateMenuButtonAndRelation(genBasic))
        {
            var previewCode = await PreviewGen(genBasic);//获取代码生成预览
            var backendPath = Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent.FullName);//获取主工程目录
            ExecBackend(previewCode.CodeBackendResults, genBasic, backendPath);//执行后端代码生成
            var srcDir = "src";//默认都是代码放在src文件夹
            var frontedPath = genBasic.FrontedPath;//获取前端代码路径,
            if (!frontedPath.Contains(srcDir))//如果不包含src
                frontedPath = genBasic.FrontedPath.CombinePath(srcDir);
            ExecFronted(previewCode.CodeFrontendResults, genBasic, frontedPath);
        }
        else
        {
            throw Oops.Bah("代码生成失败");
        }
    }


    /// <inheritdoc/>
    public async Task<FileStreamResult> ExecGenZip(BaseIdInput input)
    {
        var genBasic = await GetGenBasic(input.Id);//获取代码生成基础
        if (genBasic.GenerateType != GenConst.Zip) throw Oops.Bah("当前配置生成方式为：压缩包");
        var temDir = Path.GetTempPath().CombinePath(genBasic.ClassName);//获取临时目录并用类名做存放代码文件文件夹
        File.Delete(temDir + ".zip");// 先删除压缩包
        var previewCode = await PreviewGen(genBasic);//获取代码生成预览
        ExecBackend(previewCode.CodeBackendResults, genBasic, temDir.CombinePath(_backendDir), true);//执行后端代码生成
        ExecFronted(previewCode.CodeFrontendResults, genBasic, temDir.CombinePath(_frontDir));//执行前端生成
        ExecSql(previewCode.SqlResults, temDir);//执行sql生成
        var zipPath = ZipUtils.CompressDirectory(temDir, true);//压缩文件夹
        var result = new FileStreamResult(new FileStream(zipPath, FileMode.Open), "application/octet-stream") { FileDownloadName = $"{genBasic.ClassName}.zip" };
        return result;
    }
    #region 方法

    /// <summary>
    /// 生成后端代码文件
    /// </summary>
    /// <param name="baseCodeResults">后端代码模板</param>
    /// <param name="genBasic">代码基础</param>
    /// <param name="backendPath">后端生成路径</param>
    /// <param name="isZip">是否是zip方式</param>
    public void ExecBackend(List<GenBasePreviewOutput.GenBaseCodeResult> baseCodeResults, GenBasic genBasic, string backendPath, bool isZip = false)
    {
        var serviceDir = "Services";//服务代码文件夹
        var controllerDir = "Controllers";//服务代码文件夹
        var tempDicName = "";//临时文件夹名称
        HashSet<string> dirList = new HashSet<string> { };//文件夹列表
        var postion = "";//文件夹位置
        var parentDir = _backendDir; //父文件文件夹名称
        var serviceName = genBasic.ServicePosition.Split(".").Last();//取服务层最后一个单词命名为接口层控制器下的文件夹
        baseCodeResults.ForEach(it =>
        {
            var fileInfo = new FileInfo(it.FilePath);//获取文件信息
            var dirName = fileInfo.Directory.Name;//获取文件文件夹名称
            var parentName = fileInfo.Directory.Parent.Name;//获取父文件文件夹名称
            if (dirName == controllerDir)//如果是控制器
            {
                dirList.Add(dirName);//添加到当前文件夹到文件夹列表
                dirList.Add(serviceName);//添加服务层目录名到文件夹列表
                postion = genBasic.ControllerPosition;//设置文件夹创建位置为Api层
            }
            else if (dirName == serviceDir)//如果是服务
            {
                dirList.Remove(controllerDir);//删除接口文件夹
                dirList.Remove(serviceName);//删除服务层目录
                dirList.Add(genBasic.ClassName);//添加ClassName到文件夹列表
                dirList.Add(dirName);//添加到当前文件夹到文件夹列表
                postion = genBasic.ServicePosition;//设置文件夹创建位置为服务层
            }
            if (parentDir != parentName)//如果当前文件的父文件夹不等于partDir表示层级有变动
            {
                dirList.Add(dirName);//添加到文件夹列表
                parentDir = parentName;//重新赋值父文件夹
            }
            else
            {
                if (tempDicName != dirName)//如果临时文件夹不是当前文件夹表示同级目录下的另一个文件夹
                {
                    dirList.Remove(tempDicName);//删除上一个文件夹
                    dirList.Add(dirName);//添加当前文件夹到文件夹列表
                }
            }
            tempDicName = dirName;//给临时文件夹赋值
            var path = backendPath;//后端文件目录
            if (!isZip) path = backendPath.CombinePath(postion);//如果不是zip方式就加上命名空间
            path = path.CombinePath(dirList.ToArray());
            if (!Directory.Exists(path))//如果文件夹不存在就创建文件夹
                Directory.CreateDirectory(path);
            //var fileName = genBasic.ClassName + it.CodeFileName;//文件名等于类名加代码文件名
            //if (it.CodeFileName.StartsWith("IService"))
            //    fileName = $"I{genBasic.ClassName}Service.cs";//对IService接口要特殊处理
            //path = path.CombinePath(fileName);//最终生成文件地址
            File.WriteAllText(path.CombinePath(it.CodeFileName), it.CodeFileContent, Encoding.UTF8);//写入文件
        });

    }


    /// <summary>
    /// 生成前端代码文件
    /// </summary>
    /// <param name="baseCodeResults">前端代码模板</param>
    /// <param name="genBasic">代码基础</param>
    /// <param name="frontedPath">前端生成路径</param>
    public void ExecFronted(List<GenBasePreviewOutput.GenBaseCodeResult> baseCodeResults, GenBasic genBasic, string frontedPath)
    {
        var apiDir = "api";
        var viewDir = "views";
        var parentDir = _frontDir; //父文件文件夹名称
        HashSet<string> dirList = new HashSet<string> { };//文件夹列表
        baseCodeResults.ForEach(it =>
       {
           var fileInfo = new FileInfo(it.FilePath);//获取文件信息
           var dirName = fileInfo.Directory.Name;//获取文件文件夹名称
           var parentName = fileInfo.Directory.Parent.Name;//获取父文件文件夹名称
           var path = frontedPath.CombinePath(fileInfo.Directory.Name).CombinePath(genBasic.RouteName);//生成路径为前端路径+代码文件所在文件夹+路由地址
           if (dirName == apiDir)//如果是api文件夹
               it.CodeFileName = StringHelper.FirstCharToLower(genBasic.ClassName) + it.CodeFileName;//文件名等于路由名加类名加代码文件名
           else if (dirName == viewDir)
               path = path.CombinePath(genBasic.BusName);
           if (!Directory.Exists(path))//如果文件夹不存在就创建文件夹
               Directory.CreateDirectory(path);
           File.WriteAllText(path.CombinePath(it.CodeFileName), it.CodeFileContent, Encoding.UTF8);//写入文件
       });

    }

    /// <summary>
    /// 生成sql代码文件
    /// </summary>
    /// <param name="baseCodeResults">后端代码模板</param>
    /// <param name="execPath">生成路径</param>
    public void ExecSql(List<GenBasePreviewOutput.GenBaseCodeResult> baseCodeResults, string execPath)
    {
        execPath = execPath.CombinePath(_sqlDir);//写在sql目录里面
        if (!Directory.Exists(execPath))//如果文件夹不存在就创建文件夹
            Directory.CreateDirectory(execPath);
        baseCodeResults.ForEach(it =>
       {
           File.WriteAllText(execPath.CombinePath(it.CodeFileName), it.CodeFileContent, Encoding.UTF8);//写入文件
       });
    }

    /// <summary>
    /// 获取sql代码预览结果
    /// </summary>
    /// <param name="genViewModel">视图</param>
    /// <param name="templatePath">模板路径</param>
    /// <returns></returns>
    public async Task<List<GenBasePreviewOutput.GenBaseCodeResult>> GetSqlCodeResult(GenViewModel genViewModel, string templatePath)
    {
        var sqlCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var sqlTemplatePath = Path.Combine(templatePath, _sqlDir);//获取sql模板文件路径

        FileInfo[] files = GetAllFileInfo(sqlTemplatePath);
        foreach (FileInfo fileInfo in files)
        {

            var fileName = fileInfo.Name;//文件名
            var fileNoPrefix = fileName.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            string? tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            sqlCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = fileNoPrefix,
                FilePath = fileInfo.FullName
            });
        }


        return sqlCodeResults;

    }

    /// <summary>
    /// 获取后端代码预览结果
    /// </summary>
    /// <param name="genViewModel">视图</param>
    /// <param name="templatePath">模板路径</param>
    /// <returns></returns>
    public async Task<List<GenBasePreviewOutput.GenBaseCodeResult>> GetBackendCodeResult(GenViewModel genViewModel, string templatePath)
    {
        var backendCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var backendTemplatePath = Path.Combine(templatePath, _backendDir);//获取后端模板文件路径
        FileInfo[] files = GetAllFileInfo(backendTemplatePath);
        //files = files.Where(it => it.Name == "Service.cs.vm").ToArray();//测试用
        foreach (var fileInfo in files)
        {
            var nameWithPrefix = fileInfo.Name;//文件名
            var fileNoPrefix = nameWithPrefix.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var fileName = genViewModel.ClassName + fileNoPrefix;//文件名等于类名加代码文件名
            if (fileNoPrefix.StartsWith("IService"))
                fileName = $"I{genViewModel.ClassName}Service.cs";//对IService接口要特殊处理
            string? tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            backendCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = fileName,
                FilePath = fileInfo.FullName
            });
        }

        return backendCodeResults;

    }

    /// <summary>
    /// 获取前端代码预览结果
    /// </summary>
    /// <param name="genViewModel">视图</param>
    /// <param name="templatePath">模板路径</param>
    /// <returns></returns>
    public async Task<List<GenBasePreviewOutput.GenBaseCodeResult>> GetForntCodeResult(GenViewModel genViewModel, string templatePath)
    {

        var frontCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var frontTemplatePath = Path.Combine(templatePath, _frontDir);//获取前端模板文件路径
        FileInfo[] files = GetAllFileInfo(frontTemplatePath);
        //files = files.Where(it => it.Name == "form.vue.vm").ToArray();//测试用
        foreach (var fileInfo in files)
        {
            var fileName = fileInfo.Name;//文件名
            var fileNoPrefix = fileName.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            frontCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = fileNoPrefix,
                FilePath = fileInfo.FullName
            });
        }
        return frontCodeResults;
    }


    /// <summary>
    /// 获取代码生成视图
    /// </summary>
    /// <param name="genBasic"></param>
    /// <returns></returns>
    public async Task<GenViewModel> GetGenViewModel(GenBasic genBasic)
    {
        //实体转视图
        var genViewEngine = genBasic.Adapt<GenViewModel>();
        //获取字段信息
        var tableFieldList = await _genConfigSerivce.List(genBasic.Id);
        tableFieldList.ForEach(it =>
        {
            it.FieldNameFirstLower = StringHelper.FirstCharToLower(it.FieldName);//首字母小写
            it.FieldNameFirstUpper = StringHelper.FirstCharToUpper(it.FieldName);//首字母大写
        });
        genViewEngine.TableFields = tableFieldList;//赋值表字段信息

        return genViewEngine;
    }

    /// <summary>
    /// 视图渲染
    /// </summary>
    /// <param name="tContent">模板内容</param>
    /// <param name="genViewModel">参数</param>
    /// <returns></returns>
    public async Task<string> GetViewEngine(string tContent, GenViewModel genViewModel)
    {
        //视图引擎渲染
        var tResult = await _viewEngine.RunCompileFromCachedAsync(tContent, genViewModel, builderAction: builder =>
        {
            builder.AddAssemblyReference(typeof(GenBasic));//添加程序集
            builder.AddAssemblyReferenceByName("System.Collections");//添加程序集
        });
        return tResult;
    }

    /// <summary>
    /// 获得文件夹下所有的文件
    /// </summary>
    /// <param name="path">文件夹路径</param>
    /// <returns></returns>
    private FileInfo[] GetAllFileInfo(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        return dir.GetFiles(".", SearchOption.AllDirectories);

    }

    /// <inheritdoc/>
    public async Task<GenBasePreviewOutput> PreviewGen(GenBasic genBasic)
    {
        var basePath = AppContext.BaseDirectory;//获取项目目录
        var templatePath = basePath.CombinePath("CodeGen");//获取文件路径
        //var templatePath = App.WebHostEnvironment.WebRootPath + @"\CodeGen";//模板文件文件夹
        var genViewModel = await GetGenViewModel(genBasic);
        var frontendResult = await GetForntCodeResult(genViewModel, templatePath);
        var backendResult = await GetBackendCodeResult(genViewModel, templatePath);
        var sqlResult = await GetSqlCodeResult(genViewModel, templatePath);
        return new GenBasePreviewOutput
        {
            CodeBackendResults = backendResult,
            CodeFrontendResults = frontendResult,
            SqlResults = sqlResult
        };
    }

    /// <summary>
    /// 获取代码生成基础
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private async Task<GenBasic> GetGenBasic(long id)
    {
        var genBasic = await GetByIdAsync(id);
        if (genBasic == null)
            throw Oops.Bah("代码生成配置不存在");
        return genBasic;

    }

    /// <summary>
    /// 创建代码生成相关的菜单按钮和授权关系
    /// </summary>
    /// <param name="genBasic"></param>
    /// <returns></returns>
    private async Task<bool> CreateMenuButtonAndRelation(GenBasic genBasic)
    {
        try
        {

            #region 菜单
            var title = genBasic.FunctionName + genBasic.FunctionNameSuffix;
            var menuRep = ChangeRepository<DbRepository<SysResource>>();//切换仓储
                                                                        //获取已经存在的旧菜单
            var oldMenu = await menuRep.
                GetFirstAsync(it => it.Title == title
                && it.Category == CateGoryConst.Resource_MENU
                && it.MenuType == ResourceConst.MENU);
            if (oldMenu != null)//如果存在就直接和删除（同时删除其下面的菜单、按钮，清除对应的角色与资源信息)
                await _menuService.Delete(new List<BaseIdInput> { new BaseIdInput { Id = oldMenu.Id } });
            //添加菜单参数
            MenuAddInput menu = new MenuAddInput
            {
                Id = CommonUtils.GetSingleId(),
                ParentId = genBasic.MenuPid,
                Title = title,
                Category = CateGoryConst.Resource_MENU,
                Module = genBasic.Module,
                Icon = genBasic.Icon,
                Name = genBasic.BusName,
                Code = RandomHelper.CreateRandomString(10),
                Path = $"/{genBasic.RouteName}/{genBasic.BusName}",
                Component = $"{genBasic.RouteName}/{genBasic.BusName}/index",
                SortCode = 99
            };
            await _menuService.Add(menu);//添加菜单
            #endregion
            #region 按钮
            //添加按钮参数
            ButtonAddInput button = new ButtonAddInput
            {
                Title = genBasic.FunctionName,
                ParentId = menu.Id,
                Code = StringHelper.FirstCharToLower(genBasic.ClassName)
            };
            var buttonIds = await _buttonService.AddBatch(button);//添加按钮
            #endregion
            #region 角色授权
            var roleRep = ChangeRepository<DbRepository<SysRole>>();//切换仓储
            var superAdmin = await roleRep.GetFirstAsync(it => it.Code == RoleConst.SuperAdmin && it.Category == CateGoryConst.Role_GLOBAL);//获取超级管理员
                                                                                                                                            //授权菜单参数
            GrantResourceInput grantResource = new GrantResourceInput
            {
                Id = superAdmin.Id,
                GrantInfoList = new List<RelationRoleResuorce> { new RelationRoleResuorce { MenuId = menu.Id, ButtonInfo = buttonIds } },
                IsCodeGen = true,
            };
            await _roleService.GrantResource(grantResource);//授权菜单
            #endregion
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"代码生成创建菜单和授权报错{ex.Message}", ex.InnerException);
            return false;
        }
    }


    #endregion
}
