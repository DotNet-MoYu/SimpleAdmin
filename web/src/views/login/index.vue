<!-- 
 * @Description: 登录页
 * @Author: huguodong 
 * @Date: 2023-12-15 15:42:19
!-->
<template>
  <div class="login-container flx-center">
    <div class="login-box">
      <SwitchDark class="dark" />
      <div class="login-left">
        <img class="login-left-img" src="@/assets/images/login_left.png" alt="login" />
      </div>
      <div class="login-form">
        <div class="login-logo">
          <img v-show="props.sysLogo" class="login-icon" :src="props.sysLogo" alt="" />
          <h2 class="logo-text">{{ props.sysName }}</h2>
        </div>
        <el-tabs v-model="activeName" class="demo-tabs">
          <el-tab-pane label="账号登录" name="pwd">
            <LoginForm :tenant-option="props.sysTenantOption" :tenant-options="props.tenantOptions" />
          </el-tab-pane>
          <el-tab-pane label="手机号登录" name="phone">
            <LoginPhoneForm :tenant-option="props.sysTenantOption" :tenant-options="props.tenantOptions" />
          </el-tab-pane>
        </el-tabs>
        <div class="fixed-corner">
          <span>{{ props.sysVersion }}</span>
        </div>
      </div>
    </div>
  </div>
  <Footer />
  <choose-module v-if="auth.showChooseModule" :show-modal="true" />
</template>

<script setup lang="ts" name="login">
import LoginForm from "./components/pwd-login/index.vue";
import LoginPhoneForm from "./components/phone-login/index.vue";
import SwitchDark from "@/components/SwitchDark/index.vue";
import { useAuthStore, useConfigStore } from "@/stores/modules";
import Footer from "@/layouts/components/Footer/index.vue";
import { TenantEnum } from "@/enums";
import { SysConfig, SysOrg } from "@/api";

const auth = useAuthStore();
const config = useConfigStore();
const activeName = ref("pwd");

/** 登录参数接口 */
interface LoginProps {
  /** 系统名称 */
  sysName: string;
  /** 系统版本 */
  sysVersion: string;
  /** 系统logo */
  sysLogo: string;
  /** 多租户选项 */
  sysTenantOption: TenantEnum;
  /** 多租户列表 */
  tenantOptions: SysOrg.SysOrgInfo[]; // 租户列表
}

//默认值
const props = reactive<LoginProps>({
  sysName: "SimpleAdmin",
  sysVersion: "",
  sysLogo: "",
  sysTenantOption: TenantEnum.CLOSE,
  tenantOptions: config.tenantListGet
});

onMounted(() => {
  // 更新系统配置
  updateSysConfig(props, config);
});

/**
 * 更新系统配置
 * @param props 登录参数
 * @param config 配置store
 */
function updateSysConfig(props: LoginProps, config: ReturnType<typeof useConfigStore>) {
  // 定义更新配置的逻辑
  const updateConfig = (res: SysConfig.SysBaseConfig) => {
    props.sysName = res.SYS_NAME;
    props.sysVersion = res.SYS_VERSION;
    props.sysLogo = res.SYS_LOGO;
    props.sysTenantOption = res.SYS_TENANT_OPTIONS;
    // 如果多租户选项是选择就加载租户列表
    if (props.sysTenantOption === TenantEnum.CHOSE) {
      config.setTenantList();
    } else {
      props.tenantOptions = [];
      config.delTenantId();
    }
  };

  // 先从本地存储获取系统配置,放置网络延迟导致页面组件显示延迟
  config.getSysBaseInfo().then(updateConfig);

  // 再从网络获取系统配置
  config.setSysBaseInfo().then(updateConfig);
}
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
