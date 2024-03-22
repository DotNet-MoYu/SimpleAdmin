<!-- 
 * @Description: 岗位管理
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:43
!-->
<template>
  <div class="main-box">
    <TreeFilter
      ref="treeFilter"
      label="name"
      title="机构列表"
      :show-all="false"
      :request-api="bizOrgApi.tree"
      :default-value="initParam.orgId"
      @change="changeTreeFilter"
    />
    <div class="table-box">
      <ProTable ref="proTable" title="岗位列表" :columns="columns" :request-api="bizPositionApi.page" :init-param="initParam">
        <!-- 表格 header 按钮 -->
        <template #tableHeader="scope">
          <s-button v-auth="bizPositionButtonCode.add" suffix="岗位" @click="onOpen(FormOptEnum.ADD)" />
          <s-button
            v-auth="bizPositionButtonCode.batchDelete"
            type="danger"
            plain
            suffix="岗位"
            :opt="FormOptEnum.DELETE"
            :disabled="!scope.isSelected"
            @click="onDelete(scope.selectedListIds, '删除所选岗位')"
          />
        </template>
        <!-- 状态 -->
        <template #status="scope">
          <el-tag v-if="scope.row.status === CommonStatusEnum.ENABLE" type="success">{{
            dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status)
          }}</el-tag>
          <el-tag v-else type="danger">{{ dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status) }}</el-tag>
        </template>
        <!-- 操作 -->
        <template #operation="scope">
          <s-button v-auth="bizPositionButtonCode.edit" link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
          <s-button
            v-auth="bizPositionButtonCode.delete"
            link
            :opt="FormOptEnum.DELETE"
            @click="onDelete([scope.row.id], `删除【${scope.row.name}】岗位`)"
          />
        </template>
      </ProTable>
      <!-- 新增/编辑表单 -->
      <Form ref="formRef" />
    </div>
  </div>
</template>

<script setup lang="ts" name="sysPosition">
import { bizOrgApi, bizPositionApi, SysPosition, bizPositionButtonCode } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { SysDictEnum, FormOptEnum, CommonStatusEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import TreeFilter from "@/components/TreeFilter/index.vue";
import Form from "./components/form.vue";

const dictStore = useDictStore(); //字典仓库
// 岗位类型选项
const posCategoryOptions = dictStore.getDictList(SysDictEnum.POSITION_CATEGORY);
// 状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);
const treeFilter = ref<InstanceType<typeof TreeFilter> | null>(null);

interface InitParam {
  orgId: number | string;
}
// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<InitParam>({ orgId: 0 });
// 获取 ProTable 元素
const proTable = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<SysPosition.SysPositionInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "name", label: "岗位名称", search: { el: "input" } },
  {
    prop: "category",
    label: "岗位分类",
    enum: posCategoryOptions,
    search: { el: "tree-select" }
  },
  { prop: "code", label: "岗位编码", search: { el: "input" } },
  { prop: "status", label: "状态", enum: statusOptions, search: { el: "tree-select" } },
  { prop: "sortCode", label: "排序" },
  { prop: "createTime", label: "创建时间" },

  { prop: "operation", label: "操作", width: 150, fixed: "right" }
];

/** 部门切换 */
function changeTreeFilter(val: number | string) {
  proTable.value!.pageable.pageNum = 1;
  if (val != "") {
    // 如果传入的val不为空，则将val赋值给initParam.parentId
    initParam.orgId = val;
  } else {
    // 否则将initParam.parentId赋值为0
    initParam.orgId = 0;
  }
}

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | SysPosition.SysPositionInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(bizPositionApi.delete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh(); //刷新表格
  treeFilter.value?.refresh(); //刷新树形筛选器
}
</script>

<style lang="scss" scoped></style>
