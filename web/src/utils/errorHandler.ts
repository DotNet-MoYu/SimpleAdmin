/**
 * @description 全局代码错误捕捉
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
 * @see https://gitee.com/dotnetmoyu/SimpleAdmin
 */
import { ElNotification } from "element-plus";

/**
 * @description 全局代码错误捕捉
 * */
const errorHandler = (error: any) => {
  // 过滤 HTTP 请求错误
  if (error.status || error.status == 0) return false;
  let errorMap: { [key: string]: string } = {
    InternalError: "Javascript引擎内部错误",
    ReferenceError: "未找到对象",
    TypeError: "使用了错误的类型或对象",
    RangeError: "使用内置对象时，参数超范围",
    SyntaxError: "语法错误",
    EvalError: "错误的使用了Eval",
    URIError: "URI错误"
  };
  let errorName = errorMap[error.name] || "未知错误";
  ElNotification({
    title: errorName,
    message: error,
    type: "error",
    duration: 3000
  });
  console.log("[ error ] >", error);
};

export default errorHandler;
