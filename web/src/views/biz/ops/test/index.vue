<!-- 
 * @Description:  测试管理
 * @Author: superAdmin 
 * @Date: 2025-03-20 14:12:55
!-->
<template>
  <div class="table-box">
    <ProTable ref="proTable" title="测试列表" :columns="columns" :request-api="genTestApi.page">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button suffix="测试" @click="onOpen(FormOptEnum.ADD)" />
        <s-button
          type="danger"
          plain
          suffix="测试"
          :opt="FormOptEnum.DELETE"
          :disabled="!scope.isSelected"
          @click="onDelete(scope.selectedListIds, '删除所选测试')"
        />
      </template>
      <!-- 操作 -->
      <template #operation="scope">
        <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
        <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], '删除所选测试')" />
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
  </div>
</template>

<script setup lang="ts" name="test">
import { genTestApi, GenTest } from "@/api";
import { FormOptEnum } from "@/enums";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import Form from "./components/form.vue";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();
const sexOptions = dictStore.getDictList("GENDER"); //性别选项
const nationOptions = dictStore.getDictList("NATION"); //民族选项
const statusOptions = dictStore.getDictList("COMMON_STATUS"); //状态选项

const columns: ColumnProps<GenTest.GenTestInfo>[] = [
  { type: "selection", fixed: "left", width: 80 },
  { prop: "name", label: "姓名", search: { el: "input" } },
  { prop: "sex", label: "性别", enum: sexOptions, search: { el: "select" } },
  { prop: "nation", label: "民族", enum: nationOptions, search: { el: "select" } },
  { prop: "age", label: "年龄" },
  { prop: "bir", label: "生日" },
  { prop: "money", label: "存款" },
  { prop: "sortCode", label: "排序码" },
  { prop: "status", label: "状态", enum: statusOptions },
  { prop: "operation", label: "操作", width: 230, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | GenTest.GenTestInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(genTestApi.delete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh();
}
</script>

<style lang="scss" scoped></style>
