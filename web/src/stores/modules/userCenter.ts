/**
 * @description 用户中心
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

const name = "simple-userCenter"; // 定义模块名称

const accountBasic = "accountBasic";
const file = "file";
const message = "message";
const short = "short";
/* MqttState */
export interface UserCenterState {
  /** 当前标签页 */
  tab: string;
  accountBasic: string;
  file: string;
  message: string;
  short: string;
}

/** Mqtt模块 */
export const useCenterStore = defineStore({
  id: name,
  state: (): UserCenterState => ({
    tab: accountBasic,
    accountBasic: accountBasic,
    file: file,
    message: message,
    short: short
  }),
  getters: {
    getTab: state => state.tab,
    getAccountBasic: state => state.accountBasic,
    getFile: state => state.file,
    getMessage: state => state.message,
    getShort: state => state.short
  },
  actions: {
    setTab(tab: string) {
      this.tab = tab;
    },
    resetTab() {
      this.tab = accountBasic;
    },
    setMessage() {
      this.tab = "message";
    }
  }
});
