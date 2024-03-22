<!-- 
 * @Description: 登录策略
 * @Author: huguodong 
 * @Date: 2024-01-31 16:25:48
!-->
<template>
  <el-form ref="loginFormRef" :rules="rules" :model="loginFormProps" label-width="auto" label-suffix=" :" label-position="top">
    <s-form-item label="单用户登录开关" prop="LOGIN_SINGLE_OPEN">
      <s-radio-group v-model="loginFormProps.LOGIN_SINGLE_OPEN" :options="yesNoOptions" />
    </s-form-item>
    <s-form-item label="允许登录错误次数" prop="LOGIN_ERROR_COUNT">
      <el-input-number v-model.number="loginFormProps.LOGIN_ERROR_COUNT" :min="1" :max="999" />
    </s-form-item>
    <s-form-item label="登录错误重置时间(分)" prop="LOGIN_ERROR_RESET_TIME">
      <el-input-number v-model.number="loginFormProps.LOGIN_ERROR_RESET_TIME" :min="1" :max="999" />
    </s-form-item>
    <s-form-item label="登录错误次数过多账号锁定时长:(分)" prop="LOGIN_ERROR_LOCK">
      <el-input-number v-model.number="loginFormProps.LOGIN_ERROR_LOCK" :min="1" :max="999" />
    </s-form-item>
    <s-form-item label="登录验证码开关" prop="LOGIN_CAPTCHA_OPEN">
      <s-radio-group v-model="loginFormProps.LOGIN_CAPTCHA_OPEN" :options="yesNoOptions" />
    </s-form-item>
    <s-form-item label="登录验证码类型" prop="LOGIN_CAPTCHA_TYPE">
      <s-radio-group v-model="loginFormProps.LOGIN_CAPTCHA_TYPE" :options="captchaOptions" />
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
import { useDictStore } from "@/stores/modules";
import { SysConfigTypeEnum, SysDictEnum } from "@/enums";

const dictStore = useDictStore(); //字典仓库

// 字典类型选项
const yesNoOptions = dictStore.getDictList(SysDictEnum.YES_NO);
const captchaOptions = dictStore.getDictList(SysDictEnum.CAPTCHA_TYPE);
const submitLoading = ref(false); //提交按钮loading
const loginFormRef = ref<FormInstance>();

/**  登录策略参数 */
const loginFormProps = reactive<SysConfig.LoginPolicyConfig>({
  LOGIN_SINGLE_OPEN: yesNoOptions[0].value,
  LOGIN_CAPTCHA_OPEN: yesNoOptions[0].value,
  LOGIN_CAPTCHA_TYPE: captchaOptions[0].value,
  LOGIN_ERROR_COUNT: 5,
  LOGIN_ERROR_RESET_TIME: 5,
  LOGIN_ERROR_LOCK: 30
});

//props定义
const props = defineProps({
  loginPolicy: {
    type: Array as PropType<SysConfig.ConfigInfo[]>,
    required: true,
    default: () => []
  }
});

//监听loginPolicy变化
watch(
  () => props.loginPolicy,
  (newVal: SysConfig.ConfigInfo[]) => {
    //重新赋值
    newVal.forEach((item: SysConfig.ConfigInfo) => {
      let prop = loginFormProps[item.configKey as keyof SysConfig.LoginPolicyConfig];
      //如果是number类型就转为number
      if (typeof prop === "number") {
        (loginFormProps[item.configKey as keyof SysConfig.LoginPolicyConfig] as number) = Number(item.configValue);
      } else {
        (loginFormProps[item.configKey as keyof SysConfig.LoginPolicyConfig] as string) = item.configValue;
      }
    });
  }
);

// 表单验证规则
const rules = reactive({
  LOGIN_ERROR_COUNT: [{ required: true, message: "请输入允许登录错误次数", trigger: "blur" }],
  LOGIN_ERROR_RESET_TIME: [{ required: true, message: "请输入登录错误重置时间", trigger: "blur" }],
  LOGIN_ERROR_LOCK: [{ required: true, message: "请输入登录错误次数过多账号锁定时长", trigger: "blur" }],
  LOGIN_CAPTCHA_TYPE: [{ required: true, message: "请选择登录验证码类型", trigger: "blur" }],
  LOGIN_SINGLE_OPEN: [{ required: true, message: "请选择单用户登录开关", trigger: "blur" }],
  LOGIN_CAPTCHA_OPEN: [{ required: true, message: "请选择登录验证码开关", trigger: "blur" }]
});

/** 提交表单 */
function onSubmit() {
  loginFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    submitLoading.value = true;
    //组装参数
    const param: SysConfig.ConfigInfo[] = Object.entries(loginFormProps).map(item => {
      return {
        id: 0,
        category: SysConfigTypeEnum.LOGIN_POLICY,
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
  loginFormRef.value?.resetFields();
}
</script>

<style lang="scss" scoped></style>
