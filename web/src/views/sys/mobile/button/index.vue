<!-- 
 * @Description: 权限按钮管理
 * @Author: huguodong 
 * @Date: 2023-12-15 15:42:52
!-->
<template>
  <div>
    <form-container v-model="visible" title="权限按钮" form-size="60%">
      <div class="table-box min-h-300px">
        <ProTable ref="proTable" title="按钮列表" class="table" :columns="columns" :request-api="getPage">
          <!-- 表格 header 按钮 -->
          <template #tableHeader="scope">
            <s-button suffix="按钮" @click="onOpen(FormOptEnum.ADD)" />
            <s-button @click="onOpenBatch()"> 批量新增 </s-button>
            <s-button
              type="danger"
              plain
              suffix="按钮"
              :opt="FormOptEnum.DELETE"
              :disabled="!scope.isSelected"
              @click="onDelete(scope.selectedListIds, '删除所选按钮')"
            />
          </template>
          <!-- 操作 -->
          <template #operation="scope">
            <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
            <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.title}】按钮`)" />
          </template>
        </ProTable>
      </div>
      <template #footer>
        <el-button type="primary" @click="onClose"> 确定 </el-button>
      </template>
    </form-container>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
    <!-- 批量新增 -->
    <Batch ref="batchRef" />
  </div>
</template>

<script setup lang="ts">
import { mobileButtonApi, Button } from "@/api";
import { useHandleData } from "@/hooks/useHandleData";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { FormOptEnum } from "@/enums";
import Form from "./components/form.vue";
import Batch from "./components/batch.vue";

const visible = ref(false); //是否显示表单
const parentId = ref<number | string>(0); //按钮父Id
// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
// 表格配置项
const columns: ColumnProps<Button.ButtonInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "title", label: "名称" },
  { prop: "code", label: "编码" },
  { prop: "sortCode", label: "排序" },
  { prop: "description", label: "说明" },
  { prop: "operation", label: "操作", width: 150, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | Button.ButtonInfo = { parentId: parentId.value }) {
  formRef.value?.onOpen({
    opt: opt,
    record: record,
    successful: () => RefreshTable()
  });
}

// 批量新增引用
const batchRef = ref<InstanceType<typeof Batch> | null>(null);

/**
 * 打开批量新增表单
 */
function onOpenBatch() {
  batchRef.value?.onOpen({
    parentId: parentId.value,
    title: "",
    code: "",
    successful: RefreshTable
  });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(mobileButtonApi.delete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh();
}

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpenTable(menuId: number | string) {
  visible.value = true;
  parentId.value = menuId;
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
}

/**
 * 获取按钮分页数据
 * 如果你想在请求之前对当前请求参数做一些操作，可以自定义如下函数：params 为当前所有的请求参数（包括分页），最后返回请求列表接口
 * 默认不做操作就直接在 ProTable 组件上绑定	:requestApi="getUserList"
 */
function getPage(params: any) {
  let newParams = JSON.parse(JSON.stringify(params)); //转换成json字符串再转换成json对象
  newParams.parentId = parentId.value; //按钮父Id
  return mobileButtonApi.page(newParams);
}
// 暴露给父组件的方法
defineExpose({
  onOpenTable
});
</script>

<style lang="scss" scoped></style>
