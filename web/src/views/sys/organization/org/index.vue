<!-- 组织管理 -->
<template>
  <div class="main-box">
    <TreeFilter
      ref="treeFilter"
      label="name"
      title="组织列表"
      :request-api="sysOrgTreeApi"
      :default-value="initParam.parentId"
      @change="changeTreeFilter"
    />
    <div class="table-box">
      <ProTable ref="proTable" title="用户列表" :columns="columns" :request-api="sysOrgPageApi" :init-param="initParam">
        <!-- 表格 header 按钮 -->
        <template #tableHeader="scope">
          <s-button suffix="组织" @click="onOpen(FormOptEnum.ADD)" />
          <s-button :disabled="!scope.isSelected" :icon="CopyDocument" @click="onOpenCopy(scope.selectedListIds)">复制组织</s-button>
          <s-button
            type="danger"
            plain
            suffix="组织"
            :opt="FormOptEnum.DELETE"
            :disabled="!scope.isSelected"
            @click="onDelete(scope.selectedListIds, '删除所选组织')"
          />
        </template>
        <!-- 操作 -->
        <template #operation="scope">
          <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
          <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.name}】组织`)" />
        </template>
      </ProTable>
      <!-- 新增/编辑表单 -->
      <Form ref="formRef" />
      <!-- 复制表单 -->
      <Copy ref="copyRef" />
    </div>
  </div>
</template>

<script setup lang="ts" name="sysOrg">
import { sysOrgTreeApi, sysOrgPageApi, SysOrg, sysOrgDeleteApi } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { CopyDocument } from "@element-plus/icons-vue";
import { SysDictEnum, FormOptEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import TreeFilter from "@/components/TreeFilter/index.vue";
import Form from "./components/form.vue";
import Copy from "./components/copy.vue";

const dictStore = useDictStore(); //字典仓库
// 组织类型选项
const orgCategoryOptions = dictStore.getDictList(SysDictEnum.ORG_CATEGORY);

const treeFilter = ref<InstanceType<typeof TreeFilter> | null>(null);

interface InitParam {
  parentId: number | string;
}
// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<InitParam>({ parentId: 0 });
// 获取 ProTable 元素
const proTable = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<SysOrg.SysOrgInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "name", label: "组织名称", search: { el: "input" } },
  { prop: "names", label: "组织全称" },
  {
    prop: "category",
    label: "分类",
    enum: orgCategoryOptions,
    search: { el: "tree-select" }
  },
  { prop: "code", label: "编码", search: { el: "input" } },
  { prop: "sortCode", label: "排序" },
  { prop: "createTime", label: "创建时间" },
  { prop: "operation", label: "操作", width: 150, fixed: "right" }
];

/** 部门切换 */
function changeTreeFilter(val: number | string) {
  proTable.value!.pageable.pageNum = 1;
  if (val != "") {
    // 如果传入的val不为空，则将val赋值给initParam.parentId
    initParam.parentId = val;
  } else {
    // 否则将initParam.parentId赋值为0
    initParam.parentId = 0;
  }
}

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | SysOrg.SysOrgInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

// 表单引用
const copyRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开复制表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpenCopy(ids: string[]) {
  console.log("[ ids ] >", ids);
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(sysOrgDeleteApi, { ids }, msg);
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
