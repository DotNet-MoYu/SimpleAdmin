<!-- 用户选择器 -->
<template>
  <form-container v-model="visible" title="用户选择" form-size="90%" v-bind="$attrs">
    <div class="-mt-20px min-h-350px">
      <el-row :gutter="12" justify="space-between">
        <el-col :span="4">
          <el-tabs v-model="activeName" type="card" stretch @tab-click="handleClick">
            <el-tab-pane label="组织" class="ml-5px mr-5px" name="org">
              <el-scrollbar max-height="650px">
                <TreeFilter
                  label="name"
                  class="filterWidth"
                  title="组织列表"
                  :default-expand-all="false"
                  :request-api="getOrgTree"
                  @change="changeOrgTreeFilter"
                />
              </el-scrollbar>
            </el-tab-pane>
            <el-tab-pane label="职位" class="ml-5px mr-5px" name="pos">
              <el-scrollbar max-height="650px">
                <TreeFilter
                  label="name"
                  class="filterWidth"
                  title="职位列表"
                  :default-expand-all="false"
                  :request-api="getPositionTree"
                  @change="changePositionTreeFilter"
                />
              </el-scrollbar>
            </el-tab-pane>
            <el-tab-pane label="角色" class="ml-5px mr-5px" name="role">
              <el-scrollbar max-height="650px">
                <TreeFilter
                  label="name"
                  class="filterWidth"
                  title="角色列表"
                  :default-expand-all="false"
                  :request-api="getRoleTree"
                  @change="changeRoleTreeFilter"
                />
              </el-scrollbar>
            </el-tab-pane>
          </el-tabs>
        </el-col>
        <el-col :span="10">
          <ProTable ref="userTable" :columns="columns" :tool-button="false" :init-param="initParam" :request-api="getUserPage">
            <!-- 表格 header 按钮 -->
            <template #tableHeader="scope">
              <el-button type="primary" @click="addRecords(userTable!.tableData)">添加当前</el-button>
              <el-button type="primary" plain :disabled="!scope.isSelected" @click="addRecords(scope.selectedList)">添加选中</el-button>
            </template>
            <!-- 操作 -->
            <template #operation="scope">
              <el-button type="primary" link :icon="Plus" plain @click="addRecords([scope.row])">添加</el-button>
              <!-- <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" /> -->
            </template>
          </ProTable>
        </el-col>
        <el-col :span="10">
          <ProTable ref="chooseTable" :columns="columns" :tool-button="true" :data="chooseDataTmp" @search="searchRecords" @reset="resetRecords">
            <!-- 表格 header 按钮 -->
            <template #tableHeader="scope">
              <el-button type="danger" @click="delRecords(chooseTable!.tableData)">删除当前</el-button>
              <el-button type="danger" plain :disabled="!scope.isSelected" @click="delRecords(scope.selectedList)">删除选中</el-button>
            </template>
            <template #toolButton>
              <span>已选择:{{ chooseData.length }}人</span>
              <span>,最多选择:{{ props.maxCount }}人</span>
            </template>
            <!-- 操作 -->
            <template #operation="scope">
              <el-button type="danger" link :icon="Delete" plain @click="delRecords([scope.row])">删除</el-button>
            </template>
          </ProTable>
        </el-col>
      </el-row>
    </div>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button type="primary" @click="handleOk"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts" name="UserSelector">
import { SysUser, sysOrgTreeApi, sysUserSelectorApi, SysPosition, SysPositionTreeApi, SysRole, SysRoleTreeApi } from "@/api";
import { UserSelectProps, UserSelectTableInitParams } from "./interface";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { ElMessage, type TabsPaneContext } from "element-plus";
import { Plus, Delete } from "@element-plus/icons-vue";
const activeName = ref("org");
const emit = defineEmits({ successful: null }); // 自定义事件
const handleClick = (tab: TabsPaneContext, event: Event) => {
  console.log(tab, event);
};
const visible = ref(false); //是否显示

// 表单参数
// 定义组件props
const props = withDefaults(defineProps<UserSelectProps>(), {
  permission: false,
  multiple: false,
  maxCount: 1
});

// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<UserSelectTableInitParams>({});
// 获取 userTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const userTable = ref<ProTableInstance>();
// 获取 chooseTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const chooseTable = ref<ProTableInstance>();
// 表格配置项
const columns: ColumnProps<SysUser.SysUserInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "operation", label: "操作", width: 80, fixed: "left" },
  { prop: "account", label: "账号", search: { el: "input", span: 2 } },
  { prop: "name", label: "姓名" }
];

