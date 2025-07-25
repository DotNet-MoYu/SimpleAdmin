﻿<!-- 
 * @Description: 模块管理表单页面
 * @Author: superAdmin  
 * @Date: 2025-06-25 14:37:52
!-->

<template>
  <form-container v-model="visible" :title="`${moduleProps.opt}模块`" form-size="600px">
    <el-form
      ref="moduleFormRef"
      :rules="rules"
      :disabled="moduleProps.disabled"
      :model="moduleProps.record"
      :hide-required-asterisk="moduleProps.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="名称" prop="title">
        <s-input v-model="moduleProps.record.title"></s-input>
      </s-form-item>
      <s-form-item label="图标" prop="icon">
        <SelectIconPlus v-model:icon-value="moduleProps.record.icon!" />
      </s-form-item>
      <s-form-item label="颜色" prop="color">
        <color-picker v-model:value="moduleProps.record.color" />
      </s-form-item>
      <s-form-item label="描述" prop="description">
        <s-input v-model="moduleProps.record.description"></s-input>
      </s-form-item>
      <s-form-item label="排序码" prop="sortCode">
        <el-slider v-model="moduleProps.record.sortCode" show-input :min="1" />
      </s-form-item>
      <s-form-item label="状态" prop="status">
        <s-radio-group v-model="moduleProps.record.status" :options="statusOptions" button />
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!moduleProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { mobileModuleApi, MobileModule } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";

const dictStore = useDictStore(); //字典仓库
const visible = ref(false); //

// 通用状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);

// 表单参数
const moduleProps = reactive<FormProps.Base<MobileModule.MobileModuleInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入显示名称")],
  sortCode: [required("请输入排序")],
  icon: [required("请选择图标")],
  status: [required("请选择状态")]
});
/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<MobileModule.MobileModuleInfo>) {
  Object.assign(moduleProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    moduleProps.record.sortCode = 99;
    moduleProps.record.status = statusOptions[0].value;
    moduleProps.record.color = "#1890ff"; //默认颜色
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    mobileModuleApi.detail({ id: props.record.id }).then(res => {
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
    await mobileModuleApi
      .submitForm(moduleProps.record, moduleProps.record.id != undefined)
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
