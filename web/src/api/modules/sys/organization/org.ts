/**
 * @description 组织管理
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

import { ReqId, ResPage, SysOrg } from "@/api";
import { moduleRequest } from "@/api/request";
const http = moduleRequest("/sys/organization/org/");

export default {
  /** 获取组织分页 */
  sysOrgPage(params: SysOrg.Page) {
    return http.get<ResPage<SysOrg.SysOrgInfo>>("page", params);
  },
  /** 获取组织树 */
  sysOrgTree() {
    return http.get<SysOrg.SysOrgTree[]>("tree");
  },
  /** 获取组织详情 */
  sysOrgDetail(params: ReqId) {
    return http.get<SysOrg.SysOrgInfo>("detail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  sysOrgSubmitForm(params: {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除组织 */
  sysOrgDelete(params: ReqId[]) {
    return http.post("delete", params);
  },
  /** 复制组织 */
  sysOrgCopy(params: {}) {
    return http.post("copy", params);
  }
};
