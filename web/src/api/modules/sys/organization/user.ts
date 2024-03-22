import { CommonStatusEnum } from "@/enums";
/**
 * @description 用户管理
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

import { ReqId, ResPage, SysRole, SysUser } from "@/api";
import { moduleRequest } from "@/api/request";
const http = moduleRequest("/sys/organization/user/");

/**
 * @Description: 用户管理
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
const sysUserApi = {
  /** 用户分页 */
  page(params: SysUser.Page) {
    return http.get<ResPage<SysUser.SysUserInfo>>("page", params);
  },
  /** 用户选择器 */
  selector(params: any) {
    return http.get<ResPage<SysUser.SysUserInfo>>("selector", params);
  },
  /** 获取用户详情 */
  detail(params: ReqId) {
    return http.get<SysUser.SysUserInfo>("detail", params);
  },
  /**  提交表单 edit为true时为编辑，默认为新增 */
  submitForm(params: {}, edit: boolean = false) {
    return http.post(edit ? "edit" : "add", params);
  },
  /** 删除用户 */
  delete(params: ReqId[]) {
    return http.post("delete", params);
  },
  /** 修改用户状态 */
  updateStatus(params: ReqId, operation: CommonStatusEnum) {
    return http.post(operation === CommonStatusEnum.DISABLE ? "disableUser" : "enableUser", params);
  },
  /** 重置用户密码 */
  resetPassword(params: ReqId) {
    return http.post("resetPassword", params);
  },
  /** 获取用户拥有角色 */
  ownRole(params: ReqId) {
    return http.get<SysRole.SysRoleInfo[]>("ownRole", params);
  },
  /** 给用户授权角色 */
  grantRole(params: SysUser.GrantRoleReq) {
    return http.post("grantRole", params);
  },
  // 获取权限授权树
  permissionTreeSelector(params: ReqId) {
    return http.get<string[]>("permissionTreeSelector", params, { loading: false });
  },
  // 获取用户拥有资源
  ownResource(params: ReqId) {
    return http.get<SysRole.RoleOwnResource>("ownResource", params, { loading: false });
  },
  /** 给用户授权资源 */
  grantResource(params: SysRole.GrantResourceReq) {
    return http.post("grantResource", params);
  },
  /** 获取用户拥有权限 */
  ownPermission(params: ReqId) {
    return http.get<SysRole.RoleOwnPermission>("ownPermission", params, { loading: false });
  },
  /** 获取用户拥有权限 */
  grantPermission(params: SysRole.GrantPermissionReq) {
    return http.post("grantPermission", params);
  }
};

export { sysUserApi };
