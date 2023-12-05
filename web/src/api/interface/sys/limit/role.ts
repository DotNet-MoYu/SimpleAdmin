import { ReqId } from "@/api/interface";
/**
 * @description 角色管理
 * @license Apache License Version 2.0
 * @remarks
 * SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
 * 1.请不要删除和修改根目录下的LICENSE文件。
 * 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
 * 3.分发源码时候，请注明软件出处 https://gitee.com/zxzyjs/SimpleAdmin
 * 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
 * 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
 * 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关
 * @see https://gitee.com/zxzyjs/SimpleAdmin
 */
import { ReqPage } from "@/api/interface";
export namespace SysRole {
  /** 角色分页查询 */
  export interface Page extends ReqPage {}

  /** 角色信息 */
  export interface SysRoleInfo {
    /** 角色id */
    id: number | string;
    /** 机构ID */
    orgId: number | string;
    /** 角色名称 */
    name: string;
    /** 角色编码 */
    code: string;
    /** 角色分类 */
    category: string;
    /** 默认数据范围 */
    defaultDataScope: DefaultDataScope;
    /** 状态 */
    status: string;
    /** 排序码 */
    sortCode: number;
  }

  /** 默认数据范围 */
  export interface DefaultDataScope {
    /** 权重 */
    level: number;
    /** 标题 */
    title: string;
    /** 范围分类 */
    scopeCategory: DataScopeEnum;
    /** 机构ID列表 */
    scopeDefineOrgIdList: string[] | number[];
  }

  /** 角色树 */
  export interface SysRoleTree {
    /** id */
    id: number | string;
    /** 名称 */
    name: string;
    /** 是否是角色 */
    isRole: boolean;
    /** 子集 */
    children: SysRoleTree[];
  }

  /** 数据范围枚举 */
  export enum DataScopeEnum {
    /** 全部 */
    SCOPE_ALL = "SCOPE_ALL",
    /** 所属组织及以下 */
    SCOPE_ORG_CHILD = "SCOPE_ORG_CHILD",
    /** 所属组织 */
    SCOPE_ORG = "SCOPE_ORG",
    /** 仅自己 */
    SCOPE_SELF = "SCOPE_SELF",
    /** 自定义 */
    SCOPE_ORG_DEFINE = "SCOPE_ORG_DEFINE"
  }

  /** 数据范围数组 */
  export const dataScopeOptions: DefaultDataScope[] = [
    {
      level: 5,
      title: "全部",
      scopeCategory: DataScopeEnum.SCOPE_ALL,
      scopeDefineOrgIdList: []
    },
    {
      level: 4,
      title: "所属组织及以下",
      scopeCategory: DataScopeEnum.SCOPE_ORG_CHILD,
      scopeDefineOrgIdList: []
    },
    {
      level: 2,
      title: "所属组织",
      scopeCategory: DataScopeEnum.SCOPE_ORG,
      scopeDefineOrgIdList: []
    },
    {
      level: 1,
      title: "仅自己",
      scopeCategory: DataScopeEnum.SCOPE_SELF,
      scopeDefineOrgIdList: []
    },
    {
      level: 3,
      title: "自定义",
      scopeCategory: DataScopeEnum.SCOPE_ORG_DEFINE,
      scopeDefineOrgIdList: []
    }
  ];

  /** 角色拥有资源 */
  export interface RoleOwnResource {
    /** id */
    id: number | string;
    /** 已授权资源信息 */
    grantInfoList: RelationRoleResource[];
  }

  /** 角色有哪些资源 */
  export interface RelationRoleResource {
    /** 菜单id */
    menuId: number | string;
    /** 按钮信息 */
    buttonInfo: number[] | string[];
  }

  /** 角色权限关系扩展 */
  export interface RelationRolePermission {
    /** 数据范围 */
    scopeCategory: string;
    /** 自定义机构范围列表 */
    scopeDefineOrgIdList: string[] | number[];
    /** 接口Url */
    apiUrl: string;
  }

  /** 角色拥有权限 */
  export interface RoleOwnPermission extends RoleOwnResource {}

  /** 角色授权资源请求参数 */
  export interface GrantResourceReq extends ReqId {
    /** 授权资源信息 */
    grantInfoList: RelationRoleResource[];
  }
}
