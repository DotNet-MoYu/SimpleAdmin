/**
 * @description 用户个人中心
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
import { moduleRequest } from "@/api/request";
import { ReqId, UserCenter, Login, Menu, SysMessage } from "@/api";
const http = moduleRequest("/userCenter/");

/**
 * @Description: 用户个人中心
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54

 */
const userCenterApi = {
  /** 获取用户菜单 */
  getAuthMenuList(params: ReqId) {
    return http.get<Menu.MenuInfo[]>("loginMenu", params, { loading: false });
  },
  /** 设置默认模块 */
  setDefaultModule(params: UserCenter.ResModuleDefault) {
    http.post("setDefaultModule", params, { loading: false });
  },
  /** 修改用户密码 */
  updatePassword(params: UserCenter.ReqUpdatePassword) {
    return http.post("updatePassword", params);
  },
  /** 更新签名 */
  updateSignature(params: { signature: string }) {
    return http.post<string>("updateSignature", params, { loading: false });
  },
  /** 更新头像 */
  updateAvatar(params: any) {
    return http.post<string>("updateAvatar", params, { loading: false });
  },
  /** 更新用户信息 */
  updateUserInfo(params: Login.LoginUserInfo) {
    return http.post<string>("updateUserInfo", params, { loading: false });
  },
  /** 更新快捷方式 */
  updateUserWorkbench(params: UserCenter.ResUpdateUserWorkbench) {
    return http.post<string>("updateUserWorkbench", params, { loading: false });
  },
  /** 获取用户快捷方式 */
  loginWorkbench() {
    return http.get<any>("loginWorkbench", {}, { loading: false });
  },
  /** 获取快捷方式菜单树 */
  shortcutTree() {
    return http.get<Menu.MenuInfo[]>("shortcutTree", {}, { loading: false });
  },
  /** 获取未读shu*/
  unReadCount() {
    return http.get<UserCenter.ResUnReadCount[]>("unReadCount", {}, { loading: false });
  },
  /** 获取最新未读*/
  newUnRead() {
    return http.get<SysMessage.SysMessageInfo[]>("newUnRead", {}, { loading: false });
  },
  /** 我的消息分页*/
  myMessagePage(params: UserCenter.ReqMyMessagePage) {
    return http.get<SysMessage.SysMessageInfo>("myMessagePage", params, { loading: false });
  },
  /** 消息详情*/
  myMessageDetail(params: ReqId) {
    return http.get<UserCenter.ResSysMessageDetail>("myMessageDetail", params);
  },
  /** 标记已读*/
  setRead(params: { ids: string[] | number[] }) {
    return http.post("setRead", params);
  },
  /** 全部已读*/
  allRead(params: { category: string }) {
    return http.post("setRead", params);
  },
  /** 全部删除*/
  allDelete(params: { category: string }) {
    return http.post("setDelete", params);
  },
  /** 标记删除*/
  setDelete(params: ReqId[]) {
    return http.post("setDelete", params);
  }
};

export { userCenterApi };
