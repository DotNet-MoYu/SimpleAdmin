/**
 * @description 文件管理
 */
import { BizDocument, ReqId, ResPage } from "@/api";
import { moduleRequest } from "@/api/request";

const http = moduleRequest("/biz/document/");

const documentApi = {
  page(params: BizDocument.Page) {
    return http.get<ResPage<BizDocument.DocumentInfo>>("page", params);
  },
  tree() {
    return http.get<BizDocument.TreeNode[]>("tree", {}, { loading: false });
  },
  detail(params: ReqId) {
    return http.get<BizDocument.DocumentInfo>("detail", params);
  },
  addFolder(params: BizDocument.AddFolderReq) {
    return http.post<number>("addFolder", params);
  },
  rename(params: BizDocument.RenameReq) {
    return http.post("rename", params);
  },
  move(params: BizDocument.MoveReq) {
    return http.post("move", params);
  },
  delete(params: ReqId) {
    return http.post("delete", params);
  },
  batchDelete(params: { ids: number[] }) {
    return http.post("batchDelete", params);
  },
  uploadFiles(params: FormData) {
    return http.post("uploadFiles", params, {
      headers: { "Content-Type": "multipart/form-data" }
    });
  },
  uploadFolder(params: FormData) {
    return http.post("uploadFolder", params, {
      headers: { "Content-Type": "multipart/form-data" }
    });
  },
  uploadInit(params: BizDocument.ChunkUploadInitReq) {
    return http.post<BizDocument.ChunkUploadInitRes>("upload/init", params, { loading: false, cancel: false });
  },
  uploadChunk(params: FormData, config = {}) {
    return http.post("upload/chunk", params, {
      headers: { "Content-Type": "multipart/form-data" },
      loading: false,
      cancel: false,
      ...config
    });
  },
  uploadStatus(params: { uploadId: number }) {
    return http.get<BizDocument.ChunkUploadStatusRes>("upload/status", params, { loading: false, cancel: false });
  },
  uploadComplete(params: BizDocument.ChunkUploadCompleteReq) {
    return http.post<BizDocument.ChunkUploadCompleteRes>("upload/complete", params, { loading: false, cancel: false });
  },
  uploadCancel(params: { uploadId: number }) {
    return http.post("upload/cancel", params, { loading: false, cancel: false });
  },
  download(params: ReqId) {
    return http.get<any>("download", params, { responseType: "blob" });
  },
  preview(params: ReqId) {
    return http.get<BizDocument.PreviewInfo>("preview", params, { loading: false });
  },
  grantDetail(params: ReqId) {
    return http.get<BizDocument.GrantDetail>("grantDetail", params, { loading: false });
  },
  grantUsers(params: BizDocument.GrantUsersReq) {
    return http.post("grantUsers", params);
  },
  grantRoles(params: BizDocument.GrantRolesReq) {
    return http.post("grantRoles", params);
  }
};

const documentButtonCode = {
  addFolder: "bizDocumentAddFolder",
  uploadFile: "bizDocumentUploadFile",
  uploadFolder: "bizDocumentUploadFolder",
  delete: "bizDocumentDelete",
  batchDelete: "bizDocumentBatchDelete",
  move: "bizDocumentMove",
  rename: "bizDocumentRename",
  grant: "bizDocumentGrant"
};

export { documentApi, documentButtonCode };
