/**
 * @description  菜单管理接口
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
import { Spa } from "@/api/interface";
import { MenuTypeDictEnum } from "@/enums";
/**
 * @Description: 菜单管理接口
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
export namespace Menu {
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
  export interface MenuInfo extends Spa.SpaInfo {
    /** 所属模块 */
    module: number | string;
    /** 上级菜单 */
    parentId: number | string;
    /** 分类 */
    category: string;
    /** 子菜单 */
    children?: MenuInfo[];
  }

  /** 菜单树信息 */
  export interface MenuTreeInfo {
    /** id */
    id: number | string;
    /** 菜单名称 */
    title: string;
    /** 上级菜单Id */
    parentId: number | string;
    /** 菜单路由 */
    path?: string;
    /** 是否禁用 */
    disabled?: boolean;
    /** 菜单类型 */
    menuType: MenuType;
    /** 子菜单 */
    children?: MenuTreeInfo[];
  }

  /** 菜单树选择器输入参数 */
  export interface MenuTreeSelectorReq {
    /** 所属模块 */
    module?: number | string;
  }
}
