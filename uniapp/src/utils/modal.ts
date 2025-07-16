/**
 * @description 消息提示
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

export default {
  // 消息提示
  msg(content: any) {
    uni.showToast({
      title: content,
      icon: 'none',
    })
  },
  // 错误消息
  msgError(content: string) {
    uni.showToast({
      title: content,
      icon: 'error',
    })
  },
  // 错误消息
  msgException(content: string) {
    uni.showToast({
      title: content,
      icon: 'exception',
    })
  },
  // 成功消息
  msgSuccess(content: any) {
    uni.showToast({
      title: content,
      icon: 'success',
    })
  },
  // 隐藏消息
  hideMsg() {
    uni.hideToast()
  },
  // 弹出提示
  alert(content: any, title: any) {
    uni.showModal({
      title: title || '提示',
      content,
      showCancel: false,
    })
  },
  // 确认窗体
  confirm(content: any): Promise<any> {
    return new Promise((resolve, reject) => {
      uni.showModal({
        title: '提示',
        content,
        cancelText: '取消',
        confirmText: '确定',
        success(res) {
          resolve(res)
        },
      })
    })
  },
  // 提示信息
  showToast(option: any) {
    if (typeof option === 'object') {
      uni.showToast(option)
    }
    else {
      uni.showToast({
        title: option,
        icon: 'none',
        duration: 2500,
      })
    }
  },
  // 打开遮罩层
  loading(content: any) {
    uni.showLoading({
      title: content,
      icon: 'none',
    })
  },
  // 打开遮罩层
  showLoading() {
    uni.showLoading()
  },
  // 关闭遮罩层
  hideLoading() {
    uni.hideLoading()
  },
}
