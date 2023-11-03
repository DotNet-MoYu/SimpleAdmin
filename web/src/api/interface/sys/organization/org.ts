import { SysUser } from "@/api";
/**
 * @description 组织管理
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

import { ReqPage } from "@/api";
export namespace SysOrg {
  /** 组织分页查询 */
  export interface Page extends ReqPage {}

  /** 组织树查询 */
  export interface ReqTree {
    parentId?: number | string;
  }

  /** 组织信息 */
  export interface SysOrgInfo {
    /** 组织id */
    id: number | string;
    /** 父ID */
    parentId: number | string;
    /** 主管Id */
    directorId: number | string | null;
    /** 组织名称 */
    name: string;
    /** 组织全称 */
    names: string;
    /** 组织编码 */
    code: string;
    /** 组织分类 */
    category: string;
    /** 状态 */
    status: string;
    /** 排序码 */
    sortCode: number;
    /** 主管信息 */
    directorInfo: SysUser.SysUserInfo | null;
  }
  /** 组织树 */
  export interface SysOrgTree {
    /** 组织id */
    id: number | string;
    /** 父ID */
    parentId: number | string;
    /** 组织名称 */
    name: string;
    /** 子集 */
    children: SysOrgTree[];
  }
}
