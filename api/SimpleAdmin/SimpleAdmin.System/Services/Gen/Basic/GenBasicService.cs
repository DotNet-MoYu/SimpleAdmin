namespace SimpleAdmin.System;

/// <summary>
/// <inheritdoc cref="IGenbasicService"/>
/// </summary>
public class GenBasicService : DbRepository<GenBasic>, IGenbasicService
{
    private readonly ILogger<GenBasicService> _logger;
    private readonly IViewEngine _viewEngine;
    private readonly IGenConfigService _genConfigService;
    private readonly IMenuService _menuService;
    private readonly IButtonService _buttonService;
    private readonly ISysRoleService _roleService;
    private readonly string _sqlDir = "Sql";
    private readonly string _backendDir = "Backend";
    private readonly string _frontDir = "Frontend";
    private const string _interfaceDir = "interface";
    private const string _modulesDir = "modules";
    private const string _indexTs = "index.ts";

    public GenBasicService(ILogger<GenBasicService> logger,
        IViewEngine viewEngine,
        IGenConfigService genConfigService,
        IMenuService menuService,
        IButtonService buttonService, ISysRoleService roleService)
    {
        _logger = logger;
        _viewEngine = viewEngine;
        _genConfigService = genConfigService;
        _menuService = menuService;
        _buttonService = buttonService;
        _roleService = roleService;
    }

    /// <inheritdoc/>
    public async Task<SqlSugarPagedList<GenBasic>> Page(BasePageInput input)
    {
        var query = Context.Queryable<GenBasic>()
            .OrderByIF(!string.IsNullOrEmpty(input.SortField), $"{input.SortField} {input.SortOrder}")//排序
            .OrderBy(it => it.SortCode, OrderByType.Desc)//默认排序
            .OrderBy(it => it.CreateTime, OrderByType.Desc)//默认排序
            .Mapper(it =>
            {
                it.FuncList = it.Functions.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();//功能集合转为列表
            });
        var pageInfo = await query.ToPagedListAsync(input.PageNum, input.PageSize);//分页
        return pageInfo;
    }

    /// <inheritdoc/>
    public List<SqlSugarTableInfo> GetTables(bool isAll = false)
    {
        if (isAll)
            return SqlSugarUtils.GetTablesByAttribute<SugarTable>();
        return SqlSugarUtils.GetTablesByAttribute<CodeGenAttribute>();
    }



    /// <inheritdoc/>
    public List<string> GetAssemblies()
    {
        var names = new List<string>();//结果集
        var excludeList = new List<string>
        {
            "Web.Entry", "Core", "Cache", "SqlSugar", "Plugin.Core", "Plugin.Aop"
        };//排除的程序集
        //获取所有程序集名称
        var assemblies = App.Assemblies
            .Where(it => !it.FullName.StartsWith("MoYu"))
            .Select(it => it.FullName).ToList();
        assemblies.ForEach(it =>
        {
            var name = it.Split(",")[0];//根据逗号分割取第一个
            //根据.分割获取最后两个


            var projectName = string.Join(".", name.Split(".").Skip(1).ToList());
            if (!excludeList.Contains(projectName))//去掉排除的程序集
            {
                names.Add(name);
            }
        });
        return names;
    }