/** 显示选择器 */
function showSelector() {
  visible.value = true;
  chooseDataTmp.value = [];
  chooseData.value = [];
}

/** 关闭选择器 */
function onClose() {
  visible.value = false;
  chooseDataTmp.value = [];
  chooseData.value = [];
}

/** 提交数据 */
function handleOk() {
  visible.value = false;
  ElMessage.success(JSON.stringify(chooseData.value));
  emit("successful", chooseData.value);
}

/** 获取组织树 */
function getOrgTree(): Promise<any> {
  if (props.permission) {
    return sysOrgTreeApi();
  }
  return sysOrgTreeApi();
}

/** 获取组织树 */
function getPositionTree(): Promise<any> {
  if (props.permission) {
    return SysPositionTreeApi();
  }
  return SysPositionTreeApi();
}

/** 获取角色树 */
function getRoleTree(): Promise<any> {
  if (props.permission) {
    return SysRoleTreeApi();
  }
  return SysRoleTreeApi();
}

// 如果你想在请求之前对当前请求参数做一些操作，可以自定义如下函数：params 为当前所有的请求参数（包括分页），最后返回请求列表接口
// 默认不做操作就直接在 ProTable 组件上绑定	:requestApi="getUserList"
/** 获取用户分页 */
function getUserPage(params: any) {
  if (props.permission) {
    return sysUserSelectorApi(params);
  }
  return sysUserSelectorApi(params);
}

/** 部门切换 */
function changeOrgTreeFilter(val: number | string) {
  userTable.value!.pageable.pageNum = 1;
  if (val != "") {
    // 如果传入的val不为空
    initParam.orgId = val;
  } else {
    initParam.orgId = null;
  }
}

/** 职位切换 */
function changePositionTreeFilter(val: number | string, data: SysPosition.SysPositionTree) {
  userTable.value!.pageable.pageNum = 1;
  console.log("[ data ] >", data);
  if (data.isPosition) {
    // 如果是职位
    initParam.positionId = val;
  }
  if (val == "") {
    // 如果传入的val不为空
    initParam.positionId = null;
  }
}

/** 职位切换 */
function changeRoleTreeFilter(val: number | string, data: SysRole.SysRoleTree) {
  userTable.value!.pageable.pageNum = 1;
  console.log("[ data ] >", data);
  if (data.isRole) {
    // 如果是角色
    initParam.roleId = val;
  }
  if (val == "") {
    // 如果传入的val不为空
    initParam.roleId = null;
  }
}

const chooseData = ref<SysUser.SysUserInfo[]>([]); //选择的数据
const chooseDataTmp = ref<SysUser.SysUserInfo[]>([]); //临时选择的数据

/** 添加记录 */
function addRecords(records: any[]) {
  //如果不是多选,判断是否已经添加了
  if (!props.multiple) {
    if (chooseData.value.length > 0 || records.length > 1) {
      ElMessage.warning("只可选择一条");
      return;
    }
    chooseData.value = records;
    chooseDataTmp.value = chooseData.value;
  } else {
    //如果是多选,先判断已添加列表是否有重复,有则过滤掉,没有则直接添加
    records = records.filter(item => !chooseData.value.includes(item));
    if (props.maxCount && props.maxCount < records.length + chooseData.value.length) {
      ElMessage.warning("最多选择" + props.maxCount + "条");
      return;
    }
    chooseData.value = chooseData.value.concat(records); //添加到已选中列表
    chooseDataTmp.value = chooseData.value;
  }
  chooseTable.value?.refresh(); //刷新表格
}

/** 删除记录 */
function delRecords(records: any[]) {
  chooseData.value = chooseData.value.filter(item => !records.includes(item)); //过滤掉已选中的
  chooseDataTmp.value = chooseData.value;
  chooseTable.value?.refresh(); //刷新表格
}

/** 搜索记录 */
function searchRecords() {
  if (chooseTable.value?.searchParam?.account) {
    //搜索account符合的记录
    chooseDataTmp.value = chooseDataTmp.value.filter(item => item.account.includes(chooseTable.value?.searchParam.account)); //过滤掉已选中的
    chooseTable.value?.refresh(); //刷新表格
  }
}

/** 重置记录 */
function resetRecords() {
  chooseDataTmp.value = chooseData.value;
  chooseTable.value?.refresh(); //刷新表格
}
// 暴露方法
defineExpose({ showSelector });
</script>

<style lang="scss" scoped>
.filterWidth {
  width: 100%;
}
:deep(.el-tabs--border-card > .el-tabs__content) {
  padding: 5px;
}
:deep(.table-main) {
  height: 90%;
}
</style>
