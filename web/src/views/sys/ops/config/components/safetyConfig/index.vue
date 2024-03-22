<!-- 
 * @Description: 安全配置
 * @Author: huguodong 
 * @Date: 2024-01-31 15:33:50
!-->
<template>
  <el-tabs v-model="activeName" class="demo-tabs" tab-position="left">
    <el-tab-pane label="登录策略" name="login" class="pl-5"><LoginForm :login-policy="loginPolicy" /></el-tab-pane>
    <el-tab-pane label="密码策略" name="pwd" class="pl-5"><PwdForm :pwd-policy="pwdPolicy" /></el-tab-pane>
  </el-tabs>
</template>

<script setup lang="ts">
import { SysConfig } from "@/api";
import LoginForm from "./loginForm.vue";
import PwdForm from "./pwdForm.vue";
import { SysConfigTypeEnum } from "@/enums";

const activeName = ref("login");
const loginPolicy = ref<SysConfig.ConfigInfo[]>([]);
const pwdPolicy = ref<SysConfig.ConfigInfo[]>([]);

//props定义
const props = defineProps({
  sysConfigs: {
    type: Array as PropType<SysConfig.ConfigInfo[]>,
    required: true,
    default: () => []
  }
});

//监听sysConfigs变化
watch(
  () => props.sysConfigs,
  (newVal: SysConfig.ConfigInfo[]) => {
    // 配置赋值
    loginPolicy.value = newVal.filter(item => item.category == SysConfigTypeEnum.LOGIN_POLICY);
    pwdPolicy.value = newVal.filter(item => item.category == SysConfigTypeEnum.PWD_POLICY);
  }
);
</script>

<style lang="scss" scoped>
.demo-tabs > .el-tabs__content {
  padding: 32px;
  font-size: 132px;
  font-weight: 600;
  color: #6b778c;
}
</style>
