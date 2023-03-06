using Furion.DependencyInjection;
using SimpleAdmin.Core;

namespace SimpleAdmin.Application;

/// <summary>
/// 测试服务
/// </summary>
public interface IGenTestService : ITransient
{

    /// <summary>
    /// 测试分页查询
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页结果</returns>
    Task<SqlSugarPagedList<GenTest>> Page(GenTestPageInput input);

    /// <summary>
    /// 添加测试
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(GenTestAddInput input);

    /// <summary>
    /// 删除测试
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(List<BaseIdInput> input);

    /// <summary>
    /// 修改测试
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(GenTestEditInput input);

    /// <summary>
    /// 测试详情
    /// </summary>
    /// <param name="input">Id参数</param>
    /// <returns>详细信息</returns>
    Task<GenTest> Detail(BaseIdInput input);
    Task<dynamic> Preview(ImportPreviewInput input);
    Task<dynamic> Export(GenTestPageInput input);
    Task<FileStreamResult> Template();

    Task<ImportResultOutPut<GenTestImportInput>> Import(ImportResultInput<GenTestImportInput> input);
}