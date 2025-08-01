<!-- 
 * @Description: 上传文件组件
 * @Author: huguodong 
 * @Date: 2025-07-31 15:23:54
!-->
<template>
  <el-upload
    class="upload-file"
    :action="uploadUrl"
    :show-file-list="false"
    :before-upload="beforeUpload"
    :on-success="handleSuccess"
    :on-error="handleError"
    :on-start="handleStart"
    :headers="headers"
    :accept="accept"
  >
    <el-button type="primary" :loading="loading">
      <el-icon class="mr-1"><Upload /></el-icon>
      {{ buttonText }}
    </el-button>
  </el-upload>
</template>

<script setup lang="ts">
import { Upload } from "@element-plus/icons-vue";
import { ElMessage } from "element-plus";
import { useUserStore } from "@/stores/modules";

const userStore = useUserStore();
const props = defineProps<{
  uploadUrl: string; // 上传接口地址
  buttonText?: string; // 按钮文字
  accept?: string; // 可选文件类型
}>();

const emit = defineEmits<{
  (e: "success", response: any): void;
}>();

const loading = ref(false);

// 按钮显示文字
const buttonText = computed(() => props.buttonText || "上传文件");

// 接收文件类型（支持 Excel 和 PDF）
const accept = computed(() => props.accept || ".xls,.xlsx,.pdf");

// 请求头（如需携带 token）
const headers = {
  Authorization: `Bearer ${userStore.accessToken}`
};

// 上传开始时触发
const handleStart = () => {
  loading.value = true;
};

// 上传成功时触发
const handleSuccess = (response: any) => {
  loading.value = false;
  console.log("[ response ] >", response);
  if (response.code !== 200) {
    ElMessage.error(response.msg);
    return;
  }
  ElMessage.success("上传成功！");
  emit("success", response);
};

// 上传失败时触发
const handleError = () => {
  loading.value = false;
  ElMessage.error("上传失败，请重试！");
};

// 上传前校验文件类型和大小
const beforeUpload = (file: File) => {
  const isExcel =
    file.type === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
    file.type === "application/vnd.ms-excel" ||
    file.name.endsWith(".xls") ||
    file.name.endsWith(".xlsx");

  const isPDF = file.type === "application/pdf" || file.name.endsWith(".pdf");

  const isValid = isExcel || isPDF;
  const isLt5M = file.size / 1024 / 1024 < 5;

  if (!isValid) ElMessage.error("只能上传 Excel 或 PDF 文件！");
  if (!isLt5M) ElMessage.error("文件大小不能超过 5MB！");
  return isValid && isLt5M;
};
</script>

<style scoped>
.upload-file {
  display: inline-block;
}
</style>
