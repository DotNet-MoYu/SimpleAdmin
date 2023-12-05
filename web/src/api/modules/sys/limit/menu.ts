/**
 * @description 菜单管理
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

import { moduleRequest } from "@/api/request";
import { ReqId, Spa, Menu } from "@/api/interface";
const http = moduleRequest("/sys/limit/menu/");

export default {
  /** 获取菜单树 */
  menuTree(params: Menu.Tree) {
    return http.get<Menu.MenuInfo[]>("tree", params);
  },
  /** 获取菜单详情 */
  menuDetail(params: ReqId) {
    return http.get<Spa.SpaInfo>("detail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  menuSubmitForm(params: {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除菜单 */
  menuDelete(params: ReqId[]) {
    return http.post("delete", params);
  },
  /** 修改模块菜单 */
  menuChangeModule(params: { id: number | string; module: number | string }) {
    return http.post("changeModule", params);
  }
};
