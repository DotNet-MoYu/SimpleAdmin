<template>
  <el-form ref="loginFormRef" :model="loginForm" :rules="loginRules" size="large">
    <s-form-item v-if="tenantOption == TenantEnum.CHOSE" prop="orgId" class="-ml-12px">
      <s-select v-model="loginForm.tenantId" :options="tenantOptions" label="name" value="id" placeholder="请选择租户"></s-select>
    </s-form-item>
    <el-form-item prop="phone">
      <el-input v-model="loginForm.phone" placeholder="请输入手机号码">
        <template #prefix>
          <el-icon class="el-input__icon">
            <Iphone />
          </el-icon>
        </template>
      </el-input>
    </el-form-item>
    <!-- 验证码 -->
    <el-form-item prop="validCode">
      <div class="flex-y-center w-full">
        <el-input v-model="loginForm.validCode" placeholder="请输入短信验证码">
          <template #prefix>
            <el-icon class="el-input__icon">
              <message />
            </el-icon>
          </template>
        </el-input>
        <div class="w-18px"></div>
        <el-button size="large" @click="getCaptchaCode" :disabled="smsState.smsSendBtn">
          {{ (!smsState.smsSendBtn && "获取验证码") || smsState.time + " s" }}
        </el-button>
      </div>
    </el-form-item>
  </el-form>
  <div class="login-btn">
    <el-button :icon="CircleClose" round size="large" @click="resetForm(loginFormRef)"> 重置 </el-button>
    <el-button :icon="UserFilled" round size="large" type="primary" :loading="auth.loginLoading" @click="handleSubmit(loginFormRef)">
      登录
    </el-button>
  </div>
  <!-- 验证码弹窗 -->
  <el-dialog v-model="dialogVisible" title="机器验证" width="20%" show-close destroy-on-close>
    <el-form ref="captchaFormRef" :model="captchaForm" :rules="captchaRules" class="mt-25px" size="large">
      <el-row :gutter="8">
        <el-col :span="17">
          <el-form-item prop="validCode">
            <div class="flex-y-center w-full">
              <el-input v-model="captchaForm.validCode" placeholder="请输入验证码">
                <template #prefix>
                  <el-icon class="el-input__icon">
                    <key />
                  </el-icon>
                </template>
              </el-input>
            </div>
          </el-form-item>
        </el-col>
        <el-col :span="7"> <img :src="captchaForm.validCodeBase64" @click="loginCaptcha" /></el-col>
      </el-row>
    </el-form>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="getPhoneValidCode">确定</el-button>
      </span>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { Login, SysOrg, loginApi } from "@/api";
import { CircleClose, UserFilled } from "@element-plus/icons-vue";
import type { ElForm } from "element-plus";
import { TenantEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { useAuthStore, useConfigStore } from "@/stores/modules";
import { ElMessage } from "element-plus";

const auth = useAuthStore();
const config = useConfigStore();
const { loginPhone } = useAuthStore();

// 手机登录接口
interface PhoneProps {
  /** 多租户选项 */
  tenantOption: TenantEnum;
  /** 租户列表 */
  tenantOptions: SysOrg.SysOrgInfo[];
}
//props
const props = defineProps<PhoneProps>();
const dialogVisible = ref(false); // 是否显示弹窗
const loginForm = reactive<Login.PhoneLoginForm>({
  phone: "18652848888",
  validCode: "", // 验证码
  validCodeReqNo: "", // 验证码请求号
  tenantId: config.tenantIdGet
});

// 如果没有租户id就默认选中第一个
if (!config.tenantIdGet) {
  loginForm.tenantId = props.tenantOptions[0]?.id;
}

// 发送验证码状态
let smsState = ref({
  /** 倒计时 */
  time: 60,
  /** 发送验证码按钮状态 */
  smsSendBtn: false
});

// 图形验证码表单参数
const captchaForm = reactive({
  /** 图形验证码 */
  validCode: "",
  /** 图形验证码请求号 */
  validCodeReqNo: "",
  /** 图形验证码base64 */
  validCodeBase64: ""
});
const captchaFormRef = ref<FormInstance>(); // 图形验证码表单实例

type FormInstance = InstanceType<typeof ElForm>; // 表单实例
const loginFormRef = ref<FormInstance>(); // 表单实例引用
const loginRules = reactive({
  phone: [required("请输入手机号码")],
  code: [required("请输入短信验证码")],
  tenantId: [required("请选择租户")]
});

const captchaRules = reactive({
  validCode: [required("请输入验证码")]
});

onMounted(() => {
  // 监听 enter 事件（调用登录）
  document.onkeydown = (e: KeyboardEvent) => {
    e = (window.event as KeyboardEvent) || e;
    if (e.code === "Enter" || e.code === "enter" || e.code === "NumpadEnter") {
      if (auth.loginLoading) return;
      handleSubmit(loginFormRef.value);
    }
  };
});

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

// login
const handleSubmit = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.validate(async valid => {
    if (!valid) return;
    await loginPhone({ ...loginForm }).catch(async () => {
      await loginCaptcha(); // 加载验证码
    }); // 调用登录接口
  });
};

// resetForm
const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.resetFields();
};

/** 获取图片验证码 */
async function getCaptchaCode() {
  await loginCaptcha(); // 加载验证码
  dialogVisible.value = true;
}

/** 加载图片验证码 */
async function loginCaptcha() {
  const { data } = await loginApi.picCaptcha(); // 获取验证码
  if (data) {
    captchaForm.validCodeBase64 = data.validCodeBase64; // 验证码base64
    captchaForm.validCodeReqNo = data.validCodeReqNo; // 验证码请求号
  }
}

/** 获取手机验证码 */
function getPhoneValidCode() {
  captchaFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    dialogVisible.value = false;
    //设置发送验证码按钮状态
    smsState.value.smsSendBtn = true;
    const interval = window.setInterval(() => {
      if (smsState.value.time-- <= 0) {
        smsState.value.time = 60;
        smsState.value.smsSendBtn = false;
        window.clearInterval(interval);
      }
    }, 1000);
    loginApi
      .getPhoneValidCode({
        phone: loginForm.phone,
        validCode: captchaForm.validCode,
        validCodeReqNo: captchaForm.validCodeReqNo
      })
      .then(res => {
        console.log("[ res.data ] >", res.data);
        loginForm.validCodeReqNo = res.data;
        //发送成功
        dialogVisible.value = false;
        // 提示发送成功
        ElMessage.success("验证码发送成功");
      })
      .catch(() => {
        //发送失败
        ElMessage.error("短信验证码发送失败");
        smsState.value.time = 60;
        smsState.value.smsSendBtn = false;
        window.clearInterval(interval);
      });
  });
}
</script>

<style scoped lang="scss">
@import "../../index.scss";
</style>
