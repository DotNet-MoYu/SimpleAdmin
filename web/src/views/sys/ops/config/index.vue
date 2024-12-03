<!-- 
 * @Description: 
 * @Author: huguodong 
 * @Date: 2023-12-22 16:59:28
!-->
<template>
  <div class="card">
    <el-tabs v-model="activeName" class="-mt-4">
      <el-tab-pane label="系统配置" name="sysConfig"><SysBaseConfig :sys-configs="sysBase" /></el-tab-pane>
      <el-tab-pane label="安全配置" name="safety"><SafetyConfig :sys-configs="safetyConfig" /></el-tab-pane>
      <el-tab-pane label="MQTT配置" name="mqtt"><MqttConfig :sys-configs="mqttConfig" /></el-tab-pane>
      <el-tab-pane label="文件配置" name="file">开发中......</el-tab-pane>
      <el-tab-pane label="其他配置" name="other"><OtherConfig v-if="activeName === 'other'" /></el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import SysBaseConfig from "./components/sysBaseConfig.vue";
import SafetyConfig from "./components/safetyConfig/index.vue";
import OtherConfig from "./components/otherConfig/index.vue";
import MqttConfig from "./components/mqttConfig/index.vue";
import { SysConfig, sysConfigApi } from "@/api";
import { SysConfigTypeEnum } from "@/enums";

const activeName = ref("sysConfig");

const sysBase = ref<SysConfig.ConfigInfo[]>([]);
const safetyConfig = ref<SysConfig.ConfigInfo[]>([]);
const mqttConfig = ref<SysConfig.ConfigInfo[]>([]);
onMounted(async () => {
  // 获取系统配置
  await sysConfigApi.list().then(res => {
    if (res.data) {
      //sysBase赋值
      sysBase.value = res.data.filter(item => item.category == SysConfigTypeEnum.SYS_BASE);
      //safetyBase赋值
      safetyConfig.value = res.data.filter(item => item.category == SysConfigTypeEnum.LOGIN_POLICY || item.category == SysConfigTypeEnum.PWD_POLICY);
      //mqttConfig赋值
      mqttConfig.value = res.data.filter(item => item.category == SysConfigTypeEnum.MQTT_POLICY);
    }
  });
});
</script>

<style lang="scss" scoped>
:deep(.el-tabs__item) {
  font-size: large;
}
</style>
