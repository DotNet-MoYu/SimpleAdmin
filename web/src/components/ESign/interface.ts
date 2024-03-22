/**
 * @description 电子签名组件接口
 * @license Apache License Version 2.0
2022
,
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
import ESign from "./index.vue";

/** 电子签名组件属性接口 */
export interface ESignProps {
  /** 画布宽度，即导出图片的宽度 */
  width: number;
  /** 画布高度，即导出图片的高度 */
  height: number;
  /** 画笔粗细 */
  lineWidth: number;
  /** 画笔颜色 */
  lineColor: string;
  /** 背景颜色画布背景色，为空时画布背景透明，支持多种格式 '#ccc'，'#E5A1A1'，'rgb(229, 161, 161)'，'rgba(0,0,0,.6)'，'red' */
  bgColor: string;
  /** 是否裁剪，在画布设定尺寸基础上裁掉四周空白部分 */
  isCrop: boolean;
  /** 清空画布时(reset)是否同时清空设置的背景色(bgColor) */
  isClearBgColor?: boolean;
  /** 生成图片格式 image/jpeg(jpg格式下生成的图片透明背景会变黑色请慎用或指定背景色)、 image/webp */
  format?: string;
  /** 生成图片质量；在指定图片格式为 image/jpeg 或 image/webp的情况下，可以从 0 到 1 的区间内选择图片的质量。如果超出取值范围，将会使用默认值 0.92。其他参数会被忽略。 */
  quality?: number;
  /** 前面图片 */
  image: string;
}

/** 电子签名组件点位坐标接口 */
export interface Points {
  x: number;
  y: number;
}

/**
 * @description 角色选择器实例类型
 */
export type ESignInstance = Omit<InstanceType<typeof ESign>, keyof ComponentPublicInstance | keyof ESignProps>;
