<template>
  <div class="document-log-page">
    <ProTable ref="proTable" :columns="columns" :request-api="requestPage" :init-param="initParam" :tool-button="['refresh', 'setting', 'search']">
      <template #tableHeader>
        <el-button type="danger" plain @click="onEmpty">清空日志</el-button>
      </template>
      <template #type="scope">
        <el-tag :type="logTypeTagType(scope.row.type)" effect="plain">{{ logTypeLabel(scope.row.type) }}</el-tag>
      </template>
    </ProTable>
  </div>
</template>

<script setup lang="ts" name="bizDocumentLog">
import { BizDocumentLog, documentLogApi } from "@/api";
import { ColumnProps, EnumProps, ProTableInstance } from "@/components/ProTable/interface";
import { useHandleData } from "@/hooks/useHandleData";

interface LogSearchParam extends BizDocumentLog.Page {
  doTimeRange: string[];
}

const proTable = ref<ProTableInstance>();
const initParam = reactive<BizDocumentLog.Page>({
  pageNum: 1,
  pageSize: 10
});

const logTypeOptions: EnumProps[] = [
  { label: "新建文件夹", value: 1 },
  { label: "上传文件", value: 2 },
  { label: "上传文件夹", value: 3 },
  { label: "重命名", value: 4 },
  { label: "移动", value: 5 },
  { label: "删除", value: 6 },
  { label: "恢复", value: 7 },
  { label: "授权", value: 8 },
  { label: "清空回收站", value: 9 },
  { label: "永久删除", value: 10 }
];

const columns: ColumnProps<BizDocumentLog.LogInfo>[] = [
  { prop: "name", label: "操作名称", minWidth: 180, search: { el: "input" } },
  { prop: "type", label: "操作类型", minWidth: 140, enum: logTypeOptions, search: { el: "select" } },
  { prop: "detail", label: "操作详情", minWidth: 320 },
  { prop: "userName", label: "用户名", minWidth: 120, search: { el: "input" } },
  {
    prop: "doTime",
    label: "操作时间",
    minWidth: 180,
    search: {
      el: "date-picker",
      key: "doTimeRange",
      props: {
        type: "daterange",
        valueFormat: "YYYY-MM-DD",
        rangeSeparator: "至",
        startPlaceholder: "开始日期",
        endPlaceholder: "结束日期"
      }
    }
  }
];

function requestPage(params: LogSearchParam) {
  const { doTimeRange, ...rest } = params;

  // ProTable 搜索表单更适合直接传区间数组，这里在请求前转成后端约定的 start/end 字段。
  return documentLogApi.page({
    ...rest,
    startTime: doTimeRange?.[0] || undefined,
    endTime: doTimeRange?.[1] || undefined
  });
}

function logTypeLabel(type: BizDocumentLog.LogType) {
  return logTypeOptions.find(item => item.value === type)?.label || "未知";
}

function logTypeTagType(type: BizDocumentLog.LogType) {
  if ([1, 2, 3].includes(type)) return "success";
  if ([6, 10].includes(type)) return "danger";
  if (type === 5) return "warning";
  if ([7, 8, 9].includes(type)) return "info";
  return undefined;
}

async function onEmpty() {
  await useHandleData(documentLogApi.empty, {}, "清空文件日志");
  proTable.value?.refresh();
}
</script>
