/**
 * @description 接口
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
import CropUpload from "./index.vue";

/**
 * 裁剪上传组件的属性
 */
export interface CropUploadOptions {
  /** 预览地址 */
  previewUrl: string;
  /** 文件名 */
  fileName: string;
}

/** 裁剪图片的格式 */
export enum OutputType {
  jpeg = "jpeg",
  png = "png",
  webp = "webp"
}

/** VueCropper属性 */
export interface VueCropperProps {
  /** 裁剪图片的地址,url 地址, base64, blob */
  img: string;
  /** 裁剪生成图片的质量 */
  outputSize: number;
  /** 是否输boolean出原图比例的截图 */
  full: false;
  /** 裁剪生成图片的格式 */
  outputType: OutputType;
  /** 是否可以移动图片 */
  canMove: boolean;
  /** 固定截图框大小 */
  fixedBox: boolean;
  /** 上传图片按照原始比例渲染 */
  original: boolean;
  /** 截图框能否拖动 */
  canMoveBox: boolean;
  /** 是否默认生成截图框 */
  autoCrop: boolean;
  /** 默认生成截图框宽度 */
  autoCropWidth: number;
  /** 默认生成截图框高度 */
  autoCropHeight: number;
  /** 截图框是否被限制在图片里面 */
  centerBox: boolean;
  /** 是否按照设备的dpr 输出等比例图片 */
  high: boolean;
  /** 截图数据 */
  cropData: object;
  /** 图片根据截图框输出比例倍数 */
  enlarge: number;
  /** 图片默认渲染方式 */
  mode: "contain";
  /** 限制图片最大宽度和高度 */
  maxImgSize: number;
  /** 	true 为展示真实输出图片宽高 false 展示看到的截图框宽高 */
  infoTrue: boolean;
  /** limitMinSize */
  limitMinSize: number[];
}

/**
 * @description 角色选择器实例类型
 */
export type CropUploadInstance = Omit<InstanceType<typeof CropUpload>, keyof ComponentPublicInstance | keyof CropUploadOptions>;
