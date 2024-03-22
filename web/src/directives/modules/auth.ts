/**
 * @description 按钮权限指令
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
import { useAuthStore } from "@/stores/modules/auth";
import type { Directive, DirectiveBinding } from "vue";

/**
 * @description 按钮权限指令
 * 通过传入的权限码判断当前用户是否有权限，如果没有权限则移除该元素
 * @param value {string | string[]} - 权限码
 * @example
 * <button v-auth="'userAdd'">需要有用户添加用户权限</button>
 * <button v-auth="['userAdd','userEdit']">需要有添加用户权限编辑用户</button>
 * <button v-auth:and="['userAdd', 'userEdit']">添加和编辑用户权限都要有</button>
 * <button v-auth.and="['userAdd', 'userEdit']">添加和编辑用户权限都要有</button>
 */
const auth: Directive = {
  // 当指令挂载时调用
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    // 获取传入的值和参数
    const { value, arg } = binding;
    // 获取authStore
    const authStore = useAuthStore();
    // 获取用户权限按钮列表
    const currentPageRoles = authStore.authButtonListGet ?? [];
    // 如果传入的值是数组，并且数组的长度大于0
    if (value instanceof Array && value.length) {
      const fn = binding.modifiers.and || arg === "and" ? "every" : "some"; //some表示只要有一个权限通过即可，every表示必须每个权限都通过
      const hasPermission = value[fn](item => currentPageRoles.includes(item)); // 判断传入的权限码是否在当前页面的权限列表中
      // 如果不包含，则移除该元素
      if (!hasPermission) el.remove();
    } else {
      // 如果传入的值不包含当前页面的角色，则移除该元素
      if (!currentPageRoles.includes(value)) el.remove();
    }
  }
};

export default auth;
