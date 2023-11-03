<template>
  <el-form ref="loginFormRef" :model="loginForm" :rules="loginRules" size="large">
    <s-form-item prop="phone">
      <el-input v-model="loginForm.phone" placeholder="请输入手机号码">
        <template #prefix>
          <el-icon class="el-input__icon">
            <Iphone />
          </el-icon>
        </template>
      </el-input>
    </s-form-item>
    <!-- 验证码 -->
    <s-form-item path="code">
      <div class="flex-y-center w-full">
        <el-input v-model:value="loginForm.code" placeholder="请输入短信验证码">
          <template #prefix>
            <el-icon class="el-input__icon">
              <message />
            </el-icon>
          </template>
        </el-input>
        <div class="w-18px"></div>
        <el-button size="large"> 获取验证码 </el-button>
      </div>
    </s-form-item>
  </el-form>
  <div class="login-btn">
    <el-button :icon="CircleClose" round size="large" @click="resetForm(loginFormRef)"> 重置 </el-button>
    <el-button :icon="UserFilled" round size="large" type="primary" :loading="loading" @click="login(loginFormRef)"> 登录 </el-button>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";

import { Login } from "@/api/interface";

import { CircleClose, UserFilled } from "@element-plus/icons-vue";
import type { ElForm } from "element-plus";

type FormInstance = InstanceType<typeof ElForm>;
const loginFormRef = ref<FormInstance>();
const loginRules = reactive({
  phone: [{ required: true, message: "请输入用户名", trigger: "blur" }],
  code: [{ required: true, message: "请输入密码", trigger: "blur" }]
});

const loading = ref(false);
const loginForm = reactive<Login.PhoneLoginForm>({
  phone: "18652848888",
  code: "",
  validCode: "", // 验证码
  validCodeReqNo: "" // 验证码请求号
});

// login
const login = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.validate(async valid => {
    if (!valid) return;
    loading.value = true;
    try {
      // // 1.执行登录接口
      // const { data } = await loginApi({ ...loginForm, password: md5(loginForm.password) });
      // userStore.setToken(data.access_token);
      // // 2.添加动态路由
      // await initDynamicRouter();
      // // 3.清空 tabs、keepAlive 数据
      // tabsStore.closeMultipleTab();
      // keepAliveStore.setKeepAliveName();
      // // 4.跳转到首页
      // router.push(HOME_URL);
      // ElNotification({
      //   title: getTimeState(),
      //   message: "欢迎登录 SimpleAdmin",
      //   type: "success",
      //   duration: 3000
      // });
    } finally {
      loading.value = false;
    }
  });
};

// resetForm
const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.resetFields();
};

onMounted(() => {
  // 监听 enter 事件（调用登录）
  document.onkeydown = (e: KeyboardEvent) => {
    e = (window.event as KeyboardEvent) || e;
    if (e.code === "Enter" || e.code === "enter" || e.code === "NumpadEnter") {
      if (loading.value) return;
      login(loginFormRef.value);
    }
  };
});
</script>

<style scoped lang="scss">
@import "../../index.scss";
</style>
