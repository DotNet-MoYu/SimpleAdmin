/**
 * @description 鏂囦欢鏃ュ織鎺ュ彛
 */
import { ReqPage } from "@/api";
export namespace BizDocumentLog {
  export type LogType = 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10;
  export type DateRange = [string, string];
  export interface Page extends ReqPage {
    name?: string;
    userName?: string;
    type?: LogType;
    startTime?: string;
    endTime?: string;
  }
  export interface LogInfo {
    id: number;
    documentId?: number;
    name: string;
    detail: string;
    type: LogType;
    userId: number;
    userName: string;
    doTime: string;
  }
}
