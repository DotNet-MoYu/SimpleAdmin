<template>
  <form-container v-model="visible" :title="title" form-size="960px">
    <div class="preview-container">
      <el-image v-if="preview.previewType === 'image' && preview.content" :src="preview.content" fit="contain" class="preview-image" />
      <el-scrollbar v-else-if="preview.previewType === 'text'" max-height="70vh" class="preview-text-wrap">
        <pre class="preview-text">{{ preview.content }}</pre>
      </el-scrollbar>
      <el-empty v-else :description="unsupportedTip" />
    </div>
    <template #footer>
      <el-button v-if="fileId" type="primary" plain @click="downloadCurrent">下载文件</el-button>
      <el-button @click="visible = false">关闭</el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { BizDocument, documentApi } from "@/api";
import { ElMessage } from "element-plus";

const visible = ref(false);
const title = ref("文件预览");
const fileId = ref<number>();
const unsupportedTip = ref("当前文件类型暂不支持预览，请下载后查看。");
const preview = reactive<BizDocument.PreviewInfo>({
  previewType: "none",
  contentType: "application/octet-stream",
  content: "",
  fileName: ""
});

async function onOpen(id: number, name?: string) {
  fileId.value = id;
  title.value = name ? `文件预览 - ${name}` : "文件预览";
  const { data } = await documentApi.preview({ id });
  Object.assign(preview, data);
  unsupportedTip.value = isOfficeLike(data.fileName) ? "Office / PDF 在线预览暂未接入，请先下载查看。" : "当前文件类型暂不支持预览，请下载后查看。";
  visible.value = true;
}

async function downloadCurrent() {
  if (!fileId.value) return;
  const response = (await documentApi.download({ id: fileId.value })) as any;
  const blob = new Blob([response.data], { type: "application/octet-stream;charset=UTF-8" });
  const contentDisposition = response.headers["content-disposition"];
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  const match = /filename=([^;]+\.[^\.;]+);*/.exec(contentDisposition || "");
  link.download = match ? decodeURIComponent(match[1]) : preview.fileName || "download";
  link.click();
  document.body.appendChild(link);
  document.body.removeChild(link);
  window.URL.revokeObjectURL(link.href);
  if (preview.previewType === "none") {
    ElMessage.info("在线预览待接入，已为你开始下载文件。");
  }
}

function isOfficeLike(fileName: string) {
  const suffix = fileName?.split(".").pop()?.toLowerCase() || "";
  return ["doc", "docx", "xls", "xlsx", "ppt", "pptx", "pdf"].includes(suffix);
}

defineExpose({ onOpen });
</script>

<style scoped lang="scss">
.preview-container {
  min-height: 320px;
}

.preview-image {
  width: 100%;
  max-height: 70vh;
}

.preview-text-wrap {
  width: 100%;
  border: 1px solid var(--el-border-color);
  border-radius: 8px;
}

.preview-text {
  margin: 0;
  padding: 16px;
  white-space: pre-wrap;
  word-break: break-word;
  line-height: 1.6;
}
</style>
