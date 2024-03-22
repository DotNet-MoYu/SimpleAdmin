<!-- 
 * @Description: 饼状图
 * @Author: huguodong 
 * @Date: 2023-12-15 15:44:47
!-->
<template>
  <div id="pieChat" class="h-150px"></div>
</template>

<script setup lang="ts" name="pieChart">
import { VisLog, visLogApi } from "@/api";
import { Pie } from "@antv/g2plot";

onMounted(() => {
  visLogApi.pieChart().then(res => {
    createPieChat(res.data);
  });
});

/**
 * 创建折线图,具体文档参考https://g2plot.antv.antgroup.com/api/plots/pie
 * @param data 数据
 */
function createPieChat(data: VisLog.PineChart[]) {
  const piePlot = new Pie("pieChat", {
    appendPadding: 10,
    data,
    angleField: "value",
    colorField: "type",
    radius: 0.9,
    color: ["#409EFF", "rgb(188, 189, 190)"],
    label: {
      type: "inner",
      offset: "-30%",
      content: ({ percent }) => `${(percent * 100).toFixed(0)}%`,
      style: {
        fontSize: 14,
        textAlign: "center"
      }
    },
    interactions: [{ type: "element-active" }]
  });
  piePlot.render(); //渲染
}
</script>

<style scoped lang="scss"></style>
