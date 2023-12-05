<!--角色管理 -->
<template>
  <div class="main-box">
    <TreeFilter
      ref="treeFilter"
      label="name"
      title="组织列表"
      :request-api="sysOrgApi.sysOrgTree"
      :default-value="initParam.orgId"
      @change="changeTreeFilter"
    />
    <div class="table-box">
      <ProTable ref="proTable" :columns="columns" :request-api="sysRoleApi.sysRolePage" :init-param="initParam">
        <!-- 表格 header 按钮 -->
        <template #tableHeader="scope">
          <s-button suffix="角色" @click="onOpen(FormOptEnum.ADD)" />
          <s-button
            type="danger"
            plain
            suffix="角色"
            :opt="FormOptEnum.DELETE"
            :disabled="!scope.isSelected"
            @click="onDelete(scope.selectedListIds, '删除所选角色')"
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
          <el-space>
            <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
            <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.name}】角色`)" />
            <el-dropdown @command="handleCommand">
              <el-link type="primary" :underline="false" :icon="ArrowDown"> 授权 </el-link>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item v-for="(item, index) in cmdEnum" :key="index" :command="command(scope.row, item)">{{ item }}</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </el-space>
        </template>
      </ProTable>
      <!-- 新增/编辑表单 -->
      <Form ref="formRef" />
      <!-- 授权资源表单 -->
      <GrantResourceForm ref="grantResourceFormRef" />
    </div>
  </div>
</template>

<script setup lang="ts" name="sysRole">
import { sysOrgApi, sysRoleApi, SysRole } from "@/api";
import { ArrowDown } from "@element-plus/icons-vue";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { SysDictEnum, FormOptEnum, CommonStatusEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import TreeFilter from "@/components/TreeFilter/index.vue";
import Form from "./components/form.vue";
import GrantResourceForm from "./components/grantResourceForm.vue";

const dictStore = useDictStore(); //字典仓库
// 角色类型选项
const roleCategoryOptions = dictStore.getDictList(SysDictEnum.ROLE_CATEGORY);
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
const columns: ColumnProps<SysRole.SysRoleInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "name", label: "角色名称", search: { el: "input" } },
  {
    prop: "category",
    label: "角色分类",
    enum: roleCategoryOptions,
    search: { el: "tree-select" }
  },
  { prop: "code", label: "角色编码", search: { el: "input" } },
  { prop: "status", label: "状态", enum: statusOptions, search: { el: "tree-select" } },
  { prop: "sortCode", label: "排序" },
  { prop: "createTime", label: "创建时间" },
  { prop: "operation", label: "操作", width: 230, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | SysRole.SysRoleInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(sysRoleApi.sysRoleDelete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh(); //刷新表格
  treeFilter.value?.refresh(); //刷新树形筛选器
}

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

/** 更多下拉菜单命令枚举 */
enum cmdEnum {
  GrantResource = "授权资源",
  GrantPermission = "授权权限",
  GrantUser = "授权角色"
}

/** 下拉菜单参数接口 */
interface Command {
  row: SysRole.SysRoleInfo;
  command: cmdEnum;
}

/**配置command的参数 */
function command(row: SysRole.SysRoleInfo, command: cmdEnum): Command {
  return {
    row: row,
    command: command
  };
}

const grantResourceFormRef = ref<InstanceType<typeof GrantResourceForm> | null>(null); //授权资源表单引用
/**
 * 更多下拉菜单点击事件
 * @param command
 */
function handleCommand(command: Command) {
  switch (command.command) {
    case cmdEnum.GrantUser:
      break;
    case cmdEnum.GrantResource:
      // 打开授权资源表单
      grantResourceFormRef.value?.onOpen({
        opt: FormOptEnum.EDIT,
        record: command.row,
        successful: RefreshTable
      });
      break;
    case cmdEnum.GrantPermission:
      break;
  }
}
</script>

<style lang="scss" scoped></style>
