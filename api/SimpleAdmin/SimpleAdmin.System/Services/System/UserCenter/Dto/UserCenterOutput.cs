namespace SimpleAdmin.System;

/// <summary>
/// 我的机构数样式
/// </summary>
public class LoginOrgTreeOutput
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 父ID
    /// </summary>
    public long Pid { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<LoginOrgTreeOutput> Children { get; set; }

    /// <summary>
    /// 样式
    /// </summary>
    public MyStyle Style { get; set; }

    /// <summary>
    /// 我的机构样式
    /// </summary>
    public class MyStyle
    {
        /// <summary>
        /// 背景色
        /// </summary>
        public string Background { get; set; } = "var(--primary-color)";

        /// <summary>
        ///  字体颜色
        /// </summary>
        public string Color { get; set; } = "#FFF";
    }
}