<!-- 
 * @Description:  菜单管理
 * @Author: superAdmin 
 * @Date: 2025-06-26 16:41:59
!-->
<template>
  <div class="table-box">
    <ProTable ref="proTable" title="菜单列表" :columns="columns" :request-api="getTableList" :request-auto="false" :pagination="false">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button suffix="菜单" @click="onOpen(FormOptEnum.ADD)" />
        <s-button
          type="danger"
          plain
          suffix="菜单"
          :opt="FormOptEnum.DELETE"
          :disabled="!scope.isSelected"
          @click="onDelete(scope.selectedListIds, '删除所选菜单')"
        />
      </template>
      <!-- 表格 菜单类型 按钮 -->
      <template #menuType="scope">
        <el-space>
          <!-- <el-tag>{{ dictStore.dictTranslation(SysDictEnum.MENU_TYPE, scope.row.menuType) }}</el-tag> -->
          <el-tag v-if="scope.row.menuType === MenuTypeDictEnum.MENU" type="success">{{
            dictStore.dictTranslation(SysDictEnum.MENU_TYPE, scope.row.menuType)
          }}</el-tag>
          <el-tag v-else type="info">{{ dictStore.dictTranslation(SysDictEnum.MENU_TYPE, scope.row.menuType) }}</el-tag>
        </el-space>
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
        <el-space>
          <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
          <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.title}】菜单`)" />
          <el-dropdown @command="handleCommand">
            <el-link type="primary" :underline="false" :icon="ArrowDown"> 更多 </el-link>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item v-if="scope.row.parentId === 0" :command="command(scope.row, cmdEnum.ChangeModule)">{{
                  cmdEnum.ChangeModule
                }}</el-dropdown-item>
                <el-dropdown-item v-if="isMenu(scope.row.menuType)" :command="command(scope.row, cmdEnum.Button)">{{
                  cmdEnum.Button
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

<script setup lang="ts" name="mobileMenu">
import { MobileMenu, mobileMenuApi } from "@/api";
import { ArrowDown } from "@element-plus/icons-vue";
import { FormOptEnum, CommonStatusEnum, SysDictEnum, MenuTypeDictEnum } from "@/enums";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import Form from "./components/form.vue";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import ChangeModule from "./components/changeModule.vue";
import Button from "../button/index.vue";

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();
const menuTypeOptions = dictStore.getDictList("MENU_TYPE"); //菜单类型选项
const regTypeOptions = dictStore.getDictList("YES_NO"); //正规则选项
const statusOptions = dictStore.getDictList("COMMON_STATUS"); //状态选项
const moduleOptions = ref<{ label: string; value: string | number }[]>([]);
const columns: ColumnProps<MobileMenu.MobileMenuInfo>[] = [
  { type: "selection", fixed: "left", width: 80 },
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
      }
    },
    isShow: false
  },
  { prop: "title", label: "菜单名称", width: 150, align: "left", search: { el: "input" } },
  { prop: "menuType", label: "菜单类型", enum: menuTypeOptions, search: { el: "select" } },
  { prop: "path", label: "路径" },
  { prop: "icon", label: "图标" },
  { prop: "color", label: "颜色" },
  { prop: "sortCode", label: "排序码" },
  { prop: "regType", label: "正规则", enum: regTypeOptions },
  { prop: "status", label: "状态", enum: statusOptions },
  { prop: "description", label: "描述" },
  { prop: "operation", label: "操作", width: 230, fixed: "right" }
];

onMounted(() => {
  // 获取应用模块下拉选项
  mobileMenuApi.moduleSelector().then(res => {
    moduleOptions.value = res.data.map(item => ({ label: item.title, value: item.id }));
    // 设置默认值（取第一个模块）
    if (moduleOptions.value.length > 0) {
      // 1. 赋值初始化参数
      proTable.value!.searchInitParam["module"] = moduleOptions.value[0].value;
      // 2. 重置参数（使新参数生效）
      proTable.value?.resetParam();
      proTable.value?.getTableList();
    }
  });
});

// 如果你想在请求之前对当前请求参数做一些操作，可以自定义如下函数：params 为当前所有的请求参数（包括分页），最后返回请求列表接口
// 默认不做操作就直接在 ProTable 组件上绑定	:requestApi="getUserList"
const getTableList = (params: any) => {
  let newParams = JSON.parse(JSON.stringify(params));
  return mobileMenuApi.tree(newParams);
};

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | MobileMenu.MobileMenuInfo = {}) {
  console.log("[ proTable.value?.searchParam.module ] >", proTable.value?.searchParam.module);
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable }, proTable.value?.searchParam.module);
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(mobileMenuApi.delete, { ids }, msg);
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
});

/** 更多下拉菜单命令枚举 */
enum cmdEnum {
  ChangeModule = "更改模块",
  Button = "权限按钮"
}

/** 下拉菜单参数接口 */
interface Command {
  row: MobileMenu.MobileMenuInfo;
  command: cmdEnum;
}

/**配置command的参数 */
function command(row: MobileMenu.MobileMenuInfo, command: cmdEnum): Command {
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
    case cmdEnum.ChangeModule:
      //更改模块
      changeModuleFormRef.value?.onOpen({
        moduleOptions: moduleOptions.value,
        id: command.row.id,
        title: command.row.title,
        module: command.row.module,
        successful: () => RefreshTable()
      });
      break;
    case cmdEnum.Button:
      //权限按钮
      buttonFormRef.value?.onOpenTable(command.row.id);
      break;
  }
}
</script>

<style lang="scss" scoped></style>
