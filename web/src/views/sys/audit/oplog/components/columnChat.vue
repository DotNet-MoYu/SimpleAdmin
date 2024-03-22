<!-- 
 * @Description: 柱状图
 * @Author: huguodong 
 * @Date: 2023-12-15 15:44:12
!-->
<template>
  <div id="columnChat" class="h-150px"></div>
</template>

<script setup lang="ts" name="columnChart">
import { OpLog, opLogApi } from "@/api";
import { Column } from "@antv/g2plot";

onMounted(() => {
  opLogApi.columnChart().then(res => {
    createColumnChat(res.data);
  });
});

/**
 * 创建折线图,具体文档参考https://g2plot.antv.antgroup.com/api/plots/column
 * @param data 数据
 */
function createColumnChat(data: OpLog.ColumnChart[]) {
  const column = new Column("columnChat", {
    data,
    isGroup: true,
    xField: "date",
    yField: "count",
    seriesField: "name",
    /** 设置颜色 */
    color: ["#409EFF", "#F5222D"],
    /** 设置间距 */
    // marginRatio: 0.1,
    label: {
      // 可手动配置 label 数据标签位置
      position: "middle", // 'top', 'middle', 'bottom'
      // 可配置附加的布局方法
      layout: [
        // 柱形图数据标签位置自动调整
        { type: "interval-adjust-position" },
        // 数据标签防遮挡
        { type: "interval-hide-overlap" },
        // 数据标签文颜色自动调整
        { type: "adjust-color" }
      ]
    }
  });
  column.render(); //渲染
}
</script>

<style scoped lang="scss"></style>
