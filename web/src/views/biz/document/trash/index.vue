<template>
  <div class="table-box">
    <ProTable ref="proTable" :columns="columns" :request-api="trashApi.page">
      <template #tableHeader="scope">
        <div class="table-header-wrap">
          <div class="table-header-actions">
            <el-button type="primary" plain :disabled="!scope.isSelected" @click="onRecover(scope.selectedListIds)">批量恢复</el-button>
            <el-button type="danger" plain :disabled="!scope.isSelected" @click="onDeletePermanent(scope.selectedListIds)">批量删除</el-button>
            <el-button type="danger" plain @click="onEmpty">清空回收站</el-button>
          </div>
          <div class="selection-meta">
            <el-icon><InfoFilled /></el-icon>
            <span>已选择：{{ scope.selectedListIds.length }}</span>
          </div>
        </div>
      </template>
      <template #fileTypeLabel="scope">
        <el-tag :type="fileTypeTagType(scope.row.fileTypeLabel)" effect="plain">{{ scope.row.fileTypeLabel || "文件" }}</el-tag>
      </template>
      <template #label="scope">
        <el-tag v-if="scope.row.label" type="info" effect="plain">{{ resolveLabel(scope.row.label) }}</el-tag>
        <span v-else class="text-#909399">-</span>
      </template>
      <template #remark="scope">
        <span class="remark-text">{{ scope.row.remark || "-" }}</span>
      </template>
      <template #operation="scope">
        <el-space>
          <el-button link type="primary" @click="onRecover([scope.row.id])">恢复</el-button>
          <el-button link type="danger" @click="onDeletePermanent([scope.row.id])">删除</el-button>
        </el-space>
      </template>
    </ProTable>
  </div>
</template>

<script setup lang="ts" name="bizDocumentTrash">
import { BizTrash, trashApi } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import { InfoFilled } from "@element-plus/icons-vue";

const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();

const fileTypeOptions: Array<{ label: string; value: string }> = [
  { label: "文件夹", value: "文件夹" },
  { label: "文档", value: "文档" },
  { label: "图片", value: "图片" },
  { label: "压缩包", value: "压缩包" },
  { label: "应用程序", value: "应用程序" },
  { label: "文件", value: "文件" }
];

const docLabelOptions = computed(() => dictStore.getDictList("doc_label"));

const columns: ColumnProps<BizTrash.TrashInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "name", label: "名称", minWidth: 220, search: { el: "input" } },
  { prop: "fileTypeLabel", label: "类型", minWidth: 120 },
  { prop: "fileType", label: "文件类型", enum: fileTypeOptions, search: { el: "select" }, isShow: false, isSetting: false },
  { prop: "createTime", label: "创建时间", minWidth: 180 },
  { prop: "createUserName", label: "创建人", minWidth: 120 },
  { prop: "updateTime", label: "删除时间", minWidth: 180 },
  { prop: "updateUserName", label: "删除人", minWidth: 120 },
  { prop: "sizeInfo", label: "大小", minWidth: 120 },
  { prop: "label", label: "标签", enum: docLabelOptions, search: { el: "select" }, minWidth: 140 },
  { prop: "remark", label: "备注", minWidth: 180 },
  { prop: "operation", label: "操作", minWidth: 180, fixed: "right" }
];

async function onRecover(ids: Array<string | number>) {
  const validIds = ids.map(id => Number(id));
  if (!validIds.length) return;
  await useHandleData(trashApi.recover, { ids: validIds }, "恢复选中文件");
  proTable.value?.refresh();
}

async function onDeletePermanent(ids: Array<string | number>) {
  const validIds = ids.map(id => Number(id));
  if (!validIds.length) return;
  await useHandleData(trashApi.deletePermanent, { ids: validIds }, "永久删除选中文件");
  proTable.value?.refresh();
}

async function onEmpty() {
  await useHandleData(trashApi.empty, {}, "清空回收站");
  proTable.value?.refresh();
}

function resolveLabel(value?: string | number) {
  if (value == null || value === "") return "-";
  const text = dictStore.dictTranslation("doc_label", String(value));
  return text === "无此字典" ? String(value) : text;
}

function fileTypeTagType(fileType?: string) {
  switch (fileType) {
    case "文件夹":
      return "warning";
    case "文档":
      return "info";
    case "图片":
      return "danger";
    case "压缩包":
      return "success";
    default:
      return undefined;
  }
}
</script>

<style scoped lang="scss">
.table-header-wrap {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}

.table-header-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.selection-meta {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  min-height: 32px;
  padding: 0 12px;
  color: var(--el-color-primary);
  background: var(--el-color-primary-light-9);
  border: 1px solid var(--el-color-primary-light-7);
  border-radius: 6px;
}

.remark-text {
  display: inline-block;
  max-width: 180px;
  color: var(--el-text-color-regular);
  word-break: break-word;
}
</style>
