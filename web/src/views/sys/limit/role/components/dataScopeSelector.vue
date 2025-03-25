<!-- 
 * @Description: 数据范围选择器
 * @Author: huguodong 
 * @Date: 2023-12-15 15:43:27
!-->
<template>
  <div>
    <el-space>
      <s-radio-group v-model="defaultLevel" :options="scopeOptions" value-key="level" label-key="title" @change="changeDataScope">
        <template #radio="{ item }">
          <el-badge
            v-if="
              dataScope.level === defineScope?.level && item.scopeCategory === SysRole.DataScopeEnum.SCOPE_ORG_DEFINE && item.scopeDefineOrgIdList
            "
            :value="dataScope.scopeDefineOrgIdList.length"
            type="success"
          >
            <span class="mb-5">{{ item.title }}</span>
          </el-badge>
          <span v-else>{{ item.title }}</span>
        </template>
      </s-radio-group>
      <div v-if="dataScope.level === defineScope?.level">
        <el-button link type="primary" class="ml-15px" @click="handleDefineOrg">选择组织</el-button>
      </div>
    </el-space>
    <el-dialog v-model="dialogVisible" append-to-body title="选择组织" width="400" destroy-on-close>
      <TreeFilter
        title="组织列表"
        multiple
        check-strictly
        class="treeFilter"
        label="name"
        :request-api="sysOrgApi.tree"
        :default-value="dataScope.scopeDefineOrgIdList"
        @change="changeTreeFilter"
      />
      <template #footer>
        <el-button @click="dialogCancel">取消</el-button>
        <el-button type="primary" @click="chooseDefineOrg"> 确定 </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { SysRole, sysOrgApi } from "@/api";

const dialogVisible = ref(false); //是否显示弹窗

// props
interface DataScopeProps {
  modelValue: SysRole.DefaultDataScope | undefined;
}
const emit = defineEmits(["update:modelValue"]); //定义emit
const props = defineProps<DataScopeProps>(); //定义props

// 用户信息
const dataScope = computed({
  get: () => props.modelValue ?? scopeOptions[0], // 判断是否有值，没有值则取第一个
  set: val => emit("update:modelValue", val)
});

// 数据范围列表
const scopeOptions = SysRole.dataScopeOptions;

// 默认数据范围
const defaultLevel = ref(dataScope.value.level);

// 自定义数据范围
const defineScope = SysRole.dataScopeOptions.find(item => item.scopeCategory === SysRole.DataScopeEnum.SCOPE_ORG_DEFINE);

// 自定义组织id列表
const defineOrgIdList = ref<number[] | string[]>(dataScope.value.scopeDefineOrgIdList);

/** 修改数据权限 */
function changeDataScope(value: any) {
  let scope = SysRole.dataScopeOptions.find(item => item.level === value);
  if (scope) {
    dataScope.value = scope;
  }
}

// /** 处理自定义 */
function handleDefineOrg() {
  dialogVisible.value = true;
  console.log("[ dataScope.value ] >", dataScope.value);
}

/** 组织树数据变化 */
function changeTreeFilter(val: any[]) {
  defineOrgIdList.value = val;
}

/** 取消选择组织 */
function dialogCancel() {
  // 关闭弹窗
  dialogVisible.value = false;
}

/** 选择机构 */
function chooseDefineOrg() {
  // 设置默认数据权限中的机构ID列表
  // 关闭弹窗
  dialogVisible.value = false;
  dataScope.value.scopeDefineOrgIdList = defineOrgIdList.value;
  console.log("[  dataScope.value ] >", dataScope.value);
}
</script>

<style lang="scss" scoped>
.treeFilter {
  width: 100% !important;
}
:deep(.el-badge__content.is-fixed) {
  transform: translateY(-30%) translateX(100%);
}
</style>
