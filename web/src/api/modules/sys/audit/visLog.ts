/**
 * @description 访问日志
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

import { ResPage, VisLog } from "@/api/interface";
import { moduleRequest } from "@/api/request";
const http = moduleRequest("/sysaudit/logVisit/");

/**
 * @Description: 访问日志
 * @Author: huguodong
 * @Date: 2023-12-15 15:34:54
 */
const visLogApi = {
  /** 获取访问日志分页 */
  page(params: VisLog.Page) {
    return http.get<ResPage<VisLog.VisLogInfo>>("page", params);
  },
  /** 获取访问日志折线图数据 */
  lineChart() {
    return http.get<VisLog.LineChart[]>("lineChartData", {}, { loading: false });
  },
  /** 获取访问日志饼状图数据 */
  pieChart() {
    return http.get<VisLog.PineChart[]>("pieChartData", {}, { loading: false });
  },
  /** 清空访问日志 */
  delete(category: string) {
    return http.post("delete", { category });
  }
};

export { visLogApi };
