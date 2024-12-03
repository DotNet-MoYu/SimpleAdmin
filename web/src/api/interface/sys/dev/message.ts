/**
 * @description 站内消息接口
 * @license Apache License Version 2.0
2022
,
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

import { ReqPage } from "@/api/interface";
import { MessageTypeDictEnum, MessageSendWayDictEnum } from "@/enums";
import { SysUser, SysRole } from "@/api";
/**
 * @Description: 站内消息接口
 * @Author: huguodong
 * @Date: 2024-11-08 15:54:07
 */
export namespace SysMessage {
  /** 消息类型 */
  export type MessageType = MessageTypeDictEnum.INFORM | MessageTypeDictEnum.NOTICE | MessageTypeDictEnum.MESSAGE;

  /** 消息信息 */
  export interface SysMessageInfo {
    /** 消息ID */
    id: number | string;
    /** 消息类型 */
    category: string;
    /** 消息标题 */
    subject: string;
    /** 消息内容 */
    content: string;
    /** 消息接收者 */
    receiverType: string;
    /** 是否已读 */
    read: boolean;
    /** 创建时间 */
    createTime: string;
    /** 接受者Id列表 */
    receiverInfo: SysUser.SysUserInfo[] | SysRole.SysRoleInfo[];
    /**发送方式 */
    sendWay: MessageSendWayDictEnum;
    /**发送时间 */
    sendTime: string;
    /**发送时间格式化 */
    sendTimeFormat: string;
    /**延迟时间 */
    delayTime: number;
    /**发送状态 */
    status: string;
    /**接收详情 */
    receiverDetail: receiverDetail[]; //接收者详情
  }

  /** 消息信息 */
  export interface receiverDetail {
    /** 消息ID */
    id: number | string;
    /** 名称 */
    name: string;
    /** 是否已读 */
    read: boolean;
  }
  /** 消息分页查询 */
  export interface Page extends ReqPage {}
}
