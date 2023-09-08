/**
 * @description 登录模块接口
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
import { Login } from "@/api/interface";
import { moduleRequest } from "@/api/request";

const http = moduleRequest("/auth/b/");

// 用户登录
export const loginApi = (params: Login.LoginForm) => {
  return http.post<Login.Login>("login", params, { noLoading: true }); // 正常 post json 请求  ==>  application/json
};

//获取验证码
export const picCaptchaApi = () => {
  return http.get<Login.ReqValidCode>("getPicCaptcha", {}, { noLoading: true });
};

// 用户退出登录
export const logoutApi = (params: Login.Logout) => {
  return http.post("logout", params);
};

//获取用户信息
export function getLoginUserApi() {
  return http.get<Login.LoginUserInfo>("getLoginUser", {}, { noLoading: true });
}