    /// <inheritdoc/>
    public async Task<GenBasic> Add(GenBasicAddInput input)
    {
        var entity = input.Adapt<GenBasic>();//输入参数转实体
        entity.Functions = string.Join(",", input.FuncList);//功能集合
        var tableColumns = SqlSugarUtils.GetTableColumns(input.ConfigId, input.DbTable);//获取表的字段信息
        var genConfigs = new List<GenConfig>();//代码生成配置字段集合
        //遍历字段
        var sortCode = 0;
        //遍历字段
        tableColumns.ForEach(it =>
        {
            #region 判断是否想显示

            var yesOrNo = GenConst.Yes;
            if (it.IsPrimaryKey || SqlSugarUtils.IsCommonColumn(it.ColumnName))//如果是主键或者是公共字段则不显示
                yesOrNo = GenConst.No;
            else
                yesOrNo = GenConst.Yes;

            #endregion 判断是否想显示

            //添加到字段集合
            genConfigs.Add(new GenConfig
            {
                IsPrimaryKey = it.IsPrimaryKey ? GenConst.Yes : GenConst.No,
                FieldName = it.ColumnName,
                FieldType = it.DataType,
                FieldNetType = SqlSugarUtils.ConvertDataType(it.DataType),
                FieldRemark = it.ColumnDescription ?? it.ColumnName,
                EffectType = EffTypeConst.INPUT,
                Width = 100,
                WhetherTable = yesOrNo,
                WhetherAddUpdate = yesOrNo,
                WhetherImportExport = yesOrNo,
                WhetherResizable = GenConst.No,
                WhetherRequired = GenConst.No,
                WhetherRetract = GenConst.No,
                QueryWhether = GenConst.No,
                SortCode = yesOrNo == GenConst.No ? 99 : sortCode//如果是公共字段就排最后
            });
            sortCode++;
        });
        var index = 0;
        genConfigs = genConfigs.OrderBy(it => it.SortCode).ToList();//排序
        genConfigs.ForEach(it =>
        {
            it.FieldIndex = index;
            index++;
        });
        if (!genConfigs.Any(it => it.IsPrimaryKey == GenConst.Yes))//判断是否有主键
        {
            throw Oops.Bah("要生成的表必须有主键");
        }

        //事务
        var result = await Tenant.UseTranAsync(async () =>
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
    public async Task Delete(BaseIdListInput input)
    {
        //获取所有ID
        var ids = input.Ids.ToList();
        if (ids.Count > 0)
        {
            //事务
            var result = await Tenant.UseTranAsync(async () =>
            {
                await DeleteByIdsAsync(ids.Cast<object>().ToArray());//删除基础表
                await Context.Deleteable<GenConfig>().Where(it => ids.Contains(it.BasicId))
                    .ExecuteCommandAsync();//删除配置表
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
        entity.Functions = string.Join(",", input.FuncList);//功能集合
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
    public async Task ExecGenPro(ExecGenInput input)
    {
        var genBasic = await GetGenBasic(input.Id);//获取代码生成基础
        if (genBasic.GenerateType != GenConst.Pro) throw Oops.Bah("当前配置生成方式为：项目中");
        var backendPath =
            Path.Combine(new DirectoryInfo(App.WebHostEnvironment.ContentRootPath).Parent.FullName);//获取主工程目录
        if (await CreateMenuButtonAndRelation(genBasic))
        {
            var previewCode = await PreviewGen(genBasic);//获取代码生成预览
            if (input.ExecType != GenConst.ExecAll)//如果不是全部执行
            {
                if (input.ExecType == GenConst.ExecBackend)
                    ExecBackend(previewCode.CodeBackendResults);//执行后端代码生成
                else if (input.ExecType == GenConst.ExecFrontend)
                    ExecFronted(previewCode.CodeFrontendResults);//执行前端代码生成
            }
            else
            {
                ExecBackend(previewCode.CodeBackendResults);//执行后端代码生成

                ExecFronted(previewCode.CodeFrontendResults);//执行前端代码生成
            }
        }
        else
        {
            throw Oops.Bah("代码生成失败");
        }
    }

    /// <inheritdoc/>
    public async Task<FileStreamResult> ExecGenZip(ExecGenInput input)
    {
        var genBasic = await GetGenBasic(input.Id);//获取代码生成基础
        // if (genBasic.GenerateType != GenConst.Zip) throw Oops.Bah("当前配置生成方式为：压缩包");
        var temDir = Path.GetTempPath().CombinePath(genBasic.ClassName);//获取临时目录并用类名做存放代码文件文件夹
        File.Delete(temDir + ".zip");// 先删除压缩包
        var previewCode = await PreviewGen(genBasic);//获取代码生成预览
        if (input.ExecType != GenConst.ExecAll)//如果不是全部执行
        {
            if (input.ExecType == GenConst.ExecBackend)
            {
                ExecBackend(previewCode.CodeBackendResults, temDir.CombinePath(_backendDir), true);//执行后端代码生成
                ExecSql(previewCode.SqlResults, temDir);//执行sql生成
            }
            else if (input.ExecType == GenConst.ExecFrontend)
                ExecFronted(previewCode.CodeFrontendResults, temDir.CombinePath(_frontDir));//执行前端生成
        }
        else
        {
            ExecBackend(previewCode.CodeBackendResults, temDir.CombinePath(_backendDir), true);//执行后端代码生成
            ExecFronted(previewCode.CodeFrontendResults, temDir.CombinePath(_frontDir));//执行前端生成
            ExecSql(previewCode.SqlResults, temDir);//执行sql生成
        }
        var zipPath = ZipUtils.CompressDirectory(temDir, true);//压缩文件夹
        var result = new FileStreamResult(new FileStream(zipPath, FileMode.Open), "application/octet-stream")
            { FileDownloadName = $"{genBasic.ClassName}.zip" };
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
    // public void ExecBackend(List<GenBasePreviewOutput.GenBaseCodeResult> baseCodeResults, GenBasic genBasic,
    //     string backendPath, bool isZip = false)
    // {
    //     var serviceDir = "Services";//服务代码文件夹
    //     var controllerDir = "Controllers";//服务代码文件夹
    //     var tempDicName = "";//临时文件夹名称
    //     var dirList = new HashSet<string> { };//文件夹列表
    //     var postion = "";//文件夹位置
    //     var parentDir = _backendDir;//父文件文件夹名称
    //     var serviceName = genBasic.ServicePosition.Split(".").Last();//取服务层最后一个单词命名为接口层控制器下的文件夹
    //     baseCodeResults.ForEach(it =>
    //     {
    //         var fileInfo = new FileInfo(it.FilePath);//获取文件信息
    //         var dirName = fileInfo.Directory.Name;//获取文件文件夹名称
    //         var parentName = fileInfo.Directory.Parent.Name;//获取父文件文件夹名称
    //         if (dirName == controllerDir)//如果是控制器
    //         {
    //             dirList.Add(dirName);//添加到当前文件夹到文件夹列表
    //             dirList.Add(serviceName);//添加服务层目录名到文件夹列表
    //             postion = genBasic.ControllerPosition;//设置文件夹创建位置为Api层
    //         }
    //         else if (dirName == serviceDir)//如果是服务
    //         {
    //             dirList.Remove(controllerDir);//删除接口文件夹
    //             dirList.Remove(serviceName);//删除服务层目录
    //             dirList.Add(genBasic.ClassName);//添加ClassName到文件夹列表
    //             dirList.Add(dirName);//添加到当前文件夹到文件夹列表
    //             postion = genBasic.ServicePosition;//设置文件夹创建位置为服务层
    //         }
    //
    //         if (parentDir != parentName)//如果当前文件的父文件夹不等于partDir表示层级有变动
    //         {
    //             dirList.Add(dirName);//添加到文件夹列表
    //             parentDir = parentName;//重新赋值父文件夹
    //         }
    //         else
    //         {
    //             if (tempDicName != dirName)//如果临时文件夹不是当前文件夹表示同级目录下的另一个文件夹
    //             {
    //                 dirList.Remove(tempDicName);//删除上一个文件夹
    //                 dirList.Add(dirName);//添加当前文件夹到文件夹列表
    //             }
    //         }
    //
    //         tempDicName = dirName;//给临时文件夹赋值
    //         var path = backendPath;//后端文件目录
    //         if (!isZip) path = backendPath.CombinePath(postion);//如果不是zip方式就加上命名空间
    //         path = path.CombinePath(dirList.ToArray());
    //         if (!Directory.Exists(path))//如果文件夹不存在就创建文件夹
    //             Directory.CreateDirectory(path);
    //         //var fileName = genBasic.ClassName + it.CodeFileName;//文件名等于类名加代码文件名
    //         //if (it.CodeFileName.StartsWith("IService"))
    //         //    fileName = $"I{genBasic.ClassName}Service.cs";//对IService接口要特殊处理
    //         //path = path.CombinePath(fileName);//最终生成文件地址
    //         File.WriteAllText(path.CombinePath(it.CodeFileName), it.CodeFileContent, Encoding.UTF8);//写入文件
    //     });
    // }
    public void ExecBackend(List<GenBasePreviewOutput.GenBaseCodeResult> baseCodeResults, string backendPath = null, bool isZip = false)
    {
        //遍历baseCodeResults
        baseCodeResults.ForEach(it =>
        {
            var path = it.FilePath;
            //判断path所属的文件夹
            var dir = Path.GetDirectoryName(path);
            //如果backendPath不为空，则将path设置为backendPath.CombinePath($"{it.CodeFileName}")
            if (backendPath != null)
            {
                path = backendPath.CombinePath($"{it.CodeFileName}");
                //重新获取path所属的文件夹
                dir = Path.GetDirectoryName(path);
            }
            //如果不存在就创建文件夹
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, it.CodeFileContent, Encoding.UTF8);//写入文件
        });
    }

    /// <summary>
    /// 生成前端代码文件
    /// </summary>
    /// <param name="baseCodeResults">前端代码模板</param>
    /// <param name="frontedPath">前端生成路径</param>
    public void ExecFronted(List<GenBasePreviewOutput.GenBaseCodeResult> baseCodeResults, string frontedPath = null)
    {
        baseCodeResults.ForEach(it =>
        {
            var path = it.FilePath;
            //判断path所属的文件夹
            var dir = Path.GetDirectoryName(path);
            if (frontedPath != null)
            {
                path = frontedPath.CombinePath($"{it.CodeFileName}");
                dir = Path.GetDirectoryName(path);
            }

            //如果不存在就创建文件夹
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(path, it.CodeFileContent, Encoding.UTF8);//写入文件
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
    public async Task<List<GenBasePreviewOutput.GenBaseCodeResult>> GetSqlCodeResult(GenViewModel genViewModel,
        string templatePath)
    {
        var sqlCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var sqlTemplatePath = Path.Combine(templatePath, _sqlDir);//获取sql模板文件路径

        var files = GetAllFileInfo(sqlTemplatePath);
        foreach (var fileInfo in files)
        {
            var fileName = fileInfo.Name;//文件名
            var fileNoPrefix = fileName.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
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
    public async Task<List<GenBasePreviewOutput.GenBaseCodeResult>> GetBackendCodeResult(GenViewModel genViewModel,
        string templatePath)
    {
        var backendCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var backendTemplatePath = Path.Combine(templatePath, _backendDir);//获取后端模板文件路径
        var serviceDir = Path.GetDirectoryName(App.WebHostEnvironment.ContentRootPath).CombinePath(genViewModel.ServicePosition);//服务文件夹
        var serviceDicList = genViewModel.ServiceDictionary.Split("/").Where(it => !string.IsNullOrWhiteSpace(it)).ToList();//获取文件夹
        foreach (var item in serviceDicList)
        {
            serviceDir = serviceDir.CombinePath(item);
        }
        serviceDir = serviceDir.CombinePath(genViewModel.ClassName);
        //获取控制器文件夹和后面的路径
        var controllerDir = Path.GetDirectoryName(App.WebHostEnvironment.ContentRootPath).CombinePath(genViewModel.ControllerPosition);
        var controllerDicList = genViewModel.ControllerDictionary.Split("/").Where(it => !string.IsNullOrWhiteSpace(it)).ToList();
        foreach (var item in controllerDicList)
        {
            controllerDir = controllerDir.CombinePath(item);
        }
        controllerDir = controllerDir.CombinePath(genViewModel.ClassName);
        var files = GetAllFileInfo(backendTemplatePath);
        // files = files.Where(it => it.Name == "Service.cs.vm").ToArray();//测试用
        var serviceFiles = files.Where(it => it.FullName.Contains("Service.cs")).ToList();
        var controllerFiles = files.Where(it => it.FullName.Contains("Controller.cs")).ToList();
        var dtoDir = files.Where(it => it.FullName.Contains("Dto")).ToList();
        foreach (var fileInfo in serviceFiles)
        {
            var nameWithPrefix = fileInfo.Name;//文件名
            var fileNoPrefix = nameWithPrefix.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var fileName = genViewModel.ClassName + fileNoPrefix;//文件名等于类名加代码文件名
            if (fileNoPrefix.StartsWith("IService"))
                fileName = $"I{genViewModel.ClassName}Service.cs";//对IService接口要特殊处理
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            backendCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = $"{genViewModel.ClassName}{fileNoPrefix}",
                FilePath = serviceDir.CombinePath(fileName)
            });
        }
        foreach (var fileInfo in controllerFiles)
        {
            var nameWithPrefix = fileInfo.Name;//文件名
            var fileNoPrefix = nameWithPrefix.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var fileName = genViewModel.ClassName + fileNoPrefix;//文件名等于类名加代码文件名
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            backendCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = $"{genViewModel.ClassName}{fileNoPrefix}",
                FilePath = controllerDir.CombinePath(fileName)
            });
        }
        foreach (var fileInfo in dtoDir)
        {
            var nameWithPrefix = fileInfo.Name;//文件名
            var fileNoPrefix = nameWithPrefix.Split(fileInfo.Extension)[0];//不带模板后缀的文件名
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var fileName = genViewModel.ClassName + fileNoPrefix;//文件名等于类名加代码文件名
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            backendCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = $"{genViewModel.ClassName}{fileNoPrefix}",
                FilePath = serviceDir.CombinePath("Dto").CombinePath(fileName)
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
    public async Task<List<GenBasePreviewOutput.GenBaseCodeResult>> GetForntCodeResult(GenViewModel genViewModel,
        string templatePath)
    {
        var frontCodeResults = new List<GenBasePreviewOutput.GenBaseCodeResult>();//结果集
        var frontTemplatePath = Path.Combine(templatePath, _frontDir);//获取前端模板文件路径
        var files = GetAllFileInfo(frontTemplatePath);
        var srcDir = genViewModel.FrontedPath.CombinePath("src");//src文件夹
        var templateVm = "template.vm";//模板文件
        var view = "views";//视图文件夹
        var api = "api";//接口文件夹
        var componentsDir = "components";//组件文件夹
        var apiDir = srcDir.CombinePath(api);
        var viewsDir = srcDir.CombinePath(view);
        var template = files.Where(it => it.Name == templateVm).FirstOrDefault();
        if (templatePath == null) throw Oops.Bah($"{templateVm}文件不存在");
        var dirs = genViewModel.RouteName.Split("/").Where(it => !string.IsNullOrWhiteSpace(it)).ToList();// 根据路/分隔，获取文件夹名字
        //读取模板文件内容,utf-8编码
        var templateContent = File.ReadAllText(template.FullName, Encoding.UTF8);
        var apiFiles = files.Where(it => it.FullName.Contains(api)).ToList();
        var viewFiles = files.Where(it => it.FullName.Contains(view)).ToList();
        foreach (var fileInfo in apiFiles)
        {
            var fileName = fileInfo.Name;//文件名
            //获取文件所在文件夹名字
            var dirName = fileInfo.Directory.Name;
            var fileWhich = fileName.Split(".")[0];//获取是接口还是实现
            if (fileName == templateVm) continue;
            var fileNoPrefix = $"{fileWhich}{genViewModel.RouteName}/{genViewModel.BusName}.ts";//不带模板后缀的文件名
            var filePath = apiDir.CombinePath(fileWhich);//生成文件路径
            //以["biz","ops"]为例,先去找api/interface文件夹下的index.ts文件
            //如果有就在index.ts文件中添加export * from "./biz",然后interfacePath路径就变成了api/interface/biz
            //如果不存在这个文件夹,就表示需要创建这个文件夹,然后在这个文件夹下创建index.ts文件,导入下一层的文件也就是ops
            for (var i = 0; i < dirs.Count; i++)
            {
                var it = dirs[i];//获取文件夹名
                var indexPath = filePath.CombinePath(_indexTs);
                // 判断interfacePath路径下是否有index.ts文件，这里因为是typescript，所以需要引入
                if (File.Exists(indexPath))
                {
                    //读取文件内容
                    var indexContent = File.ReadAllText(indexPath);
                    var content = $"export * from \"./{it}\";";//需要添加的内容
                    //判断content是否存在
                    if (!indexContent.Contains(content))
                    {
                        indexContent += content;
                    }
                    var pathSplit = indexPath.Split($"{fileWhich}")[1];
                    //先通过\分隔再用/合并字符串
                    var codeFileName = $"{fileWhich}{pathSplit}".Split("\\").Aggregate((a, b) => a + "/" + b);
                    //加到结果集
                    frontCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
                    {
                        CodeFileContent = indexContent,
                        CodeFileName = codeFileName,
                        FilePath = indexPath
                    });
                }
                filePath = filePath.CombinePath(it);//拼接文件夹路径

                if (!Directory.Exists(filePath))//如果文件夹不存在
                {
                    var index = filePath.CombinePath(_indexTs);
                    var pathSplit = index.Split($"{fileWhich}")[1];
                    var indexContent = i + 1 == dirs.Count ? genViewModel.BusName : dirs[i + 1];//如果是最后一个就是busName
                    // 在当前内容下添加内容
                    var content = templateContent + $"\n\nexport * from \"./{indexContent}\";";
                    //加到结果集
                    frontCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
                    {
                        CodeFileContent = content,
                        CodeFileName = $"{fileWhich}{pathSplit}",
                        FilePath = index
                    });
                }
            }
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            frontCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = fileNoPrefix,
                FilePath = filePath.CombinePath($"{genViewModel.BusName}.ts")
            });
        }
        //遍历视图文件
        foreach (var fileInfo in viewFiles)
        {
            var fileName = fileInfo.Name;//文件名
            //获取文件所在文件夹名字
            var dirName = fileInfo.Directory.Name;
            var name = fileName.Split(".vm")[0];//不带模板后缀的文件名
            var fileNoPrefix = $"{dirName}/{name}";//不带模板后缀的文件名
            var filePath = viewsDir;
            foreach (var dir in dirs)
            {
                filePath = filePath.CombinePath(dir);
            }
            filePath = filePath.CombinePath(genViewModel.BusName);
            if (dirName == componentsDir)//如果是组件文件夹
            {
                fileNoPrefix = $"{view}/{dirName}/{name}";//不带模板后缀的文件名
                filePath = filePath.CombinePath(componentsDir).CombinePath(name);//生成文件路径
            }
            else
                filePath = filePath.CombinePath(name);//生成文件路径
            var tContent = File.ReadAllText(fileInfo.FullName);//读取文件
            var tResult = await GetViewEngine(tContent, genViewModel);//渲染
            //将渲染结果添加到结果集
            frontCodeResults.Add(new GenBasePreviewOutput.GenBaseCodeResult
            {
                CodeFileContent = tResult,
                CodeFileName = fileNoPrefix,
                FilePath = filePath
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
        var tableFieldList = await _genConfigService.List(genBasic.Id);
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
            builder.AddAssemblyReferenceByName("SimpleTool");//添加程序集
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
        var dir = new DirectoryInfo(path);
        return dir.GetFiles(".", SearchOption.AllDirectories);
    }

    /// <summary>
    /// 代码生成预览
    /// </summary>
    /// <param name="genBasic">基础配置</param>
    /// <returns></returns>
    public async Task<GenBasePreviewOutput> PreviewGen(GenBasic genBasic)
    {
        var basePath = AppContext.BaseDirectory;//获取项目目录
        var templatePath = basePath.CombinePath("ViewEngine").CombinePath("CodeGen");//获取文件路径
        // templatePath = "D:\\胡国东\\SimpleAdmin.Net\\SimpleAdmin\\SimpleAdmin.System\\ViewEngine\\CodeGen";//测试用
        var genViewModel = await GetGenViewModel(genBasic);
        var frontendResult = await GetForntCodeResult(genViewModel, templatePath);
        var backendResult = await GetBackendCodeResult(genViewModel, templatePath);
        var sqlResult = await GetSqlCodeResult(genViewModel, templatePath);
        return new GenBasePreviewOutput
        {
            CodeBackendResults = backendResult,
            SqlResults = sqlResult,
            CodeFrontendResults = frontendResult
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
        genBasic.FuncList = genBasic.Functions.Split(',').ToList();
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
            var oldMenu = await menuRep.GetFirstAsync(it => it.Title == title
                && it.Category == CateGoryConst.RESOURCE_MENU
                && it.MenuType == SysResourceConst.MENU &&
                it.Code != SysResourceConst.SYSTEM);
            if (oldMenu != null)//如果存在就直接和删除（同时删除其下面的菜单、按钮，清除对应的角色与资源信息)
                await _menuService.Delete(new BaseIdListInput()
                {
                    Ids = new List<long>()
                    {
                        oldMenu.Id
                    }
                });
            //添加菜单参数
            var menu = new MenuAddInput
            {
                Id = CommonUtils.GetSingleId(),
                ParentId = genBasic.MenuPid,
                Title = title,
                Category = CateGoryConst.RESOURCE_MENU,
                Module = genBasic.Module,
                Icon = genBasic.Icon,
                Name = genBasic.BusName,
                Code = RandomHelper.CreateRandomString(10),
                Path = $"{genBasic.RouteName}/{genBasic.BusName}",
                Component = $"{genBasic.RouteName}/{genBasic.BusName}/index".TrimStart('/'),
                SortCode = 99,
                Status = CommonStatusConst.ENABLE,
                IsKeepAlive = true
            };
            await _menuService.Add(menu);//添加菜单

            #endregion 菜单

            #region 按钮

            //添加按钮参数
            var button = new ButtonAddInput
            {
                Title = genBasic.FunctionName,
                ParentId = menu.Id,
                Code = StringHelper.FirstCharToLower(genBasic.ClassName),
                Status = CommonStatusConst.ENABLE
            };
            var buttonIds = await _buttonService.AddBatch(button);//添加按钮

            #endregion 按钮

            #region 角色授权

            var roleRep = ChangeRepository<DbRepository<SysRole>>();//切换仓储
            var superAdmin = await roleRep.GetFirstAsync(it =>
                it.Code == SysRoleConst.SUPER_ADMIN && it.Category == CateGoryConst.ROLE_GLOBAL);//获取超级管理员
            //授权菜单参数
            var grantResource = new GrantResourceInput
            {
                Id = superAdmin.Id,
                GrantInfoList = new List<RelationRoleResource>
                    { new RelationRoleResource { MenuId = menu.Id, ButtonInfo = buttonIds } },
                IsCodeGen = true
            };
            await _roleService.GrantResource(grantResource);//授权菜单

            #endregion 角色授权

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"代码生成创建菜单和授权报错{ex.Message}", ex.InnerException);
            return false;
        }
    }

    #endregion 方法
}
