/**
 * @description 代码生成
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

import { ReqPage, ReqId } from "@/api/interface";

/**
 * @Description: 生成代码配置接口
 * @Author: huguodong
 * @Date: 2025-01-07 13:06:14
 */
export namespace GenCode {
  /** 代码生成基础 */
  export interface GenBasic {
    /** ID */
    id: number | string;
    /** 库名 */
    configId: string;
    /** 主表 */
    dbTable: string;
    /** 实体名称 */
    entityName: string;
    /** 功能列表 */
    functions: string;
    /** 数据权限 */
    dataPermission: string;
    /** 生成模版 */
    moduleType: string;
    /** 树Id字段 */
    treeId: string;
    /** 树父Id字段 */
    treePid: string;
    /** 树名称字段 */
    treeName: string;
    /** 关联子表名 */
    childTable: string;
    /** 关联子表外键 */
    childFk: string;
    /** 表前缀 */
    tablePrefix: string;
    /** 生成方式 */
    generateType: string;
    /** 所属模块 */
    module: number;
    /** 上级目录 */
    menuPid: number;
    /** 路由名 */
    routeName: string;
    /** 图标 */
    icon: string;
    /** 功能名 */
    functionName: string;
    /** 功能名后缀 */
    functionNameSuffix: string;
    /** 业务名 */
    busName: string;
    /** 类名 */
    className: string;
    /** 表单布局 */
    formLayout: string;
    /** 使用栅格 */
    gridWhether: string;
    /** 左树 */
    leftTree: string;
    /** 前端路径 */
    frontedPath: string;
    /** 服务代码存放项目 */
    servicePosition: string;
    /** 服务代码存放文件夹 */
    serviceDictionary: string;
    /** 控制器代码存放位置 */
    controllerPosition: string;
    /** 控制器代码存放文件夹 */
    controllerDictionary: string;
    /** 作者 */
    authorName: string;
    /** 排序 */
    sortCode: number;
    /** 功能列表 */
    funcList: string[];
  }

  /** 代码生成配置 */
  export interface GenConfig {
    /** ID */
    id: number | string;
    /** 基础配置ID */
    basicId: number;
    /** 字段排序 */
    fieldIndex: number;
    /** 是否主键 */
    isPrimaryKey: string;
    /** 字段 */
    fieldName: string;
    /** 名称 */
    fieldRemark: string;
    /** 类型 */
    fieldType: string;
    /** 实体类型 */
    fieldNetType: string;
    /** 作用类型 */
    effectType: string;
    /** 外键实体名称 */
    fkEntityName: string;
    /** 外键ID */
    fkColumnId: string;
    /** 外键显示字段 */
    fkColumnName: string;
    /** 字典 */
    dictTypeCode: string;
    /** 列宽度 */
    width: number;
    /** 列表显示 */
    whetherTable: string;
    /** 列省略 */
    whetherRetract: string;
    /** 可伸缩列 */
    whetherResizable: string;
    /** 是否增改 */
    whetherAddUpdate: string;

    /** 是否导入导出 */
    whetherImportExport: string;
    /** 必填 */
    whetherRequired: string;
    /** 查询 */
    queryWhether: string;
    /** 查询方式 */
    queryType: string;
    /** 排序 */
    sortCode: number;
  }

  // 表属性接口
  export interface TableList {
    label: string;
    value: string;
    tableName: string;
    configId: string;
    entityName: string;
    tableDescription: string;
    tableColumns: any[];
  }

  /** 分页查询 */
  export interface Page extends ReqPage {}

  /** 代码生成基础 */
  export interface ExecGen extends ReqId {
    /** 生成类型 */
    execType: string;
  }

  /** SqlSugar字段信息 */
  export interface SqlSugarColumnInfo {
    /** 字段名 */
    columnName: string;
    /** 字段类型 */
    dataType: string;
    /** 是否主键 */
    isPrimaryKey: boolean;
    /** 是否可空 */
    isNullable: boolean;
    /** 字段说明 */
    columnDescription: string;
  }

  /** SqlSugar表信息 */
  export interface ResSqlSugarTableInfo {
    /** 所属库 */
    configId: string;
    /** 表名 */
    tableName: string;
    /** 表注释 */
    entityName: string;
    /** 主键 */
    tableDescription: string;
    /** 表字段 */
    columns: SqlSugarColumnInfo[];
  }

  /** 代码生成结果 */
  export interface GenBaseCodeResult {
    /** 代码文件名 */
    codeFileName: string;
    /** 代码文件路径 */
    filePath: string;
    /** 代码文件内容 */
    codeFileContent: string;
  }

  /** 代码生成预览 */
  export interface ResGenBasePreview {
    /** SQL代码结果集 */
    sqlResults: GenBaseCodeResult[];
    /** 前端代码结果集 */
    codeFrontendResults: GenBaseCodeResult[];
    /** 后端代码结果集 */
    codeBackendResults: GenBaseCodeResult[];
  }
}
