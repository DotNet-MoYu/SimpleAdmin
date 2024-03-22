<!-- 
 * @Description: 密码策略
 * @Author: huguodong 
 * @Date: 2024-01-31 16:25:33 
!-->
<template>
  <el-form ref="pwdFormRef" :rules="rules" :model="pwdFormProps" label-width="auto" label-suffix=" :" label-position="top">
    <s-form-item label="系统默认密码" prop="PWD_DEFAULT_PASSWORD" class="w-50">
      <s-input v-model="pwdFormProps.PWD_DEFAULT_PASSWORD"></s-input>
    </s-form-item>
    <s-form-item label="密码定期提醒更新" prop="PWD_REMIND">
      <s-radio-group v-model="pwdFormProps.PWD_REMIND" :options="yesNoOptions" />
    </s-form-item>
    <s-form-item label="密码定期提醒更新时间(天)" prop="LOGIN_ERROR_LOCK">
      <el-input-number v-model.number="pwdFormProps.PWD_REMIND_DAY" :min="1" :max="999" />
    </s-form-item>
    <s-form-item label="修改初始密码提醒" prop="PWD_UPDATE_DEFAULT">
      <s-radio-group v-model="pwdFormProps.PWD_UPDATE_DEFAULT" :options="yesNoOptions" />
    </s-form-item>
    <s-form-item label="密码最小长度" prop="PWD_MIN_LENGTH">
      <el-input-number v-model.number="pwdFormProps.PWD_MIN_LENGTH" :min="6" :max="20" />
    </s-form-item>
    <el-row :gutter="16">
      <el-col :span="6">
        <s-form-item label="是否含数字" prop="PWD_CONTAIN_NUM">
          <s-radio-group v-model="pwdFormProps.PWD_CONTAIN_NUM" :options="yesNoOptions" />
        </s-form-item>
      </el-col>
      <el-col :span="6">
        <s-form-item label="是否包含小写字母" prop="PWD_CONTAIN_LOWER">
          <s-radio-group v-model="pwdFormProps.PWD_CONTAIN_LOWER" :options="yesNoOptions" />
        </s-form-item>
      </el-col>
      <el-col :span="6">
        <s-form-item label="是否包含大写字母" prop="PWD_CONTAIN_UPPER">
          <s-radio-group v-model="pwdFormProps.PWD_CONTAIN_UPPER" :options="yesNoOptions" />
        </s-form-item>
      </el-col>
      <el-col :span="6">
        <s-form-item label="是否包含特殊字符" prop="PWD_CONTAIN_CHARACTER">
          <s-radio-group v-model="pwdFormProps.PWD_CONTAIN_CHARACTER" :options="yesNoOptions" />
        </s-form-item>
      </el-col>
    </el-row>
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
const submitLoading = ref(false); //提交按钮loading
const pwdFormRef = ref<FormInstance>();

/**  登录策略参数 */
const pwdFormProps = reactive<SysConfig.PwdPolicyConfig>({
  PWD_DEFAULT_PASSWORD: "",
  PWD_REMIND: yesNoOptions[0].value,
  PWD_REMIND_DAY: 30,
  PWD_UPDATE_DEFAULT: yesNoOptions[0].value,
  PWD_MIN_LENGTH: 6,
  PWD_CONTAIN_NUM: yesNoOptions[0].value,
  PWD_CONTAIN_LOWER: yesNoOptions[0].value,
  PWD_CONTAIN_UPPER: yesNoOptions[0].value,
  PWD_CONTAIN_CHARACTER: yesNoOptions[0].value
});

//props定义
const props = defineProps({
  pwdPolicy: {
    type: Array as PropType<SysConfig.ConfigInfo[]>,
    required: true,
    default: () => []
  }
});

//监听pwdPolicy变化
watch(
  () => props.pwdPolicy,
  (newVal: SysConfig.ConfigInfo[]) => {
    //重新赋值
    newVal.forEach((item: SysConfig.ConfigInfo) => {
      let prop = pwdFormProps[item.configKey as keyof SysConfig.PwdPolicyConfig];
      //如果是number类型就转为number
      if (typeof prop === "number") {
        (pwdFormProps[item.configKey as keyof SysConfig.PwdPolicyConfig] as number) = Number(item.configValue);
      } else {
        (pwdFormProps[item.configKey as keyof SysConfig.PwdPolicyConfig] as string) = item.configValue;
      }
    });
  }
);

// 表单验证规则
const rules = reactive({
  PWD_DEFAULT_PASSWORD: [{ required: true, message: "请输入系统默认密码", trigger: "blur" }],
  PWD_REMIND: [{ required: true, message: "请选择密码定期提醒更新", trigger: "blur" }],
  PWD_REMIND_DAY: [{ required: true, message: "请输入密码定期提醒更新时间", trigger: "blur" }],
  PWD_UPDATE_DEFAULT: [{ required: true, message: "请选择修改初始密码提醒", trigger: "blur" }],
  PWD_MIN_LENGTH: [{ required: true, message: "请输入密码最小长度", trigger: "blur" }],
  PWD_CONTAIN_NUM: [{ required: true, message: "请选择是否含数字", trigger: "blur" }],
  PWD_CONTAIN_LOWER: [{ required: true, message: "请选择是否包含小写字母", trigger: "blur" }],
  PWD_CONTAIN_UPPER: [{ required: true, message: "请选择是否包含大写字母", trigger: "blur" }],
  PWD_CONTAIN_CHARACTER: [{ required: true, message: "请选择是否包含特殊字符", trigger: "blur" }]
});

/** 提交表单 */
function onSubmit() {
  pwdFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    submitLoading.value = true;
    //组装参数
    const param: SysConfig.ConfigInfo[] = Object.entries(pwdFormProps).map(item => {
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
  pwdFormRef.value?.resetFields();
}
</script>

<style lang="scss" scoped></style>
