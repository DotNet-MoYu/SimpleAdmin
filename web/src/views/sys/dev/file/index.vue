<template>
  <div class="table-box file-page">
    <ProTable ref="proTable" title="文件列表" :columns="columns" :request-api="fileApi.page" :init-param="initParam">
      <template #tableHeader="scope">
        <FileToolbar
          :is-selected="scope.isSelected"
          :selected-ids="scope.selectedListIds"
          @delete-selected="onDelete($event, '删除所选文件')"
          @clear-selection="proTable?.clearSelection()"
          @uploaded="refreshTable"
        />
      </template>

      <template #name="scope">
        <FileInfoCell :row="scope.row" />
      </template>

      <template #engine="scope">
        <el-tag :type="engineTagType(scope.row.engine)" effect="plain">
          {{ engineLabel(scope.row.engine) }}
        </el-tag>
      </template>

      <template #bucket="scope">
        <span>{{ scope.row.bucket || "-" }}</span>
      </template>

      <template #storagePath="scope">
        <el-tooltip :content="scope.row.storagePath || '-'" placement="top">
          <span class="path-text">{{ scope.row.storagePath || "-" }}</span>
        </el-tooltip>
      </template>

      <template #createUser="scope">
        <span>{{ scope.row.createUser || "-" }}</span>
      </template>

      <template #operation="scope">
        <el-space wrap>
          <el-button link type="primary" @click="onDownload(scope.row)">下载</el-button>
          <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.name}】文件`)">删除</s-button>
        </el-space>
      </template>
    </ProTable>
  </div>
</template>

<script setup lang="ts" name="sysFile">
import { fileApi, SysFile } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import FileInfoCell from "./components/fileInfoCell.vue";
import FileToolbar from "./components/fileToolbar.vue";

const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();
const initParam = reactive<SysFile.Page>({
  pageNum: 1,
  pageSize: 10,
  sortField: "id",
  sortOrder: "desc"
});

const engineOptions = computed(() => dictStore.getDictList(SysDictEnum.FILE_ENGINE));

const columns: ColumnProps<SysFile.SysFileInfo>[] = [
  { type: "selection", fixed: "left", width: 54 },
  { prop: "searchKey", label: "文件名", search: { el: "input" }, isShow: false },
  { prop: "name", label: "文件信息", minWidth: 260 },
  { prop: "engine", label: "存储引擎", minWidth: 120, enum: engineOptions, search: { el: "select" } },
  { prop: "sizeInfo", label: "文件大小", minWidth: 110 },
  { prop: "bucket", label: "存储桶", minWidth: 140 },
  { prop: "storagePath", label: "存储路径", minWidth: 260 },
  { prop: "createUser", label: "创建人", minWidth: 120 },
  { prop: "createTime", label: "创建时间", minWidth: 180 },
  { prop: "updateTime", label: "更新时间", minWidth: 180 },
  { prop: "operation", label: "操作", width: 150, fixed: "right" }
];

async function onDelete(ids: Array<number | string>, msg: string) {
  await useHandleData(fileApi.delete, { ids }, msg);
  refreshTable();
}

async function onDownload(row: SysFile.SysFileInfo) {
  const response = (await fileApi.download({ id: row.id })) as any;
  const blob = new Blob([response.data], { type: "application/octet-stream;charset=UTF-8" });
  const contentDisposition = response.headers["content-disposition"];
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  const match = /filename=([^;]+\.[^\.;]+);*/.exec(contentDisposition || "");
  link.download = match ? decodeURIComponent(match[1]) : row.name;
  link.click();
  document.body.appendChild(link);
  document.body.removeChild(link);
  window.URL.revokeObjectURL(link.href);
}

function refreshTable() {
  proTable.value?.refresh();
}

function engineLabel(engine?: string) {
  if (!engine) return "-";
  const label = dictStore.dictTranslation(SysDictEnum.FILE_ENGINE, engine);
  return label === "无此字典" ? engine : label;
}

function engineTagType(engine?: string) {
  if (engine === "LOCAL") return "success";
  if (engine === "MINIO") return "warning";
  return "info";
}
</script>

<style scoped lang="scss">
.file-page {
  .path-text {
    display: inline-block;
    max-width: 100%;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    color: var(--el-text-color-regular);
  }
}
</style>
