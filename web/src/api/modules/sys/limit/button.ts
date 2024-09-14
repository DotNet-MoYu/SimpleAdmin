/**
 * @description 权限按钮
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
import { ReqId, ResPage, Button } from "@/api/interface";
const http = moduleRequest("/sys/limit/button/");

/**
 * @Description: 权限按钮
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
const buttonApi = {
  /** 获取单页分页 */
  page(params: Button.Page) {
    return http.get<ResPage<Button.ButtonInfo>>("page", params);
  },
  /** 获取单页详情 */
  detail(params: ReqId) {
    return http.get<Button.ButtonInfo>("detail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  submitForm(params = {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除单页 */
  delete(params: ReqId[]) {
    return http.post("delete", params);
  },
  /** 删除单页 */
  batch(params: Button.Batch) {
    return http.post("batch", params);
  }
};

export { buttonApi };
