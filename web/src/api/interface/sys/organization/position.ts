/**
 * @description  职位管理接口
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

import { ReqPage } from "@/api";
/**
 * @Description: 职位管理接口
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
export namespace SysPosition {
  /** 职位分页查询 */
  export interface Page extends ReqPage {}

  /** 职位信息 */
  export interface SysPositionInfo {
    /** 职位id */
    id: number | string;
    /** 职位ID */
    orgId: number | string;
    /** 职位名称 */
    name: string;
    /** 职位编码 */
    code: string;
    /** 职位分类 */
    category: string;
    /** 状态 */
    status: string;
    /** 排序码 */
    sortCode: number;
  }
  /** 职位树 */
  export interface SysPositionTree {
    /** id */
    id: number | string;
    /** 名称 */
    name: string;
    /** 是否是职位 */
    isPosition: boolean;
    /** 子集 */
    children: SysPositionTree[];
  }

  /** 职位选择器 */
  export interface SysPositionSelector {
    /** id */
    id: number | string;
    /** 名称 */
    name: string;
    /** 组织Id */
    orgId: number | string;
    /** 子集 */
    children: SysPositionSelector[];
  }
}
