<!-- 
 * @Description: 角色管理
 * @Author: huguodong 
 * @Date: 2023-12-15 15:43:53
!-->
<template>
  <div class="main-box">
    <TreeFilter
      ref="treeFilter"
      label="name"
      title="组织列表"
      :show-all="false"
      :request-api="bizOrgApi.tree"
      :default-value="initParam.orgId"
      @change="changeTreeFilter"
    />
    <div class="table-box">
      <ProTable ref="proTable" :columns="columns" :request-api="bizRoleApi.page" :init-param="initParam">
        <!-- 表格 header 按钮 -->
        <template #tableHeader="scope">
          <s-button v-auth="bizRoleButtonCode.add" suffix="角色" @click="onOpen(FormOptEnum.ADD)" />
          <s-button
            v-auth="bizRoleButtonCode.batchDelete"
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
            <s-button v-auth="bizRoleButtonCode.edit" link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
            <s-button
              v-auth="bizRoleButtonCode.delete"
              link
              :opt="FormOptEnum.DELETE"
              @click="onDelete([scope.row.id], `删除【${scope.row.name}】角色`)"
            />
            <el-dropdown @command="handleCommand">
              <el-link type="primary" :underline="false" :icon="ArrowDown"> 授权 </el-link>
              <template #dropdown>
                <el-dropdown-menu>
                  <div v-auth="bizRoleButtonCode.grantResource">
                    <el-dropdown-item :command="command(scope.row, cmdEnum.GrantResource)">{{ cmdEnum.GrantResource }}</el-dropdown-item>
                  </div>
                  <div v-auth="bizRoleButtonCode.grantUser">
                    <el-dropdown-item :command="command(scope.row, cmdEnum.GrantUser)">{{ cmdEnum.GrantUser }}</el-dropdown-item>
                  </div>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </el-space>
        </template>
      </ProTable>
      <!-- 新增/编辑表单 -->
      <Form ref="formRef" />
      <!-- 授权资源组件 -->
      <GrantResource ref="grantResourceRef" />
      <!-- 用户选择器 -->
      <user-selector
        ref="userSelectorRef"
        biz
        :org-tree-api="bizOrgApi.tree"
        :position-tree-api="bizPositionApi.tree"
        :role-tree-api="bizRoleApi.tree"
        :user-selector-api="bizUserApi.selector"
        multiple
        @successful="handleChooseUser"
      />
    </div>
  </div>
</template>

<script setup lang="ts" name="sysRole">
import { bizOrgApi, bizRoleApi, SysRole, SysUser, bizUserApi, bizPositionApi, bizRoleButtonCode } from "@/api";
import { ArrowDown } from "@element-plus/icons-vue";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { SysDictEnum, FormOptEnum, CommonStatusEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import TreeFilter from "@/components/TreeFilter/index.vue";
import Form from "./components/form.vue";
import GrantResource from "./components/grantResource.vue";
import { UserSelectorInstance } from "@/components/Selectors/UserSelector/interface";

const dictStore = useDictStore(); //字典仓库

// 状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);
// 组织列表组件
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
  await useHandleData(bizRoleApi.delete, { ids }, msg);
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
  GrantUser = "授权用户"
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

const grantResourceRef = ref<InstanceType<typeof GrantResource> | null>(null); //授权资源组件引用
const userSelectorRef = ref<UserSelectorInstance>(); //用户选择器引用
const roleId = ref<number | string>(0); //角色id

/**
 * 更多下拉菜单点击事件
 * @param command
 */
function handleCommand(command: Command) {
  switch (command.command) {
    case cmdEnum.GrantUser:
      roleId.value = command.row.id; //获取角色id
      bizRoleApi.ownUser({ id: command.row.id }).then(res => {
        userSelectorRef.value?.showSelector(res.data); //显示用户选择器
      });
      break;
    case cmdEnum.GrantResource:
      // 打开授权资源组件
      grantResourceRef.value?.onOpen({
        opt: FormOptEnum.EDIT,
        record: command.row,
        successful: RefreshTable
      });
      break;
  }
}

/** 选择用户 */
function handleChooseUser(data: SysUser.SysUserInfo[]) {
  //组装参数
  const grantUser: SysRole.GrantUserReq = {
    id: roleId.value,
    grantInfoList: data.map(item => item.id) as number[] | string[]
  };
  bizRoleApi.grantUser(grantUser);
}
</script>

<style lang="scss" scoped></style>
