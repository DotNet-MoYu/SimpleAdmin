/**
 * @description 封装 axios 请求类
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
import axios, { AxiosInstance, AxiosError, AxiosRequestConfig, InternalAxiosRequestConfig, AxiosResponse } from "axios";
import { showFullScreenLoading, tryHideFullScreenLoading } from "@/components/Loading/fullScreen";
import { LOGIN_URL } from "@/config";
import { ElMessage } from "element-plus";
import { ResultData } from "@/api/interface";
import { ResultEnum, TokenEnum } from "@/enums";
import { checkStatus } from "../helper/checkStatus";
import { useUserStore } from "@/stores/modules";
import { AxiosCanceler } from "../helper/axiosCancel";
import router from "@/routers";

// 自定义 AxiosRequestConfig 接口，增加 noLoading 属性
export interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  loading?: boolean;
  cancel?: boolean;
}
const axiosCanceler = new AxiosCanceler();

/**
 * @Description: http请求
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54

 */
export default class RequestHttp {
  service: AxiosInstance;
  /**
   * @description 构造函数
   * @param config axios 配置
   */
  public constructor(config: AxiosRequestConfig) {
    // axios 实例化
    this.service = axios.create(config);
    // 设置请求拦截器
    this.setInterceptor();
  }
  /**  指定方法提示 */
  apiNameArray = ["add", "edit", "grant", "batch", "update"];
  /**  指定方法不提示 */
  noMessageApiNameArray: string[] = [];
  /**
   * @description 设置请求拦截器
   */
  setInterceptor() {
    /**
     * @description 请求拦截器
     * 客户端发送请求 -> [请求拦截器] -> 服务器
     * token校验(JWT) : 接受服务器返回的 token,存储到 vuex/pinia/本地储存当中
     */
    this.service.interceptors.request.use(
      (config: CustomAxiosRequestConfig) => {
        const userStore = useUserStore();
        // 重复请求不需要取消，在 api 服务中通过指定的第三个参数: { cancel: false } 来控制
        config.cancel ??= true;
        config.cancel && axiosCanceler.addPending(config);
        // 当前请求不需要显示 loading，在 api 服务中通过指定的第三个参数: { loading: false } 来控制
        config.loading ??= true;
        config.loading && showFullScreenLoading();
        //检查 config.headers 对象是否存在以及是否具有 set 方法
        if (config.headers && typeof config.headers.set === "function") {
          const { accessToken, refreshToken } = userStore;
          if (accessToken) {
            config.headers.set(TokenEnum.TOKEN_NAME, TokenEnum.TOKEN_PREFIX + accessToken);
            // 判断 accessToken 是否过期
            const jwt = this.decryptJWT(accessToken);
            const exp = this.getJWTDate(jwt.exp);
            // token 已经过期
            if (new Date() >= exp) {
              // 获取刷新 token
              const refreshAccessToken = refreshToken;
              // 携带刷新 token
              if (refreshAccessToken) {
                config.headers.set("X-" + TokenEnum.TOKEN_NAME, TokenEnum.TOKEN_PREFIX + refreshAccessToken);
              }
            }
          }
        }
        // get请求加时间戳
        if (config.method === "get") {
          config.params = {
            ...config.params,
            _t: new Date().getTime()
          };
        }
        return config;
      },
      (error: AxiosError) => {
        return Promise.reject(error);
      }
    );

    /**
     * @description 响应拦截器
     *  服务器换返回信息 -> [拦截统一处理] -> 客户端JS获取到信息
     */
    this.service.interceptors.response.use(
      (response: AxiosResponse & { config: CustomAxiosRequestConfig }) => {
        // 检查并存储授权信息
        this.checkAndStoreAuthentication(response);
        const { data, config } = response;
        const userStore = useUserStore();
        axiosCanceler.removePending(config);
        config.loading && tryHideFullScreenLoading();
        // 登录失效
        if (data.code == ResultEnum.OVERDUE) {
          userStore.clearUserStore();
          router.replace(LOGIN_URL);
          ElMessage.error(data.msg);
          return Promise.reject(data);
        }
        // 全局错误信息拦截（防止下载文件的时候返回数据流，没有 code 直接报错）
        if (data.code && data.code !== ResultEnum.SUCCESS) {
          ElMessage.error(data.msg);
          return Promise.reject(data);
        } else {
          // 统一成功提示
          const responseUrl: string = response.config.url || ""; //获取请求地址
          this.apiNameArray.forEach(apiName => {
            let responseApiArray = responseUrl.split("/"); //分割
            let method = responseApiArray[responseApiArray.length - 1]; //取最后一个
            let result = this.noMessageApiNameArray.includes(method); //判断是否在不提示的数组中
            if (!result && responseUrl.includes(apiName)) {
              //如果不在不提示的数组中并且请求地址包含指定的方法
              ElMessage.success(data.msg);
            }
          });
        }
        // 成功请求（在页面上除非特殊情况，否则不用处理失败逻辑）
        return data;
      },
      async (error: AxiosError) => {
        const { response } = error;
        tryHideFullScreenLoading();
        // 请求超时 && 网络错误单独判断，没有 response
        if (error.message.indexOf("timeout") !== -1) ElMessage.error("请求超时！请您稍后重试");
        if (error.message.indexOf("Network Error") !== -1) ElMessage.error("网络错误！请您稍后重试");
        // 根据服务器响应的错误状态码，做不同的处理
        if (response) checkStatus(response.status);
        // 服务器结果都没有返回(可能服务器错误可能客户端断网)，断网处理:可以跳转到断网页面
        if (!window.navigator.onLine) router.replace("/500");
        return Promise.reject(error);
      }
    );
  }

