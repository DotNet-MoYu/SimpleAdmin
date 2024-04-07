/**
 * @description 系统配置
 * @license Apache License Version 2.0
2022
,
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

import { defineStore } from "pinia";
import piniaPersistConfig from "@/stores/helper/persist";
import { SysConfig, SysOrg, commonApi } from "@/api";
import { SysBaseEnum, TenantEnum } from "@/enums";

const name = "simple-config"; // 定义模块名称
/**  DictState */
export interface SysConfigState {
  /** 系统基本信息 */
  sysInfo: SysConfig.SysBaseConfig;
  /** 租户列表 */
  tenantList: SysOrg.SysOrgInfo[];
  /** 租户id,用来下次登录自动选择租户 */
  tenantId?: number | string;
}

/** 配置模块 */
export const useConfigStore = defineStore({
  id: name,
  state: (): SysConfigState => ({
    sysInfo: {
      SYS_NAME: "",
      SYS_LOGO: "",
      SYS_ICO: "",
      SYS_VERSION: "",
      SYS_COPYRIGHT: "",
      SYS_COPYRIGHT_URL: "",
      SYS_FOOTER_LINKS: [],
      SYS_TENANT_OPTIONS: TenantEnum.CHOSE
    },
    tenantList: [],
    tenantId: undefined
  }),
  getters: {
    sysBaseInfoGet: state => state.sysInfo,
    tenantIdGet: state => state.tenantId,
    tenantListGet: state => state.tenantList
  },
  actions: {
    /**  设置系统基本信息 */
    async setSysBaseInfo() {
      /**  获取系统基本信息 */
      const { data } = await commonApi.sysInfo();
      if (data) {
        //sysConfigProps赋值
        data.forEach((item: SysConfig.ConfigInfo) => {
          //如果是对象类型的属性就转成对象
          if (item.configKey == SysBaseEnum.SYS_DEFAULT_WORKBENCH_DATA) {
            const workBenchData: SysConfig.WorkBenchData = JSON.parse(item.configValue);
            this.sysInfo.SYS_DEFAULT_WORKBENCH_DATA = workBenchData;
          } else if (item.configKey == SysBaseEnum.SYS_FOOTER_LINKS) {
            const footerLinks: SysConfig.FooterLinkProps[] = JSON.parse(item.configValue);
            this.sysInfo.SYS_FOOTER_LINKS = footerLinks;
          } else {
            // 其他属性直接赋值
            (this.sysInfo[item.configKey as keyof SysConfig.SysBaseConfig] as string) = item.configValue;
          }
        });
      }
      return this.sysInfo;
    },
    /** 获取系统基本信息 */
    async getSysBaseInfo() {
      if (this.sysInfo.SYS_NAME != "") {
        return this.sysInfo;
      } else {
        return await this.setSysBaseInfo();
      }
    },
    /** 获取租户列表 */
    async setTenantList() {
      const { data } = await commonApi.tenantList();
      this.tenantList = data;
      return data;
    },
    /** 设置租户ID */
    setTenantId(tenantId: number | string) {
      this.tenantId = tenantId;
    },
    /** 删除租户ID */
    delTenantId() {
      this.tenantId = null;
      this.tenantList = [];
    }
  },
  persist: piniaPersistConfig(name)
});
