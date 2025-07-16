/**
 * @description 认证
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
import type { Login, ReqId } from '@/api/interface'
import { defineStore } from 'pinia'
import { loginApi, userMobileCenterApi } from '@/api'

import { HOME_URL } from '@/const/systemConst'
import { useConfigStore } from './config'
import { useDictStore } from './dict'
import { useUserStore } from './user'

export const useAuthStore = defineStore(
  'simple-auth',
  () => {
    // 定义登录加载状态
    const loginLoading = ref(false)
    // 定义是否开启选择模块
    const showChooseModule = ref(false)
    // 定义模块列表
    const moduleList = ref<Login.ModuleInfo[]>([])
    // 定义菜单权限列表
    const authButtonList = ref<string[]>([])
    // 定义菜单权限列表
    const authMenuList = ref<Menu.MenuOptions[]>([])

    // 设置模块列表
    const setModuleList = (list: Login.ModuleInfo[]) => {
      moduleList.value = list
    }

    // 选择模块
    const chooseModule = async (config: ReqId) => {
      const userStore = useUserStore()
      userStore.setModule(config.id)
      showChooseModule.value = false
    }

    // 获取菜单权限列表
    const getAuthButtonList = async () => {
      const userStore = useUserStore()
      authButtonList.value = userStore.userInfo?.buttonCodeList || []
    }

    // 获取菜单权限列表
    const getAuthMenuList = async (moduleId: number | string) => {
      // const { data } = await userCenterApi.getAuthMenuList({ id: moduleId })
      // authMenuList.value = data
      console.log(moduleId)
    }

    // 设置租户ID
    const setTenantId = (model: Login.LoginForm | Login.PhoneLoginForm) => {
      if (model.tenantId) {
        const configStore = useConfigStore()
        configStore.setTenantId(model.tenantId)
      }
    }

    // 登录后处理
    const handleActionAfterLogin = async () => {
      const dictStore = useDictStore()
      const userStore = useUserStore()

      try {
        await Promise.all([
          userMobileCenterApi.getLoginUser().then((res) => {
            userStore.setUserInfo(res.data)
          }),
          dictStore.setDictTree(),
          userMobileCenterApi.loginMobileMenu().then((res) => {
            userStore.setUserMobileMenus(res.data)
          }),
        ])
        uni.switchTab({ url: HOME_URL })
      }
      catch (error) {
        console.error(error)
      }
    }

    // 密码登录
    const loginPwd = async (model: Login.LoginForm) => {
      loginLoading.value = true
      setTenantId(model)
      try {
        const res = await loginApi.login(model)
        if (res.data) {
          await handleActionAfterLogin()
        }
      }
      catch (err) {
        return Promise.reject(err)
      }
      finally {
        loginLoading.value = false
      }
    }

    // 手机登录
    const loginPhone = async (model: Login.PhoneLoginForm) => {
      loginLoading.value = true
      setTenantId(model)
      try {
        const res = await loginApi.loginByPhone(model)
        if (res.data) {
          await handleActionAfterLogin()
        }
      }
      catch (err) {
        return Promise.reject(err)
      }
      finally {
        loginLoading.value = false
      }
    }

    return {
      loginLoading,
      showChooseModule,
      moduleList,
      authButtonList,
      authMenuList,
      setModuleList,
      chooseModule,
      getAuthButtonList,
      getAuthMenuList,
      loginPwd,
      loginPhone,
      setTenantId,
      handleActionAfterLogin,
    }
  },
  {
    persist: true,
  },
)
