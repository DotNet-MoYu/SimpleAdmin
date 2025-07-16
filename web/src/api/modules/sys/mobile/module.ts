/**
 * @description 模块管理Api
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

import { ReqId, ResPage, MobileModule, moduleRequest } from "@/api";
const http = moduleRequest("/sys/mobile/module/");

/**
 * @Description: 模块管理
 * @Author: superAdmin
 * @Date: 2025-06-25 14:37:52
 */
const mobileModuleApi = {
  /** 获取模块分页 */
  page(params: MobileModule.Page) {
    return http.get<ResPage<MobileModule.MobileModuleInfo>>("page", params);
  },
  /** 获取模块详情 */
  detail(params: ReqId) {
    return http.get<MobileModule.MobileModuleInfo>("detail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  submitForm(params = {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除模块 */
  delete(params: ReqId[]) {
    return http.post("delete", params);
  }
};

export { mobileModuleApi };
