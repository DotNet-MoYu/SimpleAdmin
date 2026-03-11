/**
 * @description 鏂囦欢绠＄悊鎺ュ彛
 */
import { ReqId, ReqPage, SysRole, SysUser } from "@/api";
export namespace BizDocument {
  export type NodeType = 1 | 2;
  export type FileType = "文件夹" | "文档" | "图片" | "压缩包" | "应用程序" | "文件";
  export interface Page extends ReqPage {
    parentId?: number;
    name?: string;
    fileType?: FileType | "";
    label?: number | string;
    nodeType?: NodeType;
    suffix?: string;
    createTimeStart?: string;
    createTimeEnd?: string;
    updateTimeStart?: string;
    updateTimeEnd?: string;
  }
  export interface TreeNode {
    id: number;
    parentId: number;
    rootId: number;
    name: string;
    isRoot: boolean;
    children?: TreeNode[];
  }
  export interface DocumentInfo {
    id: number;
    parentId: number;
    rootId: number;
    name: string;
    nodeType: NodeType;
    fileId?: number;
    engine?: string;
    sizeKb?: number;
    sizeInfo?: string;
    suffix?: string;
    label?: number | string;
    remark?: string;
    isDeleted: boolean;
    isRoot: boolean;
    canRename: boolean;
    canMove: boolean;
    canDelete: boolean;
    canGrant: boolean;
    canUpload: boolean;
    fileTypeLabel?: string;
    thumbnail?: string;
    createTime?: string;
    createUserName?: string;
    updateTime?: string;
    updateUserName?: string;
  }
  export interface AddFolderReq {
    parentId: number;
    name: string;
    label?: number | string;
    remark?: string;
  }
  export interface RenameReq extends ReqId {
    name: string;
    label?: number | string;
    remark?: string;
  }
  export interface MoveReq {
    ids: number[];
    targetParentId: number;
  }
  export interface UploadReq {
    parentId: number;
    engine?: string;
    files: File[];
    relativePaths?: string[];
  }
  export interface PreviewInfo {
    previewType: "image" | "text" | "none";
    contentType: string;
    content: string;
    fileName: string;
  }
  export interface GrantDetail {
    id: number;
    users: SysUser.SysUserInfo[];
    roles: SysRole.SysRoleInfo[];
  }
  export interface GrantUsersReq extends ReqId {
    userIds: number[];
  }
  export interface GrantRolesReq extends ReqId {
    roleIds: number[];
  }
}
