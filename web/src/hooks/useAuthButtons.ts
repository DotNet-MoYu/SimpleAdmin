/**
 * @description 权限判断 hooks
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

import { useAuthStore } from "@/stores/modules";
import { isArray } from "@/utils/is";

export const useAuthButtons = () => {
  const authStore = useAuthStore();
  const authButtons = authStore.authButtonListGet; // 权限按钮列表

  /**
   * @func hasPermission
   * @desc   权限判断是否能看到这个按钮，同时后端也做了校验，前端只是显示与不显示
   * @param {}   permission 按钮的权限码，可以是单个字符串，也可以是数组
   * @param {}   and 是否完全匹配，默认false，即只要有一个权限码通过则通过，如果为 true，则必须每个权限码都通过
   * 使用方法：
   * 例如 buttonCodeList 的数据为： ['button1', 'button2', 'button3']
   * 想要判断 button1 的权限，可以写成：hasPerm('button1')
   * 想要判断 button1 或 button2 的权限，可以写成：hasPerm(['button1', 'button2' ])
   * 想要判断 button1 与 button2 的权限，可以写成：hasPerm(['button1', 'button2' ], true)
   * @return {}
   */
  const hasPerm = (permission: string[] | string, and: boolean = false) => {
    //如果是数组，则需要判断数组中的每一项权限
    if (isArray(permission)) {
      const fn = and ? "every" : "some"; //some表示只要有一个权限通过即可，every表示必须每个权限都通过
      return permission[fn](item => authButtons.includes(item)); //多个权限
    } else {
      return authButtons.includes(permission); //单个权限
    }
  };

  return { hasPerm };
};
