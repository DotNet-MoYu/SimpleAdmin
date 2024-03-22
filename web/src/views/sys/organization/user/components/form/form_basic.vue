<!-- 
 * @Description: 基本信息
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:50
!-->
<template>
  <div>
    <el-tab-pane label="基础信息" name="basic">
      <el-row :gutter="16">
        <el-col :span="12">
          <s-form-item label="账号" prop="account">
            <s-input v-model="userInfo.account"></s-input>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="姓名" prop="name">
            <s-input v-model="userInfo.name"></s-input>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="16">
        <el-col :span="12">
          <s-form-item label="性别" prop="gender">
            <s-radio-group v-model="userInfo.gender" :options="genderOptions" />
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="昵称" prop="nickname">
            <s-input v-model="userInfo.nickname"></s-input>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="16">
        <el-col :span="12">
          <s-form-item label="手机号" prop="phone">
            <s-input v-model="userInfo.phone"></s-input>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="邮箱" prop="email">
            <s-input v-model="userInfo.email"></s-input>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="16">
        <el-col :span="12">
          <s-form-item label="出生日期" prop="birthday">
            <date-picker v-model="userInfo.birthday"></date-picker>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="入职日期" prop="entryDate">
            <date-picker v-model="userInfo.entryDate"></date-picker>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="16">
        <el-col :span="12">
          <s-form-item label="员工编号" prop="empNo">
            <s-input v-model="userInfo.empNo"></s-input>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="职级" prop="positionLevel">
            <s-input v-model="userInfo.positionLevel" clearable></s-input>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="16">
        <el-col :span="12">
          <s-form-item label="部门和职位" prop="positionId">
            <position-selector v-model="userInfo.orgAndPosIdList" @change="handleChange" />
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="主管" prop="directorId">
            <el-button link type="primary" @click="showSelector">选择</el-button>
            <el-tag v-if="userInfo.directorId" class="ml-3px" type="warning" closable @close="removeDirector">{{
              userInfo.directorInfo?.name
            }}</el-tag>
          </s-form-item>
        </el-col>
      </el-row>
    </el-tab-pane>
    <user-selector
      ref="userSelectorRef"
      :org-tree-api="sysOrgApi.tree"
      :position-tree-api="sysPositionApi.tree"
      :role-tree-api="sysRoleApi.tree"
      :user-selector-api="sysUserApi.selector"
      @successful="handleChooseUser"
    />
  </div>
</template>

<script setup lang="ts">
import { SysUser, sysOrgApi, sysPositionApi, sysRoleApi, sysUserApi } from "@/api";
import { useDictStore } from "@/stores/modules";
import { SysDictEnum } from "@/enums";
import { UserSelectorInstance } from "@/components/Selectors/UserSelector/interface";

const dictStore = useDictStore(); //字典仓库
// props
interface FormProps {
  modelValue: Partial<SysUser.SysUserInfo>;
}
const emit = defineEmits(["update:modelValue"]); //定义emit
const props = defineProps<FormProps>(); //定义props
// 用户信息
const userInfo = computed({
  get: () => props.modelValue,
  set: val => emit("update:modelValue", val)
});

const genderOptions = dictStore.getDictList(SysDictEnum.GENDER); //性别选项
onMounted(() => {
  // 初始化
  userInfo.value.gender = userInfo.value.gender ? userInfo.value.gender : genderOptions[0].value;
});

/** 用于处理职位选择器改变值的情况 */
function handleChange(value: any) {
  if (!value) {
    return;
  }
  // 获取改变后的值中最后一位的值，即positionId
  let positionId = value[value.length - 1];
  // 获取改变后的值中最后两位的值，即orgId
  let orgId = value[value.length - 2];
  // 将orgId赋值给userInfo的value的orgId
  userInfo.value.orgId = orgId;
  // 将positionId赋值给userInfo的value的positionId
  userInfo.value.positionId = positionId;
}

const userSelectorRef = ref<UserSelectorInstance>(); //用户选择器ref
/** 显示用户选择器 */
function showSelector() {
  //将userInfo.value.directorInfo转为 SysUser.SysUserInfo[]类型
  const directorInfo = userInfo.value.directorInfo ? [userInfo.value.directorInfo] : [];
  userSelectorRef.value?.showSelector(directorInfo);
}

/** 选择用户 */
function handleChooseUser(data: SysUser.SysUserInfo[]) {
  // 选择用户后，将用户id赋值给userInfo.value.directorId
  if (data.length > 0) {
    userInfo.value.directorId = data[0].id;
    userInfo.value.directorInfo = data[0];
  }
}

/** 移除主管 */
function removeDirector() {
  userInfo.value.directorId = null;
  userInfo.value.directorInfo = null;
}
</script>

<style lang="scss" scoped>
:deep(.el-input__wrapper) {
  width: 100% !important;
}
:deep(.el-date-editor.el-input) {
  width: 92% !important;
}
</style>
