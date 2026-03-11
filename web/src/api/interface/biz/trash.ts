/**
 * @description 鏂囦欢鍥炴敹绔欐帴鍙? */
import { ReqPage } from "@/api";
import { BizDocument } from "./document";
export namespace BizTrash {
  export interface Page extends ReqPage {
    name?: string;
    fileType?: BizDocument.FileType | "";
    label?: number | string;
    nodeType?: BizDocument.NodeType;
    suffix?: string;
    createTimeStart?: string;
    createTimeEnd?: string;
    updateTimeStart?: string;
    updateTimeEnd?: string;
  }
  export type TrashInfo = BizDocument.DocumentInfo;
}
