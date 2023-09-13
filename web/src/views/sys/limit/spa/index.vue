<!-- 单页管理 -->
<template>
  <div class="table-box">
    <ProTable ref="proTable" title="单页列表" :columns="columns" :request-api="spaPageApi">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button suffix="单页" @click="onOpen(FormOptEnum.ADD)" />
        <s-button
          type="danger"
          plain
          suffix="单页"
          :opt="FormOptEnum.DELETE"
          :disabled="!scope.isSelected"
          @click="onDelete(scope.selectedListIds, '删除所选页面')"
        />
      </template>
      <!-- 表格 菜单类型 按钮 -->
      <template #menuType="scope">
        <el-space wrap>
          <el-tag v-if="scope.row.menuType === MenuTypeDictEnum.MENU" type="success">{{
            dictStore.dictTranslation(SysDictEnum.MENU_TYPE, MenuTypeDictEnum.MENU)
          }}</el-tag>
          <el-tag v-else-if="scope.row.menuType === MenuTypeDictEnum.LINK" type="warning">{{
            dictStore.dictTranslation(SysDictEnum.MENU_TYPE, MenuTypeDictEnum.LINK)
          }}</el-tag>
          <el-tag v-else type="info">{{ dictStore.dictTranslation(SysDictEnum.MENU_TYPE, scope.row.menuType) }}</el-tag>
          <el-tag v-if="scope.row.isHome === true" type="danger">首页</el-tag>
        </el-space>
      </template>
      <!-- 菜单操作 -->
      <template #operation="scope">
        <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
        <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.title}】页面`)" />
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
  </div>
</template>

<script setup lang="tsx" name="sysSpa">
import { spaPageApi, spaDeleteApi } from "@/api";
import { Spa } from "@/api/interface";
import { useHandleData } from "@/hooks/useHandleData";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { useDictStore } from "@/stores/modules";
import { FormOptEnum, SysDictEnum, MenuTypeDictEnum } from "@/enums";
import Form from "./components/form.vue";

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();

// 单页类型选项
const spaTypeOptions = dictStore
  .getDictList(SysDictEnum.MENU_TYPE)
  .filter(item => item.value == MenuTypeDictEnum.MENU || item.value == MenuTypeDictEnum.LINK);
// 表格配置项
const columns: ColumnProps<Spa.SpaInfo>[] = [
  { type: "selection", fixed: "left", width: 80 },
  { prop: "searchKey", label: "关键字", search: { el: "input" }, isShow: false },
  { prop: "title", label: "单页名称" },
  {
    prop: "icon",
    label: "菜单图标",
    render: scope => {
      return <svg-icon icon={scope.row.icon} class="h-6 w-6" />;
    }
  },
  {
    prop: "menuType",
    label: "单页类型",
    enum: spaTypeOptions,
    search: { el: "tree-select" }
  },
  { prop: "path", label: "路由地址" },
  { prop: "component", label: "组件路径" },
  { prop: "sortCode", label: "排序" },
  { prop: "description", label: "说明" },
  { prop: "createTime", label: "创建时间" },
  { prop: "operation", label: "操作", width: 200, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | Spa.SpaInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(spaDeleteApi, { ids }, msg);
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
