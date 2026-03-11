<template>
  <div class="table-header-actions">
    <el-button type="primary" @click="triggerFileUpload">上传文件</el-button>
    <s-button type="danger" plain suffix="文件" :opt="FormOptEnum.DELETE" :disabled="!isSelected" @click="emit('deleteSelected', selectedIds)" />
    <el-button :disabled="!isSelected" @click="emit('clearSelection')">清空选择</el-button>
  </div>

  <input ref="fileInputRef" type="file" hidden multiple @change="handleFileInputChange" />
</template>

<script setup lang="ts">
import { fileApi } from "@/api";
import { FormOptEnum } from "@/enums";
import { ElMessage } from "element-plus";

defineProps<{
  isSelected: boolean;
  selectedIds: Array<number | string>;
}>();

const emit = defineEmits<{
  deleteSelected: [ids: Array<number | string>];
  clearSelection: [];
  uploaded: [payload: { successCount: number; failedCount: number }];
}>();

const fileInputRef = ref<HTMLInputElement>();

function triggerFileUpload() {
  fileInputRef.value?.click();
}

async function handleFileInputChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const files = Array.from(input.files || []);
  if (!files.length) return;

  let successCount = 0;
  for (const file of files) {
    const formData = new FormData();
    formData.append("file", file);

    try {
      await fileApi.uploadLocal(formData);
      successCount += 1;
    } catch {
      // 交给请求层统一提示错误，这里只保证后续文件继续上传。
    }
  }

  input.value = "";

  if (successCount > 0) {
    ElMessage.success(`成功上传 ${successCount} 个文件`);
    emit("uploaded", { successCount, failedCount: files.length - successCount });
  }

  if (successCount < files.length) {
    ElMessage.warning(`共有 ${files.length - successCount} 个文件上传失败，请检查后重试`);
  }
}
</script>

<style scoped lang="scss">
.table-header-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}
</style>
