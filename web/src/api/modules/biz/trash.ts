/**
 * @description 文件回收站
 */
import { BizTrash, ResPage } from "@/api";
import { moduleRequest } from "@/api/request";

const http = moduleRequest("/biz/document/trash/");

const trashApi = {
  page(params: BizTrash.Page) {
    return http.get<ResPage<BizTrash.TrashInfo>>("page", params);
  },
  recover(params: { ids: number[] }) {
    return http.post("recover", params);
  },
  batchRecover(params: { ids: number[] }) {
    return http.post("batchRecover", params);
  },
  deletePermanent(params: { ids: number[] }) {
    return http.post("deletePermanent", params);
  },
  batchDeletePermanent(params: { ids: number[] }) {
    return http.post("batchDeletePermanent", params);
  },
  empty() {
    return http.post("empty", {});
  }
};

const trashButtonCode = {
  recover: "bizDocumentTrashRecover",
  deletePermanent: "bizDocumentTrashDeletePermanent",
  empty: "bizDocumentTrashEmpty"
};

export { trashApi, trashButtonCode };
