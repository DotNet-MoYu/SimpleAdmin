import { Upload } from "@/api/interface";
import { moduleRequest } from "@/api/request";

const http = moduleRequest("/sys/file/");

/**
 * @name 文件上传模块
 */
// 图片上传
export const uploadImg = (params: FormData) => {
  return http.post<Upload.ResFileUrl>("upload/img", params);
};

// 视频上传
export const uploadVideo = (params: FormData) => {
  return http.post<Upload.ResFileUrl>("upload/video", params);
};
