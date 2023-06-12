using Furion.DependencyInjection;
using SimpleAdmin.Core;

namespace SimpleAdmin.Application;

/// <summary>
/// 测试服务
/// </summary>
public interface IGenTestService : ITransient
{
    #region 查询

    /// <summary>
    /// 测试分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>测试分页</returns>
    Task<SqlSugarPagedList<GenTest>> Page(GenTestPageInput input);	
  
    /// <summary>
    /// 测试列表查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>测试列表</returns>
    Task<List<GenTest>> List(GenTestPageInput input);	

    /// <summary>
    /// 测试详情
    /// </summary>
    /// <param name="input">Id参数</param>
    /// <returns>详细信息</returns>
    Task<GenTest> Detail(BaseIdInput input);


    #endregion
    
    #region 新增

    /// <summary>
    /// 添加测试
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(GenTestAddInput input);

    #endregion
    
    #region 编辑
    /// <summary>
    /// 修改测试
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(GenTestEditInput input);
    
    #endregion

    #region 删除

    /// <summary>
    /// 删除测试
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    #endregion

    #region 导入导出

    /// <summary>
    /// 测试导入模板下载
    /// </summary>
    /// <returns>模板</returns>
    Task<FileStreamResult> Template();
    
    /// <summary>
    /// 测试导入预览
    /// </summary>
    /// <param name="input">预览参数</param>
    /// <returns>预览结果</returns>
    Task<ImportPreviewOutput<GenTestImportInput>> Preview(ImportPreviewInput input);
    
    /// <summary>
    /// 测试导入
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<ImportResultOutPut<GenTestImportInput>> Import(ImportResultInput<GenTestImportInput> input);
    
    /// <summary>
    /// 测试导出
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<FileStreamResult> Export(GenTestPageInput input);

    #endregion
}

