/**
 * @description 用户管理
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
export namespace SysUser {
  /** 用户分页查询 */
  export interface Page extends ReqPage {
    /** 用户状态 */
    status?: string;
  }

  /** 用户信息 */
  export interface SysUserInfo {
    id: number | string;
    /** 头像 */
    avatar?: string;
    /** 签名 */
    signature?: string;
    /** 账号 */
    account: string;
    /** 姓名 */
    name?: string;
    /** 昵称 */
    nickname?: string;
    /** 性别 */
    gender?: string;
    /** 出生日期 */
    birthday?: string;
    /** 民族 */
    nation?: string;
    /** 籍贯 */
    nativePlace?: string;
    /** 家庭住址 */
    homeAddress?: string;
    /** 通信地址 */
    mailingAddress?: string;
    /** 证件类型 */
    idCardType?: string;
    /** 证件号码 */
    idCardNumber?: string;
    /** 文化程度 */
    cultureLevel?: string;
    /** 政治面貌 */
    politicalOutlook?: string;
    /** 毕业院校 */
    college?: string;
    /** 学历 */
    education?: string;
    /** 学制 */
    eduLength?: string;
    /** 学位 */
    degree?: string;
    /** 手机 */
    phone?: string;
    /** 邮箱 */
    email?: string;
    /** 家庭电话 */
    homeTel?: string;
    /** 办公电话 */
    officeTel?: string;
    /** 紧急联系人 */
    emergencyContact?: string;
    /** 紧急联系人电话 */
    emergencyPhone?: string;
    /** 紧急联系人地址 */
    emergencyAddress?: string;
    /** 员工编号 */
    empNo?: string;
    /** 入职日期 */
    entryDate?: string;
    /** 机构id */
    orgId: number;
    /** 职位id */
    positionId: number;
    /** 职级 */
    positionLevel?: string;
    /** 主管id */
    directorId?: number | string | null;
    /** 主管id */
    orgAndPosIdList: number[] | string[];
    /** 主管id */
    directorInfo?: SysUser.SysUserInfo | null;
    /** 用户状态 */
    status?: string;
    /** 排序码 */
    sortCode?: number;
    /** 默认模块 */
    defaultModule?: number | string;
    /** 机构信息 */
    orgName?: string;
    /** 机构信息全称 */
    orgNames?: string;
  }

  /** 用户分页查询 */
  export interface UserSelector extends ReqPage {
    /** 用户账号 */
    account?: string;
    orgId: string | number;
  }
}
