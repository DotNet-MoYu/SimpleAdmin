// SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
// 1.请不要删除和修改根目录下的LICENSE文件。
// 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
// 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
// 4.基于本软件的作品。，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
// 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
// 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关。

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

    #endregion 查询

    #region 新增

    /// <summary>
    /// 添加测试
    /// </summary>
    /// <param name="input">添加参数</param>
    /// <returns></returns>
    Task Add(GenTestAddInput input);

    #endregion 新增

    #region 编辑

    /// <summary>
    /// 修改测试
    /// </summary>
    /// <param name="input">编辑参数</param>
    /// <returns></returns>
    Task Edit(GenTestEditInput input);

    #endregion 编辑

    #region 删除

    /// <summary>
    /// 删除测试
    /// </summary>
    /// <param name="input">删除参数</param>
    /// <returns></returns>
    Task Delete(BaseIdListInput input);

    #endregion 删除

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
    Task<ImportResultOutPut<GenTestImportInput>>
        Import(ImportResultInput<GenTestImportInput> input);

    /// <summary>
    /// 测试导出
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<FileStreamResult> Export(GenTestPageInput input);

    #endregion 导入导出
}