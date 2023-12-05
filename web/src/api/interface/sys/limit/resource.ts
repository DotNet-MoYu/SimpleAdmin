/**
 * @description 资源
 * @license Apache License Version 2.0
 * @remarks
 * SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
 * 1.请不要删除和修改根目录下的LICENSE文件。
 * 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
 * 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
 * 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
 * 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
 * 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关
 * @see https://gitee.com/zxzyjs/SimpleAdmin
 */

export namespace SysResource {
  /** 角色授权资源树 */
  export interface ResTreeSelector {
    /** 模块id */
    id: number | string;
    /** 模块名称 */
    title: string;
    /** 图标 */
    icon: string;
    /** 模块下菜单集合 */
    menu: RoleGrantResourceMenu[];
  }

  /** 角色授权资源菜单信息 */
  export interface RoleGrantResourceMenu {
    /** 菜单id */
    id: number | string;
    /** 父id */
    parentId: number | string;
    /** 父名称 */
    parentName: string;
    /** 模块名称 */
    title: string;
    /** 模块id */
    module: number;
    /** 菜单下按钮集合 */
    button: RoleGrantResourceButton[];
    /** 父级是否选中 */
    parentCheck: boolean;
    /** 是否选中 */
    nameCheck: boolean;
  }

  /** 角色授权资源按钮信息 */
  export interface RoleGrantResourceButton {
    /** 按钮id */
    id: number | string;
    /** 名称 */
    title: string;
    /** 是否被选中 */
    check: boolean;
  }
}
