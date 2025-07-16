/**
 * @description 字典请求接口
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

import type { ReqPage } from '@/api'

export interface DictTree {
  /** 字典值 */
  dictValue: string
  /** 字典标签 */
  dictLabel: string
  /** 状态 */
  status: string
  /** 子集 */
  children: DictTree[]
}

/** 字典分页查询 */
export interface Page extends ReqPage {
  /** 所属模块名称 */
  parentId: number | string
  /** 字典分类 */
  category: string
}

/** 字典信息 */
export interface DictInfo extends DictTree {
  /** 字典id */
  id: number | string
  /** 父Id */
  parentId: number | string
  /** 字典分类 */
  category: string
  /** 字典排序 */
  sortCode: number
}
