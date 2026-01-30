import type { CustomRequestOptions } from '@/http/types'
import { RequestEnum, ResultEnum, TokenEnum } from '@/enum'
import { useUserStore } from '@/store'
import { getEnvBaseUrl } from '@/utils'
import { stringifyQuery } from './tools/queryString'

// 请求基准地址
const baseUrl = getEnvBaseUrl()

// 拦截器配置
const httpInterceptor = {
  // 拦截前触发
  invoke(options: CustomRequestOptions) {
    // 添加时间戳参数防缓存（仅 GET 请求）
    if (
      (options.method || RequestEnum.GET).toUpperCase()
      === RequestEnum.GET
    ) {
      options.query = {
        ...(options.query || {}),
        t: Date.now(),
      }
    }
    // 接口请求支持通过 query 参数配置 queryString
    if (options.query) {
      console.log('[ queryStr ] >', options.query)
      const queryStr = stringifyQuery(options.query)
      if (options.url.includes('?')) {
        options.url += `&${queryStr}`
      }
      else {
        options.url += `?${queryStr}`
      }
    }
    // 非 http 开头需拼接地址
    if (!options.url.startsWith('http')) {
      // #ifdef H5
      // console.log(__VITE_APP_PROXY__)
      if (JSON.parse(__VITE_APP_PROXY__)) {
        // 自动拼接代理前缀
        options.url
          = import.meta.env.VITE_APP_PROXY_PREFIX + options.url
      }
      else {
        options.url = baseUrl + options.url
      }
      // #endif
      // 非H5正常拼接
      // #ifndef H5
      options.url = baseUrl + options.url
      // #endif
      // TIPS: 如果需要对接多个后端服务，也可以在这里处理，拼接成所需要的地址
    }
    console.log('[ oprions.url2 ] >', options.url)
    // 1. 请求超时
    options.timeout = ResultEnum.TIMEOUT as number // 10s
    // 2. （可选）添加小程序端请求头标识
    options.header = {
      ...options.header,
    }

    // 3. 添加 token 请求头标识
    const userStore = useUserStore()
    const { accessToken, refreshToken } = userStore
    if (accessToken) {
      const token_key = TokenEnum.TOKEN_NAME as string
      options.header[token_key] = TokenEnum.TOKEN_PREFIX + accessToken
      // 判断 accessToken 是否过期
      const jwt = decryptJWT(accessToken)
      const exp = getJWTDate(jwt.exp)
      // token 已经过期
      if (new Date() >= exp) {
        // 获取刷新 token
        const refreshAccessToken = refreshToken
        // 携带刷新 token
        if (refreshAccessToken) {
          options.header[`X-${token_key}`]
            = TokenEnum.TOKEN_PREFIX + refreshAccessToken
        }
      }
    }
  },
}

export const requestInterceptor = {
  install() {
    // 拦截 request 请求
    uni.addInterceptor('request', httpInterceptor)
    // 拦截 uploadFile 文件上传
    uni.addInterceptor('uploadFile', httpInterceptor)
  },
}

/**
 * @description 解密 JWT token 的信息
 * @param token jwt token 字符串
 * @returns <any>object
 */
function decryptJWT(token: string) {
  try {
    const payload = token.split('.')[1]
    const jsonStr = base64UrlDecode(payload)
    return JSON.parse(jsonStr)
  }
  catch (err) {
    console.error('JWT 解析失败:', err)
    return {}
  }
}

/**
 * @description 将 JWT 时间戳转换成 Date，主要针对 `exp`，`iat`，`nbf`
 * @param timestamp 时间戳
 * @returns Date 对象
 */
function getJWTDate(timestamp: number) {
  return new Date(timestamp * 1000)
}

function base64UrlDecode(str: string): string {
  // Base64 URL 解码（将 URL-safe 编码还原）
  str = str.replace(/-/g, '+').replace(/_/g, '/')

  // 补齐 padding
  const pad = str.length % 4
  if (pad) {
    str += '='.repeat(4 - pad)
  }

  // H5 或支持 atob 的平台
  if (typeof atob === 'function') {
    try {
      const decoded = atob(str)
      return decodeURIComponent(escape(decoded)) // 转换中文等
    }
    catch (err) {
      console.error('base64 解码失败 (atob):', err)
    }
  }

  // 微信小程序使用 wx.base64ToArrayBuffer + wx.arrayBufferToBase64（兼容更好）
  // #ifdef MP-WEIXIN
  try {
    const byteArray = wx.base64ToArrayBuffer(str)
    const binary = String.fromCharCode.apply(
      null,
      new Uint8Array(byteArray) as any,
    )
    return decodeURIComponent(escape(binary))
  }
  catch (err) {
    console.error('base64 解码失败 (微信):', err)
  }
  // #endif

  return ''
}
