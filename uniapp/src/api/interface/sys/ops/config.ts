/**
 * @description 系统配置请求接口
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

import type { TenantEnum } from '@/enum'

/**
 * @Description: 系统配置请求接口
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
/** 系统配置返回 */
export interface ConfigInfo {
  /** id */
  id: number | string
  /** 分类 */
  category: string
  /** key */
  configKey: string
  /** value */
  configValue: string
  /** 描述 */
  remark?: string
  /** 排序 */
  sortCode?: number
}

/** 工作台 */
export interface WorkBenchData {
  /** 快捷方式 */
  shortcut: number[] | string[]
}

// 超链接接口
export interface FooterLinkProps {
  /** 标题 */
  name: string
  /** 地址 */
  url: string
  /** 排序 */
  sortCode: number
}

/** 系统配置接口 */
export interface SysBaseConfig {
  /** 系统logo */
  SYS_LOGO: string
  /** 系统ICO文件 */
  SYS_ICO?: string
  /** 系统名称 */
  SYS_NAME: string
  /** 系统版本 */
  SYS_VERSION: string
  /** 系统版权 */
  SYS_COPYRIGHT: string
  /** 系统版权链接地址 */
  SYS_COPYRIGHT_URL: string
  /** 系统默认工作台数据 */
  SYS_DEFAULT_WORKBENCH_DATA?: WorkBenchData
  /** 网站状态 */
  SYS_WEB_STATUS?: string
  /** 网站关闭提示 */
  SYS_WEB_CLOSE_PROMPT?: string
  /** 多租户选项 */
  SYS_TENANT_OPTIONS: TenantEnum
  /** 底部超链接 */
  SYS_FOOTER_LINKS: FooterLinkProps[]
}

/** 登录配置接口 */
export interface LoginPolicyConfig {
  /** 单用户登录开关 */
  LOGIN_SINGLE_OPEN: string
  /** 登录验证码开关 */
  LOGIN_CAPTCHA_OPEN: string
  /** 登录验证码类型 */
  LOGIN_CAPTCHA_TYPE: string
  /** 登录错误次数 */
  LOGIN_ERROR_COUNT: number
  /** 登录重置时间 */
  LOGIN_ERROR_RESET_TIME: number
  /** 登录错误锁定时间 */
  LOGIN_ERROR_LOCK: number
}

/** 密码配置接口 */
export interface PwdPolicyConfig {
  /** 默认密码 */
  PWD_DEFAULT_PASSWORD: string
  /** 密码定期提醒更新 */
  PWD_REMIND: string
  /** 密码提醒时间 */
  PWD_REMIND_DAY: number
  /** 修改初始密码提醒 */
  PWD_UPDATE_DEFAULT: string
  /** 密码最小长度 */
  PWD_MIN_LENGTH: number
  /** 包含数字 */
  PWD_CONTAIN_NUM: string
  /** 包含小写字母 */
  PWD_CONTAIN_LOWER: string
  /** 包含大写字母 */
  PWD_CONTAIN_UPPER: string
  /** 包含特殊字符 */
  PWD_CONTAIN_CHARACTER: string
}

/** mqtt配置接口 */
export interface MqttPolicyConfig {
  /** MQTT服务端地址 */
  MQTT_PARAM_URL: string
  /** MQTT用户名 */
  MQTT_PARAM_USERNAME: string
  /** MQTT密码 */
  MQTT_PARAM_PASSWORD: string
}
