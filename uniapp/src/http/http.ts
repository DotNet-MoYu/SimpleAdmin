import type { CustomRequestOptions } from '@/http/interceptor'
import { LOADING_URL } from '@/const/systemConst'
import { ResultEnum, TokenEnum } from '@/enum'
import { useUserStore } from '@/store'
import modal from '@/utils/modal'
import tab from '@/utils/tab'

/**  指定方法提示 */
const apiNameArray = ['add', 'edit', 'grant', 'batch', 'update']
/**  指定方法不提示 */
const noMessageApiNameArray: string[] = []

export function http<T>(options: CustomRequestOptions) {
  // 1. 返回 Promise 对象
  return new Promise<IResData<T>>((resolve, reject) => {
    uni.request({
      ...options,
      dataType: 'json',
      // #ifndef MP-WEIXIN
      responseType: 'json',
      // #endif
      // 响应成功
      success(res) {
        checkAndStoreAuthentication(res)
        const data = res.data as IResData<T>
        // 状态码 2xx，参考 axios 的设计
        if (data.code === ResultEnum.SUCCESS) {
          // 统一成功提示
          const responseUrl: string = options.url || '' // 获取请求地址
          apiNameArray.forEach((apiName) => {
            const responseApiArray = responseUrl.split('/') // 分割
            const method
              = responseApiArray[responseApiArray.length - 1] // 取最后一个
            const result
              = noMessageApiNameArray.includes(method) // 判断是否在不提示的数组中
            if (!result && responseUrl.includes(apiName)) {
              // 如果不在不提示的数组中并且请求地址包含指定的方法
              modal.msg(data.msg || '操作成功')
            }
          })
          resolve(data)
        }
        else if (data.code === ResultEnum.OVERDUE) {
          const userStore = useUserStore()
          userStore.clearUserStore()
          modal.msgError(data.msg || '登录已过期，请重新登录！')
          tab.reLaunch({
            url: LOADING_URL,
          })
          reject(res)
        }
        else {
          modal.msg(data.msg || '请求错误')
          reject(res)
        }
      },
      // 响应失败
      fail(err) {
        uni.showToast({
          icon: 'none',
          title: '网络错误，换个网络试试',
        })
        reject(err)
      },
    })
  })
}

/**
 * GET 请求
 * @param url 后台地址
 * @param query 请求query参数
 * @param header 请求头，默认为json格式
 * @returns
 */
export function httpGet<T>(url: string, query?: Record<string, any>, header?: Record<string, any>, options?: Partial<CustomRequestOptions>) {
  return http<T>({
    url,
    query,
    method: 'GET',
    header,
    ...options,
  })
}

/**
 * POST 请求
 * @param url 后台地址
 * @param data 请求body参数
 * @param query 请求query参数，post请求也支持query，很多微信接口都需要
 * @param header 请求头，默认为json格式
 * @returns
 */
export function httpPost<T>(url: string, data?: Record<string, any>, query?: Record<string, any>, header?: Record<string, any>, options?: Partial<CustomRequestOptions>) {
  return http<T>({
    url,
    query,
    data,
    method: 'POST',
    header,
    ...options,
  })
}
/**
 * PUT 请求
 */
export function httpPut<T>(url: string, data?: Record<string, any>, query?: Record<string, any>, header?: Record<string, any>, options?: Partial<CustomRequestOptions>) {
  return http<T>({
    url,
    data,
    query,
    method: 'PUT',
    header,
    ...options,
  })
}

/**
 * DELETE 请求（无请求体，仅 query）
 */
export function httpDelete<T>(url: string, query?: Record<string, any>, header?: Record<string, any>, options?: Partial<CustomRequestOptions>) {
  return http<T>({
    url,
    query,
    method: 'DELETE',
    header,
    ...options,
  })
}

http.get = httpGet
http.post = httpPost
http.put = httpPut
http.delete = httpDelete

// 支持与 alovaJS 类似的API调用
http.Get = httpGet
http.Post = httpPost
http.Put = httpPut
http.Delete = httpDelete

/**
 * 检查并存储授权信息
 * @param res 响应对象
 */
function checkAndStoreAuthentication(res: UniApp.RequestSuccessCallbackResult) {
  console.log('[ 检查并存储token ] >', res)
  const userStore = useUserStore()
  // 读取响应报文头 token 信息
  const accessToken = res.header[TokenEnum.ACCESS_TOKEN_KEY]
  const refreshAccessToken = res.header[TokenEnum.REFRESH_TOKEN_KEY]
  // 判断是否是无效 token
  if (accessToken === 'invalid_token') {
    userStore.clearToken() // 清除 token
  }
  // 判断是否存在刷新 token，如果存在则存储在本地
  else if (
    refreshAccessToken
    && accessToken
    && accessToken !== 'invalid_token'
  ) {
    userStore.setToken(accessToken, refreshAccessToken)
  }
}
