<!-- 模块管理表单页面 -->
<template>
  <form-container v-model="visible" :title="`${moduleProps.opt}模块`" form-size="600px">
    <el-form
      ref="moduleFormRef"
      :rules="rules"
      :disabled="moduleProps.disabled"
      :model="moduleProps.record"
      :hide-required-asterisk="moduleProps.disabled"
      label-width="auto"
      class="-mt-25px"
    >
      <s-form-item label="模块名称" prop="title">
        <el-input v-model="moduleProps.record.title" placeholder="请填写模块名称" clearable></el-input>
      </s-form-item>

      <s-form-item label="图标" prop="icon">
        <SelectIconPlus v-model:icon-value="moduleProps.record.icon" />
      </s-form-item>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="moduleProps.record.sortCode" show-input :min="1" />
      </s-form-item>
      <s-form-item label="说明" prop="description">
        <el-input v-model="moduleProps.record.description" placeholder="请填写模块说明" clearable></el-input>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!moduleProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { moduleDetailApi, moduleSubmitFormApi, Module } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum } from "@/enums";
import { FormInstance } from "element-plus";
const visible = ref(false); //是否显示表单

// 表单参数
const moduleProps = reactive<FormProps.Base<Module.ModuleInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入单页名称")],
  menuType: [required("请选择单页类型")],
  path: [required("请输入路由地址")],
  name: [required("请输入组件名称")],
  component: [required("请输入组件地址")],
  sortCode: [required("请输入排序")],
  icon: [required("请选择图标")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<Module.ModuleInfo>) {
  Object.assign(moduleProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    moduleProps.record.sortCode = 99;
  }

  visible.value = true; //显示表单
  if (props.id) {
    //如果传了id，就去请求api获取record
    moduleDetailApi({ id: props.id }).then(res => {
      moduleProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const moduleFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  moduleFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await moduleSubmitFormApi(moduleProps.record, moduleProps.record.id != undefined)
      .then(() => {
        moduleProps.successful!(); //调用父组件的successful方法
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
