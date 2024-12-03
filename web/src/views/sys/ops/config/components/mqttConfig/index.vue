<!-- 
 * @Description: 安全配置
 * @Author: huguodong 
 * @Date: 2024-01-31 15:33:50
!-->
<template>
  <el-tabs v-model="activeName" class="demo-tabs" tab-position="left">
    <el-tab-pane label="连接配置" name="base" class="pl-5"><BaseForm :base-config="baseConfig" /></el-tab-pane>
  </el-tabs>
</template>

<script setup lang="ts">
import { SysConfig } from "@/api";
import BaseForm from "./baseForm.vue";
import { SysConfigTypeEnum } from "@/enums";

const activeName = ref("base");
const baseConfig = ref<SysConfig.ConfigInfo[]>([]);

//props定义
const props = defineProps({
  sysConfigs: {
    type: Array as PropType<SysConfig.ConfigInfo[]>,
    required: true,
    default: () => []
  }
});

//监听sysConfigs变化
watch(
  () => props.sysConfigs,
  (newVal: SysConfig.ConfigInfo[]) => {
    // 配置赋值
    baseConfig.value = newVal.filter(item => item.category == SysConfigTypeEnum.MQTT_POLICY);
  }
);
</script>

<style lang="scss" scoped>
.demo-tabs > .el-tabs__content {
  padding: 32px;
  font-size: 132px;
  font-weight: 600;
  color: #6b778c;
}
</style>
