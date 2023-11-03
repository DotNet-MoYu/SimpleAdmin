<!-- 复制 -->
<template>
  <form-container v-model="visible" title="复制组织" form-size="600px">
    <el-form
      ref="orgFormRef"
      :rules="rules"
      :disabled="orgProps.disabled"
      :model="orgProps.record"
      :hide-required-asterisk="orgProps.disabled"
      label-width="auto"
    >
      <s-form-item label="目标组织" prop="parentId">
        <org-selector v-model:org-value="orgProps.record.parentId" />
      </s-form-item>
      <s-form-item label="包含子集" prop="isKeepAlive">
        <el-radio-group v-model="orgProps.record.parentId">
          <el-radio-button v-for="(item, index) in yesOptions" :key="index" :label="item.value">{{ item.label }}</el-radio-button>
        </el-radio-group>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!orgProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { SysOrg, sysOrgDetailApi, sysOrgSubmitFormApi } from "@/api";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库
// 是否选项
const yesOptions = dictStore.getDictList(SysDictEnum.YES_NO);

// 表单参数
const orgProps = reactive<FormProps.Base<SysOrg.SysOrgInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  name: [required("请输入组织名称")],
  parentId: [required("请选择上级组织")],
  category: [required("请选择组织类型")],
  status: [required("请选择状态")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysOrg.SysOrgInfo>) {
  Object.assign(orgProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    orgProps.record.sortCode = 99;
    orgProps.record.category = orgCategoryOptions[0].value;
    orgProps.record.status = statusOptions[0].value;
  }

  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    sysOrgDetailApi({ id: props.record.id }).then(res => {
      orgProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const orgFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  orgFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysOrgSubmitFormApi(orgProps.record, orgProps.record.id != undefined)
      .then(() => {
        orgProps.successful!(); //调用父组件的successful方法
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
