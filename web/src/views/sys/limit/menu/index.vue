<!-- 菜单管理 -->
<template>
  <div class="table-box">
    <ProTable ref="proTable" title="菜单列表" :indent="20" :columns="columns" :request-api="menuTreeApi" :pagination="false">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button suffix="菜单" @click="onOpen(FormOptEnum.ADD)">新增菜单 </s-button>
        <s-button suffix="菜单" :opt="FormOptEnum.DELETE" plain :disabled="!scope.isSelected" />
      </template>
      <!-- 表格 菜单类型 按钮 -->
      <template #menuType="scope">
        <el-space>
          <el-tag>{{ dictStore.dictTranslation(SysDictEnum.MENU_TYPE, scope.row.menuType) }}</el-tag>
          <el-tag v-if="scope.row.isHome === true" type="warning">首页</el-tag>
        </el-space>
      </template>
      <!-- 菜单操作 -->
      <template #operation="scope">
        <el-space>
          <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
          <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.title}】菜单`)" />
          <el-dropdown @command="handleCommand">
            <el-link type="primary" :underline="false" :icon="ArrowDown"> 更多 </el-link>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item v-if="scope.row.parentId === 0" :command="command(scope.row, cmdEnum.changeModule)">{{
                  "更改模块"
                }}</el-dropdown-item>
                <el-dropdown-item v-if="isMenu(scope.row.menuType)" :command="command(scope.row, cmdEnum.button)">{{
                  "权限按钮"
                }}</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </el-space>
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
    <!-- 更改模块 -->
    <ChangeModule ref="changeModuleFormRef" />
    <!-- 权限按钮 -->
    <Button ref="buttonFormRef" />
  </div>
</template>

<script setup lang="tsx" name="sysMenu">
import { menuTreeApi, Menu, menuDeleteApi } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { ArrowDown } from "@element-plus/icons-vue";
import { useUserStore, useDictStore } from "@/stores/modules";
import { SysDictEnum, FormOptEnum, MenuTypeDictEnum } from "@/enums";
import Form from "./components/form.vue";
import { useHandleData } from "@/hooks/useHandleData";
import ChangeModule from "./components/changeModule.vue";
import Button from "../button/index.vue";

const userStore = useUserStore();
const dictStore = useDictStore();
//遍历模块列表，生成下拉选项
const moduleOptions = userStore.moduleList.map(item => {
  return { label: item.title, value: item.id };
});
// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
// 表格配置项
const columns: ColumnProps<Menu.MenuInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "id", label: "Id", isShow: false },
  {
    prop: "module",
    label: "应用模块",
    enum: moduleOptions,
    search: {
      el: "select",
      props: {
        clearable: false,
        placeholder: "请选择应用模块"
      },
      defaultValue: moduleOptions[0].value
    },
    isShow: false
  },
  { prop: "title", label: "菜单名称", align: "left", search: { el: "input" } },
  {
    prop: "icon",
    label: "菜单图标",
    render: scope => {
      return <svg-icon icon={scope.row.icon} class="h-6 w-6" />;
    }
  },
  { prop: "menuType", label: "菜单类型" },
  { prop: "name", label: "组件名称" },
  { prop: "path", label: "路由地址", width: 200, search: { el: "input" } },
  { prop: "component", label: "组件路径", width: 250 },
  { prop: "sortCode", label: "排序", width: 80 },
  { prop: "description", label: "说明" },

  { prop: "operation", label: "操作", width: 230, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | Menu.MenuInfo = {}) {
  formRef.value?.onOpen(
    {
      opt: opt,
      record: record,
      successful: () => RefreshTable()
    },
    proTable.value?.searchParam.module
  );
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(menuDeleteApi, { ids }, msg);
  RefreshTable();
}
/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh();
}

/**判断是否是菜单 */
const isMenu = computed(() => (menuType: string) => {
  return menuType === MenuTypeDictEnum.MENU;
}); //是否是菜单

/** 更多下拉菜单命令枚举 */
enum cmdEnum {
  changeModule, //更改模块
  button //权限按钮
}

/** 下拉菜单参数接口 */
interface Command {
  row: Menu.MenuInfo;
  command: cmdEnum;
}

/**配置command的参数 */
function command(row: Menu.MenuInfo, command: cmdEnum): Command {
  return {
    row: row,
    command: command
  };
}

// 修改模块表单引用
const changeModuleFormRef = ref<InstanceType<typeof ChangeModule> | null>(null);
// 权限按钮表单引用
const buttonFormRef = ref<InstanceType<typeof Button> | null>(null);
/**
 * 更多下拉菜单点击事件
 * @param command
 */
function handleCommand(command: Command) {
  switch (command.command) {
    case cmdEnum.changeModule:
      //更改模块
      changeModuleFormRef.value?.onOpen({
        moduleOptions,
        id: command.row.id,
        title: command.row.title,
        module: command.row.module,
        successful: () => RefreshTable()
      });
      break;
    case cmdEnum.button:
      //权限按钮
      buttonFormRef.value?.onOpenTable(command.row.id);
      break;
  }
}
</script>

<style lang="scss" scoped>
.el-dropdown-link {
  display: flex;
  align-items: center;
  color: var(--el-color-primary);
  cursor: pointer;
}
:deep(.el-link__inner) {
  padding-left: 6px !important;
}
</style>
