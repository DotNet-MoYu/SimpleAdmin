/**
 * @description 用户选择器接口
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
import UserSelector from "./index.vue";

/** 用户选择器属性 */
export interface UserSelectProps {
  /** 组织树api */
  orgTreeApi: (data?: any) => Promise<any>;
  /** 职位选择api */
  positionTreeApi?: (data?: any) => Promise<any>;
  /** 角色选择api */
  roleTreeApi?: (data?: any) => Promise<any>;
  /** 用户选择api */
  userSelectorApi: (data?: any) => Promise<any>;
  /** 是否多选 */
  multiple?: boolean;
  /** 最大用户数 */
  maxCount?: number;
  /** 是否是业务 */
  biz?: boolean;
}

/** 用户选择器表格初始化参数 */
export interface UserSelectTableInitParams {
  /** 组织ID */
  orgId?: number | string | null;
  /** 职位ID */
  positionId?: number | string | null;
  /** 角色ID */
  roleId?: number | string | null;
}

/**
 * @description 用户选择器实例类型
 */
export type UserSelectorInstance = Omit<InstanceType<typeof UserSelector>, keyof ComponentPublicInstance | keyof UserSelectProps>;
