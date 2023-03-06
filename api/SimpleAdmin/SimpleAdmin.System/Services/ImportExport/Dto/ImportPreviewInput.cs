namespace SimpleAdmin.System;


/// <summary>
/// 导入预览输入
/// </summary>
public class ImportPreviewInput
{
    /// <summary>
    /// 文件
    /// </summary>
    [Required(ErrorMessage = "文件不能为空")]
    public IFormFile File { get; set; }

}
