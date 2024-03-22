<!-- 
 * @Description: 折线图
 * @Author: huguodong 
 * @Date: 2023-12-15 15:44:42
!-->
<template>
  <div id="lineChat" class="h-150px"></div>
</template>

<script setup lang="ts" name="lineChart">
import { VisLog, visLogApi } from "@/api";
import { Line } from "@antv/g2plot";

const seriesField = "series"; //分组字段
const yField = "value"; //图形在 y 方向对应的数据字段名，一般是纵向的坐标轴对应的字段

type alias = "用户登录" | "用户登出"; //别名

// lineMeta 用于处理数据，将数据中的字段名转换为别名
interface LineMeta {
  [key: string]: {
    alias: alias;
  };
}
const lineMeta: LineMeta = {
  loginCount: {
    alias: "用户登录"
  },
  logoutCount: {
    alias: "用户登出"
  }
};

onMounted(() => {
  visLogApi.lineChart().then(res => {
    createLineChat(res.data);
  });
});

/**
 * 创建折线图,具体文档参考https://g2plot.antv.antgroup.com/api/plots/line
 * @param data 数据
 */
function createLineChat(data: VisLog.LineChart[]) {
  const line = new Line("lineChat", {
    data: processData(data, lineMeta),
    padding: "auto",
    xField: "date",
    yField: yField,
    seriesField: seriesField,
    color: ["#409EFF", "rgb(188, 189, 190)"],
    appendPadding: [0, 8, 0, 0]
  });
  line.render(); //渲染
}

/**
 * 处理数据，将数据中的字段名转换为别名
 * @param data 数据
 * @param yFields y轴字段
 * @param meta 别名
 */
function processData(data: VisLog.LineChart[], meta: LineMeta) {
  const result: any = [];
  //将数据中的字段名转换为别名
  data.forEach((item: VisLog.LineChart) => {
    result.push({ ...item, [seriesField]: meta.loginCount?.alias, [yField]: item.loginCount });
    result.push({ ...item, [seriesField]: meta.logoutCount?.alias, [yField]: item.logoutCount });
  });
  return result;
}
</script>

<style scoped lang="scss"></style>
