<!-- 
 * @Description: 用户管理
 * @Author: huguodong 
 * @Date: 2023-12-15 15:46:03
!-->
<template>
  <div class="main-box">
    <TreeFilter
      ref="treeFilter"
      label="name"
      title="组织列表"
      :request-api="sysOrgApi.tree"
      :default-value="initParam.orgId"
      @change="changeTreeFilter"
    />
    <div class="table-box">
      <ProTable ref="proTable" title="用户列表" :columns="columns" :request-api="sysUserApi.page" :init-param="initParam">
        <!-- 表格 header 按钮 -->
        <template #tableHeader="scope">
          <s-button suffix="用户" @click="onOpen(FormOptEnum.ADD)" />
          <s-button
            type="danger"
            plain
            suffix="用户"
            :opt="FormOptEnum.DELETE"
            :disabled="!scope.isSelected"
            @click="onDelete(scope.selectedListIds, '删除所选用户')"
          />
        </template>
        <!-- 状态 -->
        <template #avatar="scope">
          <el-avatar :src="scope.row.avatar" :size="30" />
          <!-- <span>{{ scope.row.avatar }}}</span> -->
        </template>
        <!-- 状态 -->
        <template #status="scope">
          <el-switch :model-value="scope.row.status === CommonStatusEnum.ENABLE" :loading="switchLoading" @change="editStatus(scope.row)" />
        </template>
        <!-- 操作 -->
        <template #operation="scope">
          <el-space>
            <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
            <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.account}】用户`)" />
            <el-dropdown @command="handleCommand">
              <el-link type="primary" :underline="false" :icon="ArrowDown"> 更多 </el-link>
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
      <!-- 角色选择器组件 -->
      <role-selector
        ref="roleSelectorRef"
        multiple
        :org-tree-api="sysOrgApi.tree"
        :role-selector-api="sysRoleApi.roleSelector"
        @successful="handleChooseRole"
      ></role-selector>
      <!-- 授权资源组件 -->
      <GrantResource user ref="grantResourceRef" />
      <!-- 授权权限组件 -->
      <GrantPermission user ref="grantPermissionRef" />
    </div>
  </div>
</template>

<script setup lang="ts" name="SysUser">
import { sysOrgApi, sysUserApi, SysUser, SysRole, sysRoleApi } from "@/api";
import { ArrowDown } from "@element-plus/icons-vue";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { SysDictEnum, FormOptEnum, CommonStatusEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import TreeFilter from "@/components/TreeFilter/index.vue";
import Form from "./components/form/index.vue";
import { ElMessage } from "element-plus";
import { RoleSelectorInstance } from "@/components/Selectors/RoleSelector/interface";
import GrantResource from "../../limit/role/components/grantResource.vue";
import GrantPermission from "../../limit/role/components/grantPermission.vue";
const dictStore = useDictStore(); //字典仓库

// 状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);
const treeFilter = ref<InstanceType<typeof TreeFilter> | null>(null);
//状态开关loading
const switchLoading = ref(false);

interface InitParam {
  orgId: number | string;
}
// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<InitParam>({ orgId: 0 });
// 获取 ProTable 元素
const proTable = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<SysUser.SysUserInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "searchKey", label: "姓名或账号", search: { el: "input" }, isShow: false },
  { prop: "avatar", label: "头像", width: 80 },
  { prop: "account", label: "账号" },
  { prop: "name", label: "姓名" },
  { prop: "gender", label: "性别", width: 80 },
  { prop: "phone", label: "手机号" },
  { prop: "orgName", label: "组织" },
  { prop: "positionName", label: "职位" },
  {
    prop: "status",
    label: "状态",
    width: 90,
    enum: statusOptions,
    search: { el: "tree-select" }
  },
  { prop: "createTime", label: "创建时间" },
  { prop: "operation", label: "操作", width: 230, fixed: "right" }
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
function onOpen(opt: FormOptEnum, record: {} | SysUser.SysUserInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(sysUserApi.delete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh(); //刷新表格
  treeFilter.value?.refresh(); //刷新树形筛选器
}

/** 更多下拉菜单命令枚举 */
enum cmdEnum {
  ResetPassword = "重置密码",
  GrantRole = "授权角色",
  GrantResource = "授权资源",
  GrantPermission = "授权权限"
}

/** 下拉菜单参数接口 */
interface Command {
  row: SysUser.SysUserInfo;
  command: cmdEnum;
}

/**配置command的参数 */
function command(row: SysUser.SysUserInfo, command: cmdEnum): Command {
  return {
    row: row,
    command: command
  };
}

const roleSelectorRef = ref<RoleSelectorInstance>(); //角色选择器引用
const userId = ref<number | string>(0); //用户id
const grantResourceRef = ref<InstanceType<typeof GrantResource> | null>(null); //授权资源组件引用
const grantPermissionRef = ref<InstanceType<typeof GrantPermission> | null>(null); //授权权限组件引用
/**
 * 更多下拉菜单点击事件
 * @param command
 */
function handleCommand(command: Command) {
  switch (command.command) {
    case cmdEnum.ResetPassword:
      useHandleData(sysUserApi.resetPassword, { id: command.row.id }, cmdEnum.ResetPassword);
      break;
    case cmdEnum.GrantRole:
      userId.value = command.row.id; //获取用户id
      sysUserApi.ownRole({ id: command.row.id }).then(res => {
        roleSelectorRef.value?.showSelector(res.data); //显示用户选择器
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
    case cmdEnum.GrantPermission:
      // 打开授权权限组件
      grantPermissionRef.value?.onOpen({
        opt: FormOptEnum.EDIT,
        record: command.row,
        successful: RefreshTable
      });
      break;
  }
}

/** 选择角色 */
function handleChooseRole(data: SysRole.SysRoleInfo[]) {
  //组装参数
  const grantUser: SysUser.GrantRoleReq = {
    id: userId.value,
    roleIdList: data.map(item => item.id) as number[] | string[]
  };
  sysUserApi.grantRole(grantUser); //授权角色
}

/**
 * 修改状态
 * @param row  当前行数据
 */
function editStatus(row: SysUser.SysUserInfo) {
  switchLoading.value = true;
  const status = row.status === CommonStatusEnum.ENABLE ? CommonStatusEnum.DISABLE : CommonStatusEnum.ENABLE; // 状态取反
  sysUserApi.updateStatus({ id: row.id }, status).then(() => {
    switchLoading.value = false;
    ElMessage.success("修改成功");
    row.status = status;
  });
}
</script>

<style lang="scss" scoped></style>
