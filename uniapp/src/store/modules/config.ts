/**
 * @description 配置模块
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

import type { SysConfig, SysOrg } from '@/api'
import { defineStore } from 'pinia'
import { commonApi } from '@/api'
import { SysBaseEnum, TenantEnum } from '@/enum'

export const useConfigStore = defineStore(
  'simple-config',
  () => {
    const sysInfo = reactive<SysConfig.SysBaseConfig>({
      SYS_NAME: '',
      SYS_LOGO: '',
      SYS_ICO: '',
      SYS_VERSION: '',
      SYS_COPYRIGHT: '',
      SYS_COPYRIGHT_URL: '',
      SYS_FOOTER_LINKS: [],
      SYS_TENANT_OPTIONS: TenantEnum.CHOSE,
    })

    const tenantList = ref<SysOrg.SysOrgInfo[]>([])
    const tenantId = ref<number | string | null>(null)

    /** 设置系统基本信息 */
    const setSysBaseInfo = async () => {
      const { data } = await commonApi.sysInfo()
      if (data) {
        data.forEach((item: SysConfig.ConfigInfo) => {
          if (item.configKey === SysBaseEnum.SYS_FOOTER_LINKS) {
            sysInfo.SYS_FOOTER_LINKS = JSON.parse(item.configValue)
          }
          else {
            const key = item.configKey as keyof SysConfig.SysBaseConfig
            sysInfo[key] = item.configValue as any
          }
        })
      }
      return sysInfo
    }

    /** 获取系统基本信息 */
    const getSysBaseInfo = async () => {
      return sysInfo.SYS_NAME !== '' ? sysInfo : await setSysBaseInfo()
    }

    /** 设置租户列表 */
    const setTenantList = async () => {
      const { data } = await commonApi.tenantList()
      tenantList.value = data
      return data
    }

    /** 设置租户 ID */
    const setTenantId = (id: number | string) => {
      tenantId.value = id
    }

    /** 删除租户 ID */
    const delTenantId = () => {
      tenantId.value = null
      tenantList.value = []
    }

    return {
      sysInfo,
      tenantList,
      tenantId,
      setSysBaseInfo,
      getSysBaseInfo,
      setTenantList,
      setTenantId,
      delTenantId,
    }
  },
  {
    persist: true,
  },
)
