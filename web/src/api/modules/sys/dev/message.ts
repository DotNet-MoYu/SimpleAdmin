/**
 * @description 站内消息
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

import { moduleRequest } from "@/api/request";
import { ReqId, ResPage, SysMessage } from "@/api";
const http = moduleRequest("/sys/dev/message/");
const messageApi = {
  /** 获取消息分页 */
  page(params: any) {
    return http.get<ResPage<SysMessage.SysMessageInfo>>("page", params);
  },
  /** 获取消息详情 */
  detail(params = {}) {
    return http.get<SysMessage.SysMessageInfo>("detail", params);
  },
  /** 获取已读未读详情 */
  readDetail(params = {}) {
    return http.get<SysMessage.receiverDetail[]>("readDetail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  submitForm(params = {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除单页 */
  delete(params: ReqId[]) {
    return http.post("delete", params);
  }
};

export { messageApi };
