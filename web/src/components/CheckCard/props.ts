/**
 * @description  props
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
// 从Vue库中导入PropType类型
import type { PropType as VuePropType } from "vue";
// 从本地types文件中导入OptionsConfig类型

// 定义一个新类型PropType，它是VuePropType的别名
declare type PropType<T> = VuePropType<T>;

// 定义一个包含CheckModule组件props的对象
export const basicProps = {
  value: {
    type: Array as PropType<number[] | string[]> // value prop的类型是数字或字符串的数组
  },
  width: {
    type: [Number, String], // width prop的类型是数字或字符串
    default: 320 // width prop的默认值是320
  },
  multiple: {
    type: Boolean, // multiple prop的类型是布尔值
    default: false // multiple prop的默认值是false
  },
  hoverable: {
    type: Boolean, // hoverable prop的类型是布尔值
    default: false // hoverable prop的默认值是false
  },
  bordered: {
    type: Boolean, // bordered prop的类型是布尔值
    default: true // bordered prop的默认值是true
  },
  options: {
    type: Array as PropType<CheckCard.OptionsConfig[]>, // options prop的类型是OptionsConfig对象的数组
    default: () => [] // options prop的默认值是一个空数组
  }
};
