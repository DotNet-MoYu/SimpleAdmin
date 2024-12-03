<!-- 密码登录 -->
<template>
  <el-form ref="loginFormRef" :model="loginForm" :rules="loginRules" size="large">
    <s-form-item v-if="tenantOption == TenantEnum.CHOSE" prop="orgId" class="-ml-12px">
      <s-select v-model="loginForm.tenantId" :options="tenantOptions" label="name" value="id" placeholder="请选择租户"></s-select>
    </s-form-item>
    <el-form-item prop="account">
      <el-input v-model="loginForm.account" placeholder="请输入用户名">
        <template #prefix>
          <el-icon class="el-input__icon">
            <user />
          </el-icon>
        </template>
      </el-input>
    </el-form-item>
    <el-form-item prop="password">
      <el-input v-model="loginForm.password" type="password" placeholder="请输入密码" show-password autocomplete="new-password">
        <template #prefix>
          <el-icon class="el-input__icon">
            <lock />
          </el-icon>
        </template>
      </el-input>
    </el-form-item>
    <el-form-item v-if="captchaOpen" prop="validCode">
      <div class="flex-y-center w-full">
        <el-input v-model="loginForm.validCode" placeholder="请输入验证码">
          <template #prefix>
            <el-icon class="el-input__icon">
              <key />
            </el-icon>
          </template>
        </el-input>
        <div class="w-18px"></div>
        <img :src="validCodeBase64" @click="loginCaptcha" />
      </div>
    </el-form-item>
  </el-form>
  <div class="login-btn">
    <el-button :icon="CircleClose" round size="large" @click="resetForm(loginFormRef)"> 重置 </el-button>
    <el-button :icon="UserFilled" round size="large" type="primary" :loading="auth.loginLoading" @click="handleSubmit(loginFormRef)">
      登录
    </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, onBeforeUnmount } from "vue";
import { Login } from "@/api/interface";
import { SysOrg, commonApi, loginApi } from "@/api";
import { useAuthStore, useConfigStore } from "@/stores/modules";
import { CircleClose, UserFilled } from "@element-plus/icons-vue";
import type { ElForm } from "element-plus";
import { SysBaseEnum, TenantEnum } from "@/enums";
import smCrypto from "@/utils/smCrypto";
import { required } from "@/utils/formRules";

const auth = useAuthStore();
const config = useConfigStore();
const { loginPwd } = useAuthStore();

type FormInstance = InstanceType<typeof ElForm>;
const captchaOpen = ref(false); // 是否开启验证码
const validCodeBase64 = ref(); // 验证码base64

// 密码登录接口
interface PwdProps {
  /** 多租户选项 */
  tenantOption: TenantEnum;
  /** 租户列表 */
  tenantOptions: SysOrg.SysOrgInfo[];
}

//props
const props = defineProps<PwdProps>();

//监听tenantOptions变化
watch(
  () => props.tenantOptions,
  (newVal: SysOrg.SysOrgInfo[]) => {
    if (newVal.length > 0) {
      // 如果没有选择租户就默认选中第一个
      if (!loginForm.tenantId) {
        loginForm.tenantId = newVal[0].id;
      }
    }
  }
);

const loginFormRef = ref<FormInstance>(); // 表单实例
// 表单数据
const loginForm = reactive<Login.LoginForm>({
  account: "superAdmin", //用户名
  password: "123456", // 密码
  validCode: "", // 验证码
  validCodeReqNo: "", // 验证码请求号
  tenantId: config.tenantIdGet
});
// 如果没有租户id就默认选中第一个
if (!config.tenantIdGet) {
  loginForm.tenantId = props.tenantOptions[0]?.id;
}

// 表单验证规则
const loginRules = reactive({
  account: [required("请输入用户名")],
  password: [required("请输入密码")],
  tenantId: [required("请选择租户")],
  validCode: [required("请输入验证码")]
});

const handleSubmit = (formEl: FormInstance | undefined) => {
  console.log(123213);

  if (!formEl) return;
  // 表单验证
  formEl.validate(async valid => {
    if (!valid) return;
    await loginPwd({ ...loginForm, password: smCrypto.doSm2Encrypt(loginForm.password) }).catch(async () => {
      await loginCaptcha(); // 加载验证码
    }); // 调用登录接口
  });
};

// 重置表单
const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.resetFields();
};

onMounted(() => {
  // 监听 enter 事件（调用登录）
  document.onkeydown = (e: KeyboardEvent) => {
    e = (window.event as KeyboardEvent) || e;
    if (e.code === "Enter" || e.code === "enter" || e.code === "NumpadEnter") {
      if (auth.loginLoading) return;
      handleSubmit(loginFormRef.value);
    }
  };
  // 获取验证码开关
  commonApi.loginPolicy().then(async res => {
    // 如果验证码开关是开就加载验证码
    if (res.data) {
      captchaOpen.value = res.data.find(item => item.configKey === SysBaseEnum.LOGIN_CAPTCHA_OPEN)?.configValue === "true"; // 判断是否开启验证码
      if (captchaOpen.value) {
        await loginCaptcha(); // 加载验证码
      }
    }
  });
});

/** 加载图片验证码 */
async function loginCaptcha() {
  const { data } = await loginApi.picCaptcha(); // 获取验证码
  if (data) {
    validCodeBase64.value = data.validCodeBase64; // 验证码base64
    loginForm.validCodeReqNo = data.validCodeReqNo; // 验证码请求号
  }
}

onBeforeUnmount(() => {
  document.onkeydown = null;
});
</script>

<style scoped lang="scss">
@import "../../index";
</style>
