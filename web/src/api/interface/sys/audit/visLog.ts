/**
 * @description 访问日志接口
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
 * @Description: 操作日志接口
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
export namespace VisLog {
  /** 访问日志分页查询 */
  export interface Page extends ReqPage {
    /** 访问日志分类 */
    category: string;
  }

  /** 访问日志详情 */
  export interface VisLogInfo {
    /** id */
    id: number;
    /** 访问日志分类 */
    category: string;
    /** 访问日志标题 */
    name: string;
    /** 执行状态 */
    exeStatus: string;
    /** 操作Ip */
    opIp: string;
    /** 操作地址 */
    opAddress: string;
    /** 操作浏览器 */
    opBrowser: string;
    /** 操作系统 */
    opOs: string;
    /** 操作时间 */
    opTime: string;
    /** 操作人 */
    opUser: string;
    /** 操作账号 */
    opAccount: string;
    /** 操作时间 */
    createTime: string;
  }

  /** 折线图 */
  export interface LineChart {
    /** 日期 */
    date: string;
    /** 登入量 */
    loginCount: number;
    /** 登出量 */
    logoutCount: number;
  }

  /** 饼图 */
  export interface PineChart {
    /** 类型 */
    type: string;
    /** 数量 */
    value: number;
  }
}
