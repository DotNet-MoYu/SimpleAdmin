/**
 * @description 按钮权限指令
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
import { useAuthStore } from "@/stores/modules/auth";
import type { Directive, DirectiveBinding } from "vue";

// 定义一个auth变量，类型为Directive
const auth: Directive = {
  // 当指令挂载时调用
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    // 获取传入的值
    const { value } = binding;
    // 获取authStore
    const authStore = useAuthStore();
    // 获取用户权限按钮列表
    const currentPageRoles = authStore.authButtonListGet ?? [];
    // 如果传入的值是数组，并且数组的长度大于0
    if (value instanceof Array && value.length) {
      // 判断传入的值是否包含当前页面的角色
      const hasPermission = value.every(item => currentPageRoles.includes(item));
      // 如果不包含，则移除该元素
      if (!hasPermission) el.remove();
    } else {
      // 如果传入的值不包含当前页面的角色，则移除该元素
      if (!currentPageRoles.includes(value)) el.remove();
    }
  }
};

export default auth;
