/**
 * @description
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
import { ResultEnum } from "@/enums/httpEnum";
import { createRequest } from "./request";

const isHttpProxy = import.meta.env.VITE_HTTP_PROXY === "true"; // 是否使用代理
const url = import.meta.env.VITE_API_URL as string; // 请求地址

/**
 * @description 模块内的请求, 会自动加上模块的前缀
 * @param moduleUrl 模块地址
 * @param prefix  前缀
 */
export const moduleRequest = (moduleUrl: string, prefix: string = "/api") =>
  createRequest({
    //请求地址,可在 .env.** 文件中修改
    baseURL: isHttpProxy ? prefix + moduleUrl : url + prefix + moduleUrl,
    // 设置超时时间
    timeout: ResultEnum.TIMEOUT as number,
    // 跨域时候允许携带凭证
    withCredentials: true
  });
