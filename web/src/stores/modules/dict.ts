/**
 * @description  字典模块
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

import { defineStore } from "pinia";
import piniaPersistConfig from "@/stores/helper/persist";
import { Dict } from "@/api/interface";
import { getDictTreeApi } from "@/api";
import { ElNotification } from "element-plus";
import { SysDictEnum, CommonStatusEnum } from "@/enums";

/** 字典值约束 */
type dictValue = string | SysDictEnum;

/**  DictState */
export interface DictState {
  /** 字典信息 */
  dictInfo: Dict.DictTree[];
}

export const useDictStore = defineStore({
  id: "simple-dict",
  state: (): DictState => ({
    dictInfo: []
  }),
  getters: {
    dictInfoGet: state => state.dictInfo
  },
  actions: {
    /**  设置字典信息 */
    async setDictTree() {
      /**  获取字典信息 */
      const { data } = await getDictTreeApi();
      if (data) {
        this.dictInfo = data;
      } else {
        ElNotification({
          title: "系统错误",
          message: "获取系统字典信息失败，请联系系统管理员！",
          type: "warning",
          duration: 3000
        });
      }
    },
    /** 字典翻译 */
    dictTranslation(dictValue: dictValue, value: string) {
      const tree = this.dictInfo.find(item => item.dictValue === dictValue); // 通过字典值找到字典
      //如果没有找到字典，返回无此字典
      if (!tree) {
        return "无此字典";
      }
      // 通过传的值找到字典的子项
      const dict = tree.children.find(item => item.dictValue === value);
      return dict?.dictLabel || "无此字典";
    },
    /** 获取某个code下字典的列表，多用于字典下拉框 */
    getDictList(dictValue: dictValue) {
      const tree = this.dictInfo.find(item => item.dictValue === dictValue);
      if (tree) {
        //过滤停用的子项
        tree.children = tree.children.filter(item => item.status === CommonStatusEnum.ENABLE);
        return tree.children.map(item => {
          return {
            value: item["dictValue"],
            label: item["dictLabel"]
          };
        });
      }
      return [];
    }
  },
  persist: piniaPersistConfig("simple-dict")
});
