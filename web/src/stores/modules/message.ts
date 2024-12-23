/**
 * @description 系统消息
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

import { defineStore } from "pinia";
import { userCenterApi, UserCenter, SysMessage } from "@/api";
import { ElNotification } from "element-plus";
import { Message } from "@element-plus/icons-vue";

const name = "simple-message"; // 定义模块名称

/* MqttState */
export interface MessageState {
  /** 未读消息信息 */
  unReadInfo: any;
  /** 未读消息数 */
  unReadCount: number;
  /** 新未读消息 */
  newUnRead: SysMessage.SysMessageInfo[];
}

/** Mqtt模块 */
export const useMessageStore = defineStore({
  id: name,
  state: (): MessageState => ({
    unReadInfo: {},
    unReadCount: 0,
    newUnRead: []
  }),
  getters: {
    unReadCountGet: state => state.unReadCount,
    unReadInfoGet: state => state.unReadInfo
  },
  actions: {
    /** 显示更多 */
    setShowMore(state: boolean) {
      this.showMore = state;
    },
    /** 增加未读消息数 */
    unReadCountAdd(value: number) {
      this.unReadCount += value;
    },
    /** 减少未读消息数 */
    unReadCountSubtract(value: number) {
      this.unReadCount -= value;
    },
    /** 设置未读消息数 */
    unReadCountSet(value: number) {
      this.unReadCount = value;
    },
    /** 获取未读消息数 */
    async getUnReadInfo(notice: boolean = false) {
      await userCenterApi.unReadCount().then(res => {
        if (res.data.length > 0) {
          //未读消息信息数量转换
          this.unReadInfo = res.data.reduce((acc, item) => {
            acc[item.category] = item.unReadCount;
            return acc;
          }, {});
          //遍历未读消息信息，获取未读消息总数
          let count = 0;
          res.data.map((item: UserCenter.ResUnReadCount) => {
            count += item.unReadCount;
          });
          //如果未读消息总数大于当前未读消息总数，则获取最新未读消息
          if (count > this.unReadCount) {
            this.getNewMessage(notice);
          }
          this.unReadCountSet(count);
        } else {
          this.unReadCountSet(0);
        }
      });
    },
    /** 获取最新未读消息 */
    async getNewUnRead() {
      await userCenterApi.newUnRead().then(res => {
        if (res.data.length > 0) {
          this.newUnRead = res.data;
        }
      });
    },
    /** 获取未读消息数 */
    getUnReadCount(category: string) {
      return this.unReadInfo[category] || 0;
    },
    /**提示有新消息 */
    getNewMessage(notice: boolean = false, message: string = "您有一条新消息,请注意查收!") {
      this.getNewUnRead();
      if (notice) {
        ElNotification({
          title: "收到一条新消息",
          message: message,
          icon: Message,
          offset: 40
        });
      }
    },
    /**定时刷新最新消息*/
    async getNewMessageInterval() {
      setInterval(() => {
        this.getUnReadInfo(true);
      }, 10000);
    },
    /* 重置未读消息数 */
    reSet() {
      this.unReadCount = 0;
      this.unReadInfo = {};
      this.newUnRead = [];
    }
  }
});
