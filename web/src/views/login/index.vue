<template>
  <div class="login-container flx-center">
    <div class="login-box">
      <SwitchDark class="dark" />
      <div class="login-left">
        <img class="login-left-img" src="@/assets/images/login_left.png" alt="login" />
      </div>
      <div class="login-form">
        <div class="login-logo">
          <img class="login-icon" src="@/assets/images/logo.png" alt="" />
          <h2 class="logo-text">SimpleAdmin</h2>
        </div>
        <el-tabs v-model="activeName" class="demo-tabs">
          <el-tab-pane label="账号密码" name="pwd">
            <LoginForm />
          </el-tab-pane>
          <el-tab-pane label="手机号登录" name="phone"> <LoginPhoneForm /></el-tab-pane>
        </el-tabs>
      </div>
    </div>
  </div>
  <choose-module v-if="auth.showChooseModule" :show-modal="true" />
</template>

<script setup lang="ts" name="login">
import LoginForm from "./components/pwd-login/index.vue";
import LoginPhoneForm from "./components/phone-login/index.vue";
import SwitchDark from "@/components/SwitchDark/index.vue";
import { useAuthStore } from "@/stores/modules";

const auth = useAuthStore();
const activeName = ref("pwd");
</script>

<style scoped lang="scss">
@import "./index.scss";
.demo-tabs > .el-tabs__content {
  padding: 32px;
  font-size: 32px;
  font-weight: 600;
  color: #6b778c;
}
</style>
