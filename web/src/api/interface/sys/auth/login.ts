/**
 * @description 登录模块接口
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

/**
 * @Description:
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
export namespace Login {
  /** 模块信息 */
  export type ModuleInfo = {
    id: number | string;
    title: string;
    code: string;
    icon: string;
    description: string;
  };

  /**
   * 验证码
   */
  interface ValidCode {
    /**  验证码 */
    validCode: string;
    /** 验证码请求号 */
    validCodeReqNo: string;
  }

  /**
   * 账号密码登录表单
   */
  export interface LoginForm extends ValidCode {
    account: string;
    password: string;
    tenantId?: number | string;
  }

  /**
   * 手机号登录表单
   */
  export interface PhoneLoginForm extends ValidCode {
    /** 手机号 */
    phone: string;
    /** 租户Id */
    tenantId?: number | string;
  }

  /** 获取手机验证码请求 */
  export interface ReqPhoneValidCode extends ValidCode {
    /** 手机号 */
    phone: string;
  }

  /**
   * 注销表单
   */
  export interface Logout {
    token: string;
  }

  // 登录返回
  export interface Login {
    /** token */
    token: string;
    /** 默认模块 */
    defaultModule: string;
    /** 模块列表 */
    moduleList: ModuleInfo[];
  }

  /**
   * 验证码返回
   */
  export interface ReqValidCode {
    /** 验证码 */
    validCodeBase64: string;
    /** 验证码请求号 */
    validCodeReqNo: string;
  }

  /** 用户信息 */
  export interface LoginUserInfo {
    /** 用户id */
    id: string | number;
    /** 用户名 */
    account: string;
    /** 用户姓名 */
    name: string;
    /** 用户昵称 */
    nickname: string;
    /** 用户头像 */
    avatar: string;
    /** 用户性别 */
    gender: string;
    /** 民族 */
    nation: string;
    /** 出生日期 */
    birthday: string;
    /** 家庭住址 */
    homeAddress: string;
    /** 电话号码 */
    phone: string;
    /** 邮箱 */
    email: string;
    /** 用户签名 */
    signature: string;
    /** 默认模块 */
    defaultModule: number | string;
    /** 模块列表 */
    moduleList: ModuleInfo[];
    /** 组织全程 */
    orgNames: string;
    /** 职位名称 */
    positionName: string;
    /** 按钮码集合 */
    buttonCodeList: string[];
    /** 权限码集合 */
    permissionCodeList: string[];
    /** 角色码集合 */
    roleCodeList: string[];
  }
}
