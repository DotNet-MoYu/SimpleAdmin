// @ts-nocheck
export type NullableString = string | null
export type ConversionType = 'toBase64' | 'toDataURL' | 'toFile'
export type ProcessFileOptions = {
  type : ConversionType
  path: string
  filename?: string
  success ?: (res : string) => void
  fail ?: (res : any) => void
  complete ?: (res : any) => void
}



/**
 * 错误码
 * 根据uni错误码规范要求，建议错误码以90开头，以下是错误码示例：
 * - 9010001 错误信息1
 * - 9010002 错误信息2
 */
export type ProcessFileErrorCode = 9010001 | 9010002;
/**
 * myApi 的错误回调参数
 */
export interface ProcessFileFail extends IUniError {
  errCode : ProcessFileErrorCode
};
