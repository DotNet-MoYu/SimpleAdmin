/**
 * @description
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
import type { AxiosRequestConfig } from "axios";
import RequestHttp from "./instance";

/**
 * @description 创建请求实例
 * @param axiosConfig axios 配置
 */
export function createRequest(axiosConfig: AxiosRequestConfig) {
  const customInstance = new RequestHttp(axiosConfig);

  /**
   * get请求
   * @param url - 请求地址
   * @param config - axios配置
   */
  async function get<T>(url: string, params?: object, _object = {}) {
    return customInstance.get<T>(url, params, _object);
  }

  /**
   * post请求
   * @param url - 请求地址
   * @param data - 请求的body的data
   * @param config - axios配置
   */
  async function post<T>(url: string, params?: any, _object = {}) {
    return customInstance.post<T>(url, params, _object);
  }
  /**
   * put请求
   * @param url - 请求地址
   * @param data - 请求的body的data
   * @param config - axios配置
   */
  async function put<T>(url: string, params?: any, _object = {}) {
    return customInstance.put<T>(url, params, _object);
  }

  /**
   * delete请求
   * @param url - 请求地址
   * @param config - axios配置
   */
  async function handleDelete<T>(url: string, params?: any, _object = {}) {
    return customInstance.delete<T>(url, params, _object);
  }

  /**
   *
   * 下载
   * @param url - 请求地址
   * @param config - axios配置
   */
  async function download(url: string, params?: object, _object = {}): Promise<BlobPart> {
    return customInstance.download(url, params, { ..._object, responseType: "blob" });
  }

  return {
    get,
    post,
    put,
    delete: handleDelete,
    download
  };
}
