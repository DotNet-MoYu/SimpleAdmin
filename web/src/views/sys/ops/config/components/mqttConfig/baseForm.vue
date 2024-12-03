<!-- 
 * @Description:连接配置 
 * @Author: huguodong 
 * @Date: 2024-11-07 13:55:56
!-->
<template>
  <el-form ref="baseFormRef" :rules="rules" :model="baseFormProps" label-width="auto" label-suffix=" :" label-position="top">
    <s-form-item label="MQTT服务端地址" prop="MQTT_PARAM_URL">
      <s-input v-model="baseFormProps.MQTT_PARAM_URL"></s-input>
    </s-form-item>
    <s-form-item label="MQTT用户名" prop="MQTT_PARAM_USERNAME">
      <s-input v-model="baseFormProps.MQTT_PARAM_USERNAME"></s-input>
    </s-form-item>
    <s-form-item label="MQTT密码" prop="MQTT_PARAM_PASSWORD">
      <s-input v-model="baseFormProps.MQTT_PARAM_PASSWORD"></s-input>
    </s-form-item>
    <el-form-item>
      <el-button type="primary" :loading="submitLoading" @click="onSubmit()">保存</el-button>
      <el-button style="margin-left: 10px" @click="resetForm">重置</el-button>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { SysConfig, sysConfigApi } from "@/api";
import { FormInstance } from "element-plus";
const submitLoading = ref(false); //提交按钮loading
const baseFormRef = ref<FormInstance>();
import { SysConfigTypeEnum } from "@/enums";

/**
 * @description: 连接配置参数
 */
const baseFormProps = reactive<SysConfig.MqttPolicyConfig>({
  MQTT_PARAM_URL: "",
  MQTT_PARAM_USERNAME: "",
  MQTT_PARAM_PASSWORD: ""
});

//props定义
const props = defineProps({
  baseConfig: {
    type: Array as PropType<SysConfig.ConfigInfo[]>,
    required: true,
    default: () => []
  }
});

//监听loginPolicy变化
watch(
  () => props.baseConfig,
  (newVal: SysConfig.ConfigInfo[]) => {
    //重新赋值
    newVal.forEach((item: SysConfig.ConfigInfo) => {
      (baseFormProps[item.configKey as keyof SysConfig.MqttPolicyConfig] as string) = item.configValue;
    });
  }
);

// 表单验证规则
const rules = reactive({
  MQTT_PARAM_URL: [{ required: true, message: "请输入MQTT服务端地址", trigger: "blur" }],
  MQTT_PARAM_USERNAME: [{ required: true, message: "请输入MQTT用户名", trigger: "blur" }],
  MQTT_PARAM_PASSWORD: [{ required: true, message: "请输入MQTT密码", trigger: "blur" }]
});

/** 提交表单 */
function onSubmit() {
  baseFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    submitLoading.value = true;
    //组装参数
    const param: SysConfig.ConfigInfo[] = Object.entries(baseFormProps).map(item => {
      return {
        id: 0,
        category: SysConfigTypeEnum.MQTT_POLICY,
        configKey: item[0],
        configValue: typeof item[1] === "object" ? JSON.stringify(item[1]) : String(item[1])
      };
    });
    //提交数据
    sysConfigApi.configEditForm(param).finally(() => {
      submitLoading.value = false;
    });
  });
}
/** 重置表单 */
function resetForm() {
  baseFormRef.value?.resetFields();
}
</script>

<style lang="scss" scoped></style>
