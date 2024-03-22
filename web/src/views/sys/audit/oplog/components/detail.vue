<!-- 
 * @Description: 详情
 * @Author: huguodong 
 * @Date: 2023-12-15 15:44:16
!-->
<template>
  <form-container v-model="visible" title="日志详情" form-size="650px">
    <el-descriptions :column="1" border class="mb-2">
      <el-descriptions-item label="名称">{{ logInfo.name }}</el-descriptions-item>
      <el-descriptions-item label="IP地址">{{ logInfo.opIp }}</el-descriptions-item>
      <el-descriptions-item label="地址">{{ logInfo.opAddress }}</el-descriptions-item>
      <el-descriptions-item label="请求地址">{{ logInfo.reqUrl }}</el-descriptions-item>
      <el-descriptions-item label="操作类">{{ logInfo.className }}</el-descriptions-item>
      <el-descriptions-item label="操作方法">{{ logInfo.methodName }}</el-descriptions-item>
      <el-descriptions-item label="浏览器">{{ logInfo.opBrowser }}</el-descriptions-item>
      <el-descriptions-item label="设备">{{ logInfo.opOs }}</el-descriptions-item>
      <el-descriptions-item label="时间">{{ logInfo.opTime }}</el-descriptions-item>
      <el-descriptions-item label="用户">{{ logInfo.opUser }}</el-descriptions-item>
      <el-descriptions-item label="账号">{{ logInfo.opAccount }}</el-descriptions-item>
    </el-descriptions>
    <el-space direction="vertical" class="mb-2 w-full">
      <span>请求参数:</span>
      <code-high-light language="JSON" class="w-600px" :code="paramJson" />
    </el-space>
    <el-space direction="vertical" class="mb-2" style="width: 100%">
      返回结果：
      <code-high-light autodetect class="w-600px" :code="resultJson"></code-high-light>
    </el-space>
    <template #footer>
      <el-button type="primary" @click="onClose"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts" name="detail">
import { OpLog, opLogApi } from "@/api";

const visible = ref(false); //是否显示表单

// 表单参数
const logInfo = ref<OpLog.OpLogInfo>({
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
  createTime: "",
  exeMessage: null,
  className: "",
  methodName: "",
  reqMethod: "",
  reqUrl: "",
  paramJson: "",
  resultJson: ""
});

const paramJson = ref<string>();
const resultJson = ref<string>();

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(record: OpLog.OpLogInfo) {
  logInfo.value = record;
  visible.value = true; //显示表单
  opLogApi.detail({ id: record.id }).then(res => {
    logInfo.value = res.data;
    if (res.data.paramJson) paramJson.value = JSON.stringify(JSON.parse(res.data.paramJson), null, 2);
    if (res.data.exeMessage) {
      resultJson.value = res.data.exeMessage;
    } else {
      if (res.data.resultJson) resultJson.value = JSON.stringify(JSON.parse(res.data.resultJson), null, 2);
    }
  });
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

<style lang="scss" scoped>
:deep(.el-space__item) {
  width: 100% !important;
}
</style>
