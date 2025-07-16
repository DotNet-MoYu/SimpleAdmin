<!-- 
 * @Description:  模块管理
 * @Author: superAdmin 
 * @Date: 2025-06-25 14:37:52
!-->
<template>
  <div class="table-box">
    <ProTable ref="proTable" title="模块列表" :columns="columns" :request-api="mobileModuleApi.page">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button suffix="模块" @click="onOpen(FormOptEnum.ADD)" />
        <s-button
          type="danger"
          plain
          suffix="模块"
          :opt="FormOptEnum.DELETE"
          :disabled="!scope.isSelected"
          @click="onDelete(scope.selectedListIds, '删除所选模块')"
        />
      </template>
      <!-- 状态 -->
      <template #status="scope">
        <el-tag v-if="scope.row.status === CommonStatusEnum.ENABLE" type="success">{{
          dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status)
        }}</el-tag>
        <el-tag v-else type="danger">{{ dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status) }}</el-tag>
      </template>
      <!-- 图标 -->
      <template #icon="scope">
        <svg-icon :icon="scope.row.icon" :color="scope.row.color" class="h-6 w-8" />
      </template>
      <!-- 颜色 -->
      <template #color="scope">
        <el-tag :color="scope.row.color" effect="dark" :style="{ borderColor: scope.row.color }" size="large">{{ scope.row.color }}</el-tag>
      </template>
      <!-- 操作 -->
      <template #operation="scope">
        <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
        <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除所选模块`)" />
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
  </div>
</template>

<script setup lang="tsx" name="mobileModule">
import { mobileModuleApi, MobileModule } from "@/api";
import { FormOptEnum, CommonStatusEnum, SysDictEnum } from "@/enums";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import Form from "./components/form.vue";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";

const dictStore = useDictStore(); //字典仓库
// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();

const columns: ColumnProps<MobileModule.MobileModuleInfo>[] = [
  { type: "selection", fixed: "left", width: 80 },
  { prop: "title", label: "显示名称", search: { el: "input" } },
  { prop: "description", label: "描述" },
  {
    prop: "icon",
    label: "菜单图标"
  },
  {
    prop: "color",
    label: "颜色"
  },
  { prop: "sortCode", label: "排序码" },
  { prop: "status", label: "状态" },
  { prop: "operation", label: "操作", width: 230, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | MobileModule.MobileModuleInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(mobileModuleApi.delete, { ids }, msg);
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
