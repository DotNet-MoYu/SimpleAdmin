/**
 * @description  单页管理接口
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
import { ReqPage } from "@/api/interface";
import { MenuTypeDictEnum } from "@/enums";
export namespace Spa {
  /** 单页类型 */
  export type SpaType = MenuTypeDictEnum.MENU | MenuTypeDictEnum.LINK;

  /**单页分页查询 */
  export interface Page extends ReqPage {
    menuType: SpaType;
  }

  /** 单页信息 */
  export interface SpaInfo {
    /** id */
    id: number | string;
    /** 菜单名称 */
    title: string;
    /** 组件名称 */
    name: string;
    /** 菜单描述 */
    description: string;
    /** 菜单类型 */
    menuType: string;
    /** 菜单图标 */
    icon: string;
    /** 菜单路径 */
    path: string;
    /** 菜单组件 */
    component: string;
    /** 需要高亮的 path (通常用作详情页高亮父级菜单) */
    activeMenu: string;
    /** 是否首页 */
    isHome: boolean;
    /** 排序 */
    sortCode: number;
    /** 是否隐藏 */
    isHide: boolean;
    /** 是否缓存 */
    isKeepAlive: boolean;
    /** 是否全屏 */
    isAffix: boolean;
    /** 是否全屏 */
    isFull: boolean;
  }
}