  /**
   * @description get 请求
   * @param url 请求地址
   * @param params 请求参数
   * @param _object axios 配置
   * @returns Promise<ResultData<T>>
   */
  get<T>(url: string, params?: object, _object = {}): Promise<ResultData<T>> {
    return this.service.get(url, { params, ..._object });
  }

  /**
   * @description post 请求
   * @param url 请求地址
   * @param params 请求参数
   * @param _object axios 配置
   * @returns Promise<ResultData<T>>
   */
  post<T>(url: string, params?: object | string, _object = {}): Promise<ResultData<T>> {
    return this.service.post(url, params, _object);
  }

  /**
   * @description put 请求
   * @param url 请求地址
   * @param params 请求参数
   * @param _object axios 配置
   * @returns Promise<ResultData<T>>
   */
  put<T>(url: string, params?: object, _object = {}): Promise<ResultData<T>> {
    return this.service.put(url, params, _object);
  }

  /**
   * @description delete 请求
   * @param url 请求地址
   * @param params 请求参数
   * @param _object axios 配置
   * @returns Promise<ResultData<T>>
   */
  delete<T>(url: string, params?: any, _object = {}): Promise<ResultData<T>> {
    return this.service.delete(url, { params, ..._object });
  }

  /**
   * @description 下载文件
   * @param url 请求地址
   * @param params 请求参数
   * @param _object axios 配置
   * @returns Promise<BlobPart>
   */
  download(url: string, params?: object, _object = {}): Promise<BlobPart> {
    return this.service.post(url, params, { ..._object, responseType: "blob" });
  }

  /**
   * @description 解密 JWT token 的信息
   * @param token jwt token 字符串
   * @returns <any>object
   */
  decryptJWT(token: string) {
    token = token.replace(/_/g, "/").replace(/-/g, "+");
    const json = decodeURIComponent(escape(window.atob(token.split(".")[1])));
    return JSON.parse(json);
  }

  /**
   * @description 将 JWT 时间戳转换成 Date，主要针对 `exp`，`iat`，`nbf`
   * @param timestamp 时间戳
   * @returns Date 对象
   */
  getJWTDate(timestamp: number) {
    return new Date(timestamp * 1000);
  }

  /**
   * 检查并存储授权信息
   * @param res 响应对象
   */
  checkAndStoreAuthentication(res: AxiosResponse) {
    // 读取响应报文头 token 信息
    let accessToken = res.headers[TokenEnum.ACCESS_TOKEN_KEY];
    let refreshAccessToken = res.headers[TokenEnum.REFRESH_TOKEN_KEY];
    const userStore = useUserStore();
    // 判断是否是无效 token
    if (accessToken === "invalid_token") {
      userStore.clearToken(); // 清除 token
    }
    // 判断是否存在刷新 token，如果存在则存储在本地
    else if (refreshAccessToken && accessToken && accessToken !== "invalid_token") {
      userStore.setToken(accessToken, refreshAccessToken);
    }
  }
}
