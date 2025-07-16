/**
 * @description 字典模块
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
import type { SysDict } from '@/api'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { sysDictApi } from '@/api'
import { CommonStatusEnum } from '@/enum'
import modal from '@/utils/modal'

/** 字典项值类型约束（一般是字符串） */
type dictValue = string

/** 字典模块的状态接口定义，用于类型提示 */
export interface DictState {
  dictInfo: SysDict.DictTree[] // 所有字典树信息
}

/** 使用 Pinia 定义组合式字典 store */
export const useDictStore = defineStore('simple-dict', () => {
  /**
   * 字典信息（响应式数组）
   * 结构为多个树形结构，每个字典项包含自身值和子项
   */
  const dictInfo = ref<SysDict.DictTree[]>([])

  /**
   * 请求接口获取完整字典树，并存入状态中
   * 如果接口返回失败，则弹出错误提示
   */
  const setDictTree = async () => {
    const { data } = await sysDictApi.tree()
    if (data) {
      dictInfo.value = data
    }
    else {
      modal.msg('获取系统字典信息失败，请联系系统管理员')
    }
  }

  /**
   * 字典翻译函数：根据传入的字典类型和值，返回对应中文标签
   * @param dictVal 字典类型（如 "sex", "status"）
   * @param value 字典值（如 "1", "true"）
   * @returns 字典对应的中文标签，若不存在返回“无此字典”
   */
  const dictTranslation = (dictVal: dictValue, value: string): string => {
    const tree = dictInfo.value.find(item => item.dictValue === dictVal)
    if (!tree)
      return '无此字典'
    const dict = tree.children.find(item => item.dictValue === value)
    return dict?.dictLabel || '无此字典'
  }

  /**
   * 获取某个字典下的子项列表，通常用于生成下拉框选项
   * @param dictVal 字典类型
   * @param isBool 是否转换为布尔值（用于处理 true/false 的场景）
   * @returns 字典子项数组，每项包含 label 和 value
   */
  const getDictList = (dictVal: dictValue, isBool = false) => {
    const tree = dictInfo.value.find(item => item.dictValue === dictVal)
    if (!tree)
      return []

    // 过滤掉禁用的字典项，只保留启用状态
    const filteredChildren = tree.children.filter(
      item => item.status === CommonStatusEnum.ENABLE,
    )

    // 构造 label + value 结构
    return filteredChildren.map((item) => {
      if (isBool) {
        return {
          value: item.dictValue === 'true',
          label: item.dictLabel,
        }
      }
      else {
        return {
          value: item.dictValue,
          label: item.dictLabel,
        }
      }
    })
  }

  /**
   * 获取所有一级字典项（不含子项），常用于生成某些选择器
   * @returns 所有字典项的 label + value 数组
   */
  const getDictAll = () => {
    return dictInfo.value.map(item => ({
      label: item.dictLabel,
      value: item.dictValue,
    }))
  }

  /**
   * getter：获取字典信息
   * 通常用于组件中通过 computed 获取字典数据
   */
  const dictInfoGet = computed(() => dictInfo.value)

  // 返回 store 实例提供的所有响应式属性和方法
  return {
    dictInfo,
    setDictTree,
    dictTranslation,
    getDictList,
    getDictAll,
    dictInfoGet,
  }
},
// 开启持久化存储，使用 uni-app 的本地缓存方式
{
  persist: true,
})
