<!-- 
 * @Description: 角色选择器
 * @Author: huguodong 
 * @Date: 2023-12-15 15:40:39
!-->
<template>
  <form-container v-model="visible" title="角色选择" form-size="90%" v-bind="$attrs">
    <div class="-mt-15px min-h-350px">
      <el-row :gutter="12" justify="space-between">
        <el-col :span="4">
          <el-scrollbar max-height="650px">
            <TreeFilter
              label="name"
              class="filterWidth"
              title="组织列表"
              top-name="全局"
              :default-expand-all="false"
              :request-api="orgTreeApi"
              @change="changeOrgTreeFilter"
          /></el-scrollbar>
        </el-col>
        <el-col :span="10">
          <ProTable ref="userTable" :columns="columns" :tool-button="false" :init-param="initParam" :request-api="roleSelectorApi">
            <!-- 表格 header 按钮 -->
            <template #tableHeader="scope">
              <el-button type="primary" @click="addRecords(userTable!.tableData)">添加当前</el-button>
              <el-button type="primary" plain :disabled="!scope.isSelected" @click="addRecords(scope.selectedList)">添加选中</el-button>
            </template>
            <!-- 操作 -->
            <template #operation="scope">
              <el-button type="primary" link :icon="Plus" plain @click="addRecords([scope.row])">添加</el-button>
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
              <span v-if="maxCount">,最多选择:{{ maxCount }}人</span>
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

<script setup lang="ts" name="RoleSelector">
import { SysRole } from "@/api";
import { RoleSelectProps, RoleSelectTableInitParams } from "./interface";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { ElMessage } from "element-plus";
import { Plus, Delete } from "@element-plus/icons-vue";

const emit = defineEmits({ successful: null }); // 自定义事件

const visible = ref(false); //是否显示

// 定义组件props
const props = withDefaults(defineProps<RoleSelectProps>(), {
  permission: false,
  multiple: false
});

// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<RoleSelectTableInitParams>({});
// 获取 userTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const userTable = ref<ProTableInstance>();
// 获取 chooseTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const chooseTable = ref<ProTableInstance>();
// 表格配置项
const columns: ColumnProps<SysRole.SysRoleInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "operation", label: "操作", width: 80, fixed: "left" },
  { prop: "name", label: "角色名", search: { el: "input", span: 2 } }
];

/** 显示选择器 */
function showSelector(data: SysRole.SysRoleInfo[] = []) {
  visible.value = true;
  chooseDataTmp.value = data;
  chooseData.value = data;
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
  emit("successful", chooseData.value);
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

const chooseData = ref<SysRole.SysRoleInfo[]>([]); //选择的数据
const chooseDataTmp = ref<SysRole.SysRoleInfo[]>([]); //临时选择的数据

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
    //如果是多选,先判断已添加列表是否有重复的id,有则过滤掉,没有则直接添加
    records = records.filter(item => !chooseData.value.find(it => it.id == item.id));
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
  if (chooseTable.value?.searchParam?.name) {
    //搜索name符合的记录
    chooseDataTmp.value = chooseDataTmp.value.filter(item => item.name.includes(chooseTable.value?.searchParam.name)); //过滤掉已选中的
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
