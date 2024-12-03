/**
 * @description 字典枚举
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
export enum SysDictEnum {
  /** 菜单类型 */
  MENU_TYPE = "MENU_TYPE",
  /** 用户性别类型 */
  GENDER = "GENDER",
  /** 系统通用状态 */
  COMMON_STATUS = "COMMON_STATUS",
  /** 系统角色分类 */
  ROLE_CATEGORY = "ROLE_CATEGORY",
  /** 系统机构分类	 */
  ORG_CATEGORY = "ORG_CATEGORY",
  /** 系统职位分类	 */
  POSITION_CATEGORY = "POSITION_CATEGORY",
  /** 用户民族类型	 */
  NATION = "NATION",
  /** 登录设备类型		 */
  AUTH_DEVICE_TYPE = "AUTH_DEVICE_TYPE",
  /** 系统字典分类	 */
  DICT_CATEGORY = "DICT_CATEGORY",
  /** 文件上传引擎	 */
  FILE_ENGINE = "FILE_ENGINE",
  /** 系统通用开关		 */
  COMMON_SWITCH = "COMMON_SWITCH",
  /** 用户证件类型		 */
  IDCARD_TYPE = "IDCARD_TYPE",
  /** 通用文化程度		 */
  CULTURE_LEVEL = "CULTURE_LEVEL",
  /** 系统消息类型			 */
  MESSAGE_CATEGORY = "MESSAGE_CATEGORY",
  /** 系统消息接受者类型			 */
  RECEIVER_TYPE = "RECEIVER_TYPE",
  /** 系统消息接受者类型			 */
  MESSAGE_WAY = "MESSAGE_WAY",
  /** 系统消息接受者类型			 */
  MESSAGE_STATUS = "MESSAGE_STATUS",
  /** 用户在线状态		 */
  ONLINE_STATUS = "ONLINE_STATUS",
  /** 是否		 */
  YES_NO = "YES_NO",
  /** 多租户选项 */
  TENANT_OPTIONS = "TENANT_OPTIONS",
  /** 验证码类型 */
  CAPTCHA_TYPE = "CAPTCHA_TYPE"
}

/** 菜单类型 */
export enum MenuTypeDictEnum {
  /** 目录 */
  CATALOG = "CATALOG",
  /** 菜单 */
  MENU = "MENU",
  /** 按钮 */
  BUTTON = "BUTTON",
  /** 子页 */
  SUBSET = "SUBSET",
  /** 外链 */
  LINK = "LINK"
}

export enum CommonStatusEnum {
  /** 正常 */
  ENABLE = "ENABLE",
  /** 禁用 */
  DISABLE = "DISABLED"
}

/** 字典类型枚举 */
export enum DictCategoryEnum {
  /** 系统 */
  FRM = "FRM",
  /** 业务 */
  BIZ = "BIZ"
}

/** 角色类型枚举 */
export enum OrgCategoryEnum {
  /** 全局 */
  GLOBAL = "GLOBAL",
  /** 机构 */
  ORG = "ORG"
}

/** 多租户选项枚举 */
export enum TenantEnum {
  /** 手动 */
  CHOSE = "CHOSE",
  /** 关闭 */
  CLOSE = "CLOSE",
  /** 根据域名 */
  DOMAIN = "DOMAIN"
}

/** 消息类型 */
export enum MessageTypeDictEnum {
  /** 通知 */
  INFORM = "INFORM",
  /** 公告 */
  NOTICE = "NOTICE",
  /** 消息 */
  MESSAGE = "MESSAGE"
}

/** 消息通知者类型 */
export enum MessageReceiverTypeDictEnum {
  /** 全部 */
  ALL = "ALL",
  /** 角色 */
  ROLE = "ROLE",
  /** 指定 */
  APPOINT = "APPOINT"
}

/** 消息发送方式 */
export enum MessageSendWayDictEnum {
  /** 立即 */
  NOW = "NOW",
  /** 延迟 */
  DELAY = "DELAY",
  /** 指定 */
  SCHEDULE = "SCHEDULE"
}

/**消息状态 */
export enum MessageStatusDictEnum {
  /** 等待 */
  READY = "READY",
  /** 延迟 */
  ALREADY = "ALREADY"
}
