<template>
  <div class="file-info-cell">
    <div class="file-thumb">
      <el-image v-if="row.thumbnail" :src="row.thumbnail" fit="cover" preview-disabled />
      <el-icon v-else :size="18" class="file-icon">
        <Picture v-if="isImageFile" />
        <Document v-else />
      </el-icon>
    </div>
    <div class="file-meta">
      <span class="file-name">{{ row.name || "-" }}</span>
      <span class="file-suffix">{{ suffixText }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { SysFile } from "@/api";
import { Document, Picture } from "@element-plus/icons-vue";

const props = defineProps<{
  row: SysFile.SysFileInfo;
}>();

const suffixText = computed(() => {
  if (!props.row.suffix) return "未知类型";
  return props.row.suffix.startsWith(".") ? props.row.suffix.toUpperCase() : `.${props.row.suffix}`.toUpperCase();
});

const isImageFile = computed(() => {
  const suffix = (props.row.suffix || "").replace(".", "").toLowerCase();
  return ["png", "jpg", "jpeg", "gif", "bmp", "webp", "svg"].includes(suffix);
});
</script>

<style scoped lang="scss">
.file-info-cell {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 0;
}

.file-thumb {
  width: 40px;
  height: 40px;
  flex-shrink: 0;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  background: var(--el-fill-color-light);
  overflow: hidden;
  border: 1px solid var(--el-border-color-lighter);

  :deep(.el-image) {
    width: 100%;
    height: 100%;
  }
}

.file-icon {
  color: var(--el-color-primary);
}

.file-meta {
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.file-name {
  color: var(--el-text-color-primary);
  font-weight: 500;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.file-suffix {
  color: var(--el-text-color-secondary);
  font-size: 12px;
}
</style>
