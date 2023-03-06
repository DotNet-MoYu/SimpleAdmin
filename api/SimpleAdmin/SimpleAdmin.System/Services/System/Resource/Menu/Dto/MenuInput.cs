using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.System;

/// <summary>
/// 菜单树查询参数
/// </summary>

public class MenuTreeInput
{

    /// <summary>
    /// 模块
    /// </summary>
    public long? Module { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string SearchKey { get; set; }
}

/// <summary>
/// 添加菜单参数
/// </summary>

public class MenuAddInput : SysResource, IValidatableObject
{
    /// <summary>
    /// 父ID
    /// </summary>
    [Required(ErrorMessage = "ParentId不能为空")]
    public override long? ParentId { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "Title不能为空")]
    public override string Title { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public override string MenuType { get; set; } = ResourceConst.MENU;

    /// <summary>
    /// 模块
    /// </summary>
    [Required(ErrorMessage = "Module不能为空")]
    public override long? Module { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [Required(ErrorMessage = "Path不能为空")]
    public override string Path { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [Required(ErrorMessage = "Icon不能为空")]
    public override string Icon { get; set; }

    /// <summary>
    /// 特殊验证
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //如果菜单类型是菜单
        if (MenuType == ResourceConst.MENU)
        {
            if (string.IsNullOrEmpty(Name))
                yield return new ValidationResult("Name不能为空", new[] { nameof(Name) });
            if (string.IsNullOrEmpty(Component))
                yield return new ValidationResult("Component不能为空", new[] { nameof(Name) });
        }
        //如果是内链或者外链
        else if (MenuType == ResourceConst.IFRAME || MenuType == ResourceConst.LINK)
        {
            Component = null;//设置组件为空
            if (string.IsNullOrEmpty(Name))
                Name = RandomHelper.CreateRandomString(10);//name为随机字符串
        }
        else
        {
            Name = null;//设置name为空
            Component = null;//设置组件为空
        }
        //设置分类为菜单
        Category = CateGoryConst.Resource_MENU;
    }
}

/// <summary>
/// 编辑菜单输入参数
/// </summary>
public class MenuEditInput : MenuAddInput
{
    /// <summary>
    /// ID
    /// </summary>
    [IdNotNull(ErrorMessage = "Id不能为空")]
    public override long Id { get; set; }
}

/// <summary>
/// 改变模块输入参数
/// </summary>
public class MenuChangeModuleInput : BaseIdInput
{
    /// <summary>
    /// 模块ID
    /// </summary>
    [Required(ErrorMessage = "Module不能为空")]
    public long? Module { get; set; }

}