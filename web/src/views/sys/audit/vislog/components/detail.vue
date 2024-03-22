<!-- 
 * @Description: 详情
 * @Author: huguodong 
 * @Date: 2023-12-15 15:44:38
!-->
<template>
  <form-container v-model="visible" title="日志详情" form-size="600px">
    <el-descriptions :column="1" border class="mb-2">
      <el-descriptions-item label="名称">{{ logInfo.name }}</el-descriptions-item>
      <el-descriptions-item label="IP地址">{{ logInfo.opIp }}</el-descriptions-item>
      <el-descriptions-item label="地址">{{ logInfo.opAddress }}</el-descriptions-item>
      <el-descriptions-item label="浏览器">{{ logInfo.opBrowser }}</el-descriptions-item>
      <el-descriptions-item label="设备">{{ logInfo.opOs }}</el-descriptions-item>
      <el-descriptions-item label="时间">{{ logInfo.opTime }}</el-descriptions-item>
      <el-descriptions-item label="用户">{{ logInfo.opUser }}</el-descriptions-item>
      <el-descriptions-item label="账号">{{ logInfo.opAccount }}</el-descriptions-item>
    </el-descriptions>
    <template #footer>
      <el-button type="primary" @click="onClose"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts" name="detail">
import { VisLog } from "@/api";

const visible = ref(false); //是否显示表单

// 表单参数
const logInfo = ref<VisLog.VisLogInfo>({
  name: "",
  opIp: "",
  opAddress: "",
  opBrowser: "",
  opOs: "",
  opTime: "",
  opUser: "",
  opAccount: "",
  id: 0,
  category: "",
  exeStatus: "",
  createTime: ""
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(record: VisLog.VisLogInfo) {
  logInfo.value = record;
  visible.value = true; //显示表单
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
