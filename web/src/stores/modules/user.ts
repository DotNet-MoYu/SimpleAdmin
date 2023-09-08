/**
 * @description 用户模块
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
import { defineStore } from "pinia";
import { Login } from "@/api/interface";
import piniaPersistConfig from "@/config/piniaPersist";
import { getLoginUserApi } from "@/api";
import { ElNotification } from "element-plus";
/* UserState */
export interface UserState {
  /** token */
  accessToken: string;
  /** 刷新token */
  refreshToken: string;
  /** 用户信息 */
  userInfo: Login.LoginUserInfo | null;
  /** 默认模块 */
  defaultModule: number | string | null;
  /** 选择模块 */
  chooseModule: number | string | null;
  /** 模块列表 */
  moduleList: Login.ModuleInfo[];
}

export const useUserStore = defineStore({
  id: "simple-user",
  state: (): UserState => ({
    accessToken: "",
    refreshToken: "",
    userInfo: null,
    defaultModule: null,
    chooseModule: null,
    moduleList: []
  }),
  getters: {
    userInfoGet: state => state.userInfo,
    chooseModuleGet: state => state.chooseModule
  },
  actions: {
    // Set Token
    setToken(token: string, refreshToken: string) {
      this.accessToken = token;
      this.refreshToken = refreshToken;
    },
    async getUserInfo() {
      /**  获取用户信息 */
      const { data } = await getLoginUserApi();
      if (data) {
        this.setUserInfo(data);
      } else {
        ElNotification({
          title: "系统错误",
          message: "获取个人信息失败，请联系系统管理员！",
          type: "warning",
          duration: 3000
        });
      }
      return this.userInfo;
    },
    /** 设置用户信息 */
    setUserInfo(userInfo: Login.LoginUserInfo) {
      this.userInfo = userInfo;
      this.defaultModule = userInfo.defaultModule;
      this.moduleList = userInfo.moduleList;
    },
    /** 清除token */
    clearToken() {
      this.accessToken = "";
      this.refreshToken = "";
    },
    /** 清理用户信息 */
    clearUserStore() {
      this.clearToken();
      this.userInfo = null;
      this.defaultModule = null;
      this.moduleList = [];
    },
    /** 选择模块 */
    setModule(moduleId: number | string | null) {
      this.chooseModule = moduleId;
    }
  },
  persist: piniaPersistConfig("simple-user")
});
