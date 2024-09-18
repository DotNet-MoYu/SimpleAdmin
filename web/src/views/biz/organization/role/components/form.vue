<!-- 
 * @Description: 表单
 * @Author: huguodong 
 * @Date: 2023-12-15 15:43:34
!-->
<template>
  <form-container v-model="visible" :title="`${sysRoleProps.opt}角色`" form-size="800px" @close="onClose">
    <el-form
      ref="sysRoleFormRef"
      :rules="rules"
      :disabled="sysRoleProps.disabled"
      :model="sysRoleProps.record"
      :hide-required-asterisk="sysRoleProps.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="角色名称" prop="name">
        <s-input v-model="sysRoleProps.record.name"></s-input>
      </s-form-item>
      <s-form-item label="所属组织" prop="orgId">
        <org-selector v-model:org-value="sysRoleProps.record.orgId!" :org-tree-api="bizOrgApi.tree" :show-all="false" />
      </s-form-item>
      <s-form-item label="默认数据范围" prop="code">
        <DataScopeSelector v-model="sysRoleProps.record.defaultDataScope"></DataScopeSelector>
      </s-form-item>
      <s-form-item label="状态" prop="status">
        <s-radio-group v-model="sysRoleProps.record.status" :options="statusOptions" button />
      </s-form-item>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="sysRoleProps.record.sortCode" show-input :min="1" />
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!sysRoleProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { SysRole, bizRoleApi, bizOrgApi } from "@/api";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
import DataScopeSelector from "./dataScopeSelector.vue";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库

const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS); // 通用状态选项

// 表单参数
const sysRoleProps = reactive<FormProps.Base<SysRole.SysRoleInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  name: [required("请输入角色名称")],
  orgId: [required("请选择所属组织")],
  category: [required("请选择角色类型")],
  status: [required("请选择状态")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysRole.SysRoleInfo>) {
  Object.assign(sysRoleProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    sysRoleProps.record.sortCode = 99;
    sysRoleProps.record.status = statusOptions[0].value;
    sysRoleProps.record.defaultDataScope = SysRole.dataScopeOptions[0];
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    bizRoleApi.detail({ id: props.record.id }).then(res => {
      sysRoleProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const sysRoleFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  sysRoleFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await bizRoleApi
      .submitForm(sysRoleProps.record, sysRoleProps.record.id != undefined)
      .then(() => {
        sysRoleProps.successful!(); //调用父组件的successful方法
      })
      .finally(() => {
        onClose();
      });
  });
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
