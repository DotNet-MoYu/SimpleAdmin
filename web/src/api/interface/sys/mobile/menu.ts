/**
 * @description 菜单管理接口
 * @license Apache License Version 2.0
 * @Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
 * @remarks
 * SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
 * 1.请不要删除和修改根目录下的LICENSE文件。
 * 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
 * 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
 * 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
 * 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
 * 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关
 */

import { MobileModule } from "@/api/interface";
import { MenuTypeDictEnum } from "@/enums";
import { ReqId } from "@/api/interface";

/**
 * @Description: 菜单管理接口
 * @Author: superAdmin
 * @Date: 2025-06-26 16:41:59
 */
export namespace MobileMenu {
  /** 菜单类型 */
  export type MenuType = MenuTypeDictEnum.MENU | MenuTypeDictEnum.CATALOG | MenuTypeDictEnum.LINK;

  /**菜单分页查询 */
  export interface Tree {
    /** 菜单名称 */
    menuType?: MenuType;
    /** 所属模块名称 */
    module: number | string;
  }

  /** 菜单信息 */
  export interface MobileMenuInfo extends MobileModule.MobileModuleInfo {}

  /** 菜单树选择器输入参数 */
  export interface MenuTreeSelectorReq {
    /** 所属模块 */
    module?: number | string;
  }

  /** 移动端资源授权树 */
  export interface MobileResTreeSelector {
    /** 模块id */
    id: number | string;
    /** 模块名称 */
    title: string;
    /** 图标 */
    icon: string;
    /** 模块下菜单集合 */
    menu: RoleGrantMobileResourceMenu[];
  }

  /** 角色授权移动端资源菜单信息 */
  export interface RoleGrantMobileResourceMenu {
    /** 菜单id */
    id: number | string;
    /** 父id */
    parentId: number | string;
    /** 父名称 */
    parentName: string;
    /** 菜单名称 */
    title: string;
    /** 模块id */
    module: number;
    /**菜单下按钮集合 */
    button: RoleGrantResourceButton[];
    /** 父级是否选中 */
    parentCheck: boolean;
    /** 是否选中 */
    nameCheck: boolean;
  }

  export interface RoleGrantResourceButton {
    /** 按钮id */
    id: number | string;
    /** 标题 */
    title: string;
    /** 是否被选中 */
    check: boolean;
  }
  /** 角色拥有移动端资源 */
  export interface RoleOwnMobileResource {
    /** id */
    id: number | string;
    /** 已授权移动端资源信息 */
    grantInfoList: RelationRoleMobileResource[];
  }

  /** 角色移动端资源关系 */
  export interface RelationRoleMobileResource {
    /** 菜单id */
    menuId: number | string;
    /** 按钮id */
    buttonInfo: (string | number)[];
  }

  /** 角色授权移动端资源请求参数 */
  export interface GrantMobileResourceReq extends ReqId {
    /** 授权移动端资源信息 */
    GrantInfoList: RelationRoleMobileResource[];
  }
}
