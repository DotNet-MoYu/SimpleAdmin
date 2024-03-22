<!-- 
 * @Description: 配置表单
 * @Author: huguodong 
 * @Date: 2024-02-02 10:49:28
!-->
<template>
  <form-container v-model="visible" :title="`${configProps.opt}配置`" form-size="600px">
    <el-form
      ref="configFormRef"
      :rules="rules"
      :disabled="configProps.disabled"
      :model="configProps.record"
      :hide-required-asterisk="configProps.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="配置键" prop="configKey">
        <s-input v-model="configProps.record.configKey"></s-input>
      </s-form-item>
      <s-form-item label="配置值" prop="configValue">
        <s-input v-model="configProps.record.configValue"></s-input>
      </s-form-item>
      <s-form-item label="备注" prop="remark">
        <s-input v-model="configProps.record.remark"></s-input>
      </s-form-item>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="configProps.record.sortCode" show-input :min="1" />
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!configProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { SysConfig, sysConfigApi } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum } from "@/enums";
import { FormInstance } from "element-plus";

const visible = ref(false); //是否显示表单

// 表单参数接口
interface ConfigProps extends FormProps.Base<SysConfig.ConfigInfo> {}

// 表单参数
const configProps = reactive<ConfigProps>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  configKey: [required("请输入配置键")],
  configValue: [required("请输入配置值")],
  sortCode: [required("请输入排序")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: ConfigProps) {
  Object.assign(configProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    configProps.record.sortCode = 99;
  }
  visible.value = true; //显示表单
}

// 提交数据（新增/编辑）
const configFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  configFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysConfigApi
      .submitForm(configProps.record, configProps.record.id != undefined)
      .then(() => {
        configProps.successful!(); //调用父组件的successful方法
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
