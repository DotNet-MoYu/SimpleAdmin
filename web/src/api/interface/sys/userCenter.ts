/**
 * @description  用户个人中心接口
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
import { ReqId } from "@/api/interface";
/**
 * @Description: 用户个人中心接口
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54

 */
export namespace UserCenter {
  /** 设置默认模块 */
  export interface ResModuleDefault extends ReqId {
    /** 是否默认 */
    ifDefault: boolean;
  }

  /** 修改用户密码 */
  export interface ReqUpdatePassword {
    /** 旧密码 */
    password: string;
    /** 新密码 */
    newPassword: string;
  }

  /** 更新快捷方式 */
  export interface ResUpdateUserWorkbench {
    /** 快捷方式 */
    WorkbenchData: string;
  }

  /** 未读消息数量 */
  export interface ResUnReadCount {
    /** 分类 */
    category: string;
    /** 数量 */
    unReadCount: number;
  }
  /** 消息详情 */
  export interface ResSysMessageDetail {
    /** 消息id */
    id: number | string;
    /** 消息类型 */
    category: string;
    /** 消息标题 */
    subject: string;
    /** 消息内容 */
    content: string;
    /** 是否已读 */
    read: boolean;
    /**发送时间 */
    sendTime: string;
    /**发送时间格式化 */
    sendTimeFormat: string;
  }

  export interface ReqMyMessagePage {
    category?: string; //消息分类
    searchKey?: string; //关键字
  }
}
