/**
 * @description sm2加密
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
import { sm2, sm4 } from 'sm-crypto'

const cipherMode = 0 // 1 - C1C3C2，0 - C1C2C3，默认为1
const publicKey
    = '04BD62406DF6789B1FBE8C457AECAE6D7C806CDB39316F190519905C24DF395E8952C47798D76ADECF8CA28C935702AFCDD9B17DE77121FA6448F0EDEFBD8365D6'
const key = '0123456789abcdeffedcba9876543210'

/**
 * 国密加解密工具类
 */
export default {
  // SM2加密
  doSm2Encrypt(msgString: string) {
    return sm2.doEncrypt(msgString, publicKey, cipherMode)
  },
  // SM4 加密
  doSm4Encrypt(msgString: string) {
    return sm4.encrypt(msgString, key)
  },
  // SM4 CBC加密
  doSm4CbcEncrypt(msgString: string) {
    return sm4.encrypt(msgString, key, {
      mode: 'cbc',
      iv: 'fedcba98765432100123456789abcdef',
    })
  },
  // SM4 解密
  doSm4Decrypt(encryptData: string) {
    return sm4.decrypt(encryptData, key)
  },
  // SM4 CBC解密
  doSm4CbcDecrypt(encryptData: string) {
    return sm4.decrypt(encryptData, key, {
      mode: 'cbc',
      iv: 'fedcba98765432100123456789abcdef',
    })
  },
}
