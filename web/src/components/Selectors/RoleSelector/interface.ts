/**
 * @description 接口
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
 * @see https://gitee.com/dotnetmoyu/SimpleAdmin
 */

import RoleSelector from "./index.vue";

/** 角色选择器属性 */
export interface RoleSelectProps {
  /** 组织树api列表 */
  orgTreeApi: (data?: any) => Promise<any>;
  /** 角色选择api列表 */
  roleSelectorApi: (data?: any) => Promise<any>;
  /** 是否可带数据权限 */
  permission?: boolean;
  /** 是否多选 */
  multiple?: boolean;
  /** 最大角色数 */
  maxCount?: number;
}

/** 角色选择器表格初始化参数 */
export interface RoleSelectTableInitParams {
  /** 组织ID */
  orgId?: number | string | null;
}

/**
 * @description 角色选择器实例类型
 */
export type RoleSelectorInstance = Omit<InstanceType<typeof RoleSelector>, keyof ComponentPublicInstance | keyof RoleSelectProps>;
