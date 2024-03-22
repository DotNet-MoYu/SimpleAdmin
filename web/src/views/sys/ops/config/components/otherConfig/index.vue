<!-- 
 * @Description: 其他配置
 * @Author: huguodong 
 * @Date: 2024-02-02 08:43:44
!-->
<template>
  <div class="table-box min-h-300px">
    <ProTable ref="proTable" class="table" :columns="columns" :request-api="sysConfigApi.page">
      <!-- 表格 header 按钮 -->
      <template #tableHeader>
        <s-button :suffix="title" @click="onOpen(FormOptEnum.ADD)" />
      </template>
      <!-- 操作 -->
      <template #operation="scope">
        <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
        <s-button link :opt="FormOptEnum.DELETE" @click="onDelete(scope.row.id, `删除【${scope.row.configKey}】配置`)" />
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
  </div>
</template>

<script setup lang="ts">
import { sysConfigApi, SysConfig } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { useHandleData } from "@/hooks/useHandleData";
import { FormOptEnum } from "@/enums";
import Form from "./form.vue";

const title = "配置";

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<SysConfig.ConfigInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "configKey", label: "配置键" },
  { prop: "configValue", label: "配置值" },
  { prop: "remark", label: "备注" },
  { prop: "sortCode", label: "排序", width: 80 },
  { prop: "operation", label: "操作", fixed: "right", width: 150 }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | SysConfig.ConfigInfo = {}) {
  formRef.value?.onOpen({
    opt: opt,
    record: record,
    successful: RefreshTable //刷新子表格
  });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(id: number | string, msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(sysConfigApi.delete, { id }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh(); //刷新主表格
}
</script>

<style lang="scss" scoped></style>
