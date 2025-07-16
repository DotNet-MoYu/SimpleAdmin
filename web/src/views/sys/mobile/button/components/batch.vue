<!-- 
 * @Description: 权限按钮批量表单页面
 * @Author: huguodong 
 * @Date: 2023-12-15 15:42:29
!-->
<template>
  <el-dialog v-model="visible" title="批量新增按钮" width="500px">
    <el-form ref="buttonFormRef" :rules="rules" :model="buttonProps" label-width="auto">
      <s-form-item label="权限简称" prop="title">
        <s-input v-model="buttonProps.title"></s-input>
      </s-form-item>
      <s-form-item label="编码前缀" prop="code">
        <s-input v-model="buttonProps.code"></s-input>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button primary @click="handleSubmit"> 确定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { mobileButtonApi, Button } from "@/api";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus/es/components/form";
const visible = ref(false); //是否显示表单

// 表单参数接口
interface ButtonProps extends Button.Batch {
  /** 成功之后的方法 */
  successful?: () => void;
}

// 表单参数
const buttonProps = reactive<ButtonProps>({
  parentId: 0,
  title: "",
  code: ""
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入权限简称")],
  code: [required("请输入编码前缀")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: ButtonProps) {
  Object.assign(buttonProps, props); //合并参数
  visible.value = true;
}

// 提交数据（新增/编辑）
const buttonFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  buttonFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await mobileButtonApi
      .batch(buttonProps)
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
