<!-- 权限按钮表单页面 -->
<template>
  <el-dialog v-model="visible" :title="`${buttonProps.opt}按钮`" width="500px">
    <el-form
      ref="buttonFormRef"
      :rules="rules"
      :disabled="buttonProps.disabled"
      :model="buttonProps.record"
      :hide-required-asterisk="buttonProps.disabled"
      label-width="auto"
    >
      <s-form-item label="按钮名称" prop="title">
        <el-input v-model="buttonProps.record.title" placeholder="请填写按钮名称" clearable></el-input>
      </s-form-item>
      <s-form-item label="按钮编码" prop="code">
        <el-input v-model="buttonProps.record.code" placeholder="请填写按钮编码" clearable></el-input>
      </s-form-item>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="buttonProps.record.sortCode" show-input :min="1" />
      </s-form-item>
      <s-form-item label="说明" prop="description">
        <el-input v-model="buttonProps.record.description" placeholder="请填写按钮说明" clearable></el-input>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!buttonProps.disabled" primary @click="handleSubmit"> 确定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { buttonDetailApi, buttonSubmitFormApi, Button } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum } from "@/enums";
import { FormInstance } from "element-plus/es/components/form";
const visible = ref(false); //是否显示表单

// 表单参数
const buttonProps = reactive<FormProps.Base<Button.ButtonInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入单页名称")],
  code: [required("请输入按钮编码")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<Button.ButtonInfo>) {
  Object.assign(buttonProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    buttonProps.record.sortCode = 99;
  }
  visible.value = true; //显示表单
  if (props.r.id) {
    //如果传了id，就去请求api获取record
    buttonDetailApi({ id: props.record.id }).then(res => {
      buttonProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const buttonFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  buttonFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await buttonSubmitFormApi(buttonProps.record, buttonProps.record.id != undefined)
      .then(() => {
        buttonProps.successful!(); //调用父组件的successful方法
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
