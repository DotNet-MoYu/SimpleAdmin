/**
 * @description 操作日志接口
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

import { ReqPage, VisLog } from "@/api";
/**
 * @Description: 操作日志接口
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
export namespace OpLog {
  /** 操作日志分页查询 */
  export interface Page extends ReqPage {
    /** 操作日志分类 */
    category: string;
  }

  /** 操作日志信息 */
  export interface OpLogInfo extends VisLog.VisLogInfo {
    /** 具体消息 */
    exeMessage: null;
    /** 类名称 */
    className: string;
    /** 方法名称 */
    methodName: string;
    /** 请求方式 */
    reqMethod: string;
    /** 请求地址 */
    reqUrl: string;
    /** 请求参数 */
    paramJson: string | null;
    /** 返回结果 */
    resultJson: string | null;
  }

  /** 折线图 */
  export interface ColumnChart {
    /** 日期 */
    date: string;
    /** 日志类型 */
    name: string;
    /** 数量 */
    count: number;
  }

  /** 饼图 */
  export interface PineChart {
    /** 类型 */
    type: string;
    /** 数量 */
    value: number;
  }
}
