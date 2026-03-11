/**
 * @description 文件日志
 */
import { BizDocumentLog, ResPage } from "@/api";
import { moduleRequest } from "@/api/request";

const http = moduleRequest("/biz/document/log/");

const documentLogApi = {
  page(params: BizDocumentLog.Page) {
    return http.get<ResPage<BizDocumentLog.LogInfo>>("page", params);
  },
  empty() {
    return http.post("empty", {});
  }
};

const documentLogButtonCode = {
  empty: "bizDocumentLogEmpty"
};

export { documentLogApi, documentLogButtonCode };
