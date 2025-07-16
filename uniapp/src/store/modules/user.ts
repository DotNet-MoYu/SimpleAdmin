/**
 * @description 用户管理
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

import type { Login, MobileUserCenter } from '@/api/interface'
import { defineStore } from 'pinia'
import { useToast } from 'wot-design-uni'
import { userMobileCenterApi } from '@/api'

/** 定义用户 store */
export const useUserStore = defineStore(
  'simple-user',
  () => {
    // -------------------------
    // ✅ 状态 State
    // -------------------------

    /** 访问令牌，用于接口身份验证 */
    const accessToken = ref<string>('')

    /** 刷新令牌，用于 token 过期续签 */
    const refreshToken = ref<string>('')

    /** 用户信息对象 */
    const userInfo = ref<MobileUserCenter.MobileLoginUserInfo | null>(null)

    /** 默认模块 ID（后端返回的默认模块） */
    const defaultModule = ref<number | string | null>(null)

    /** 当前用户手动选择的模块 ID */
    const chooseModule = ref<number | string | null>(null)

    /** 用户拥有的模块列表 */
    const moduleList = ref<Login.ModuleInfo[]>([])

    /** 用户的菜单资源权限（移动端） */
    const userMobileMenus = ref<MobileUserCenter.MobileResource[]>([])

    // -------------------------
    // ✅ 方法 Actions
    // -------------------------

    /**
     * 设置 token 和 refreshToken
     * @param token 访问令牌
     * @param refresh 刷新令牌
     */
    const setToken = (token: string, refresh: string) => {
      accessToken.value = token
      refreshToken.value = refresh
    }

    /**
     * 设置用户信息
     * 并同步设置默认模块和模块列表
     * @param info 用户信息对象
     */
    const setUserInfo = (info: MobileUserCenter.MobileLoginUserInfo) => {
      userInfo.value = info
      defaultModule.value = info.defaultModule
      moduleList.value = info.moduleList
    }

    /**
     * 获取用户信息（从后台接口）
     * @returns 用户信息对象或 null
     */
    const getUserInfo = async () => {
      const toast = useToast()
      const { data } = await userMobileCenterApi.getLoginUser()
      if (data) {
        setUserInfo(data)
      }
      else {
        toast.error('获取个人信息失败，请联系系统管理员！')
      }
      return userInfo.value
    }

    /**
     * 设置用户签名
     * @param signature 签名字符串
     */
    const setSignature = (signature: string) => {
      if (userInfo.value) {
        userInfo.value.signature = signature
      }
    }

    /**
     * 动态设置用户信息中的某个字段
     * @param key 字段名
     * @param value 字段值
     */
    const setUserInfoItem = (key: string, value: any) => {
      if (userInfo.value) {
        ; (userInfo.value as any)[key] = value
      }
    }

    /**
     * 清空 token 信息（注销登录时使用）
     */
    const clearToken = () => {
      accessToken.value = ''
      refreshToken.value = ''
    }

    /**
     * 清空整个用户 Store 的数据（完全登出）
     */
    const clearUserStore = () => {
      clearToken()
      userInfo.value = null
      defaultModule.value = null
      moduleList.value = []
    }

    /**
     * 设置当前选中的模块 ID
     * @param moduleId 模块 ID
     */
    const setModule = (moduleId: number | string | null) => {
      chooseModule.value = moduleId
    }

    /**
     * 设置用户菜单权限（用于移动端路由渲染）
     * @param menus 菜单资源数组
     */
    const setUserMobileMenus = (menus: MobileUserCenter.MobileResource[]) => {
      userMobileMenus.value = menus
    }

    // -------------------------
    // ✅ 导出给组件使用
    // -------------------------
    return {
      accessToken,
      refreshToken,
      userInfo,
      defaultModule,
      chooseModule,
      moduleList,
      userMobileMenus,
      // actions
      setToken,
      getUserInfo,
      setUserInfo,
      setSignature,
      setUserInfoItem,
      clearToken,
      clearUserStore,
      setModule,
      setUserMobileMenus,
    }
  },
  {
    // 启用数据持久化（默认使用 localStorage）
    persist: true,
  },
)
