/**
 * @description 机构管理
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

import { ReqId, ResPage, SysOrg } from "@/api";
import { moduleRequest } from "@/api/request";
const http = moduleRequest("/biz/organization/org/");

/**
 * @Description: 机构管理
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
const bizOrgApi = {
  /** 获取机构分页 */
  page(params: SysOrg.Page) {
    return http.get<ResPage<SysOrg.SysOrgInfo>>("page", params);
  },
  /** 获取机构树 */
  tree() {
    return http.get<SysOrg.SysOrgTree[]>("tree");
  },
  /** 获取机构详情 */
  detail(params: ReqId) {
    return http.get<SysOrg.SysOrgInfo>("detail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  submitForm(params = {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除机构 */
  delete(params: ReqId[]) {
    return http.post("delete", params);
  },
  /** 复制机构 */
  copy(params = {}) {
    return http.post("copy", params);
  }
};

/**
 * @Description: 机构管理按钮权限码
 * @Author: huguodong
 * @Date: 2024-02-20 09:51:15
 */
const bizOrgButtonCode = {
  /** 新增机构 */
  add: "bizOrgAdd",
  /** 编辑机构 */
  edit: "bizOrgEdit",
  /** 删除机构 */
  delete: "bizOrgDelete",
  /** 批量删除机构 */
  batchDelete: "bizOrgBatchDelete",
  /** 复制机构 */
  copy: "bizOrgCopy"
};

export { bizOrgApi, bizOrgButtonCode };
