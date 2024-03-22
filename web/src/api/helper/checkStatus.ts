/**
 * @description 校验网络请求状态码
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
import { useConfigStore } from "@/stores/modules";
import { ElMessage } from "element-plus";

/**
 * @description: 校验网络请求状态码
 * @param {Number} status
 * @return void
 */
export const checkStatus = (status: number) => {
  switch (status) {
    case 400:
      ElMessage.error("请求失败！请您稍后重试");
      break;
    case 401:
      ElMessage.error("登录失效！请您重新登录");
      break;
    case 403:
      ElMessage.error("当前账号无权限访问！");
      break;
    case 404:
      ElMessage.error("你所访问的资源不存在！");
      break;
    case 405:
      ElMessage.error("请求方式错误！请您稍后重试");
      break;
    case 408:
      ElMessage.error("请求超时！请您稍后重试");
      break;
    case 423:
      const configStore = useConfigStore();
      ElMessage.error(configStore.sysBaseInfoGet.SYS_WEB_CLOSE_PROMPT);
      break;
    case 500:
      ElMessage.error("服务异常！");
      break;
    case 502:
      ElMessage.error("网关错误！");
      break;
    case 503:
      ElMessage.error("服务不可用！");
      break;
    case 504:
      ElMessage.error("网关超时！");
      break;
    default:
      ElMessage.error("请求失败！");
  }
};
