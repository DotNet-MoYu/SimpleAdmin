/**
 * @description 文件管理接口
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

import { ReqPage } from "@/api/interface";

export namespace SysFile {
  /** 文件分页查询 */
  export interface Page extends ReqPage {
    /** 存储引擎 */
    engine?: string;
  }

  /** 文件信息 */
  export interface SysFileInfo {
    /** 文件ID */
    id: number | string;
    /** 文件名 */
    name: string;
    /** 后缀 */
    suffix?: string;
    /** 大小(KB) */
    sizeKb?: number;
    /** 格式化大小 */
    sizeInfo?: string;
    /** 存储引擎 */
    engine?: string;
    /** 存储桶 */
    bucket?: string;
    /** 存储路径 */
    storagePath?: string;
    /** 下载路径 */
    downloadPath?: string;
    /** 缩略图 */
    thumbnail?: string;
    /** 创建时间 */
    createTime?: string;
    /** 更新时间 */
    updateTime?: string;
    /** 创建人 */
    createUser?: string;
    /** 更新人 */
    updateUser?: string;
  }
}
