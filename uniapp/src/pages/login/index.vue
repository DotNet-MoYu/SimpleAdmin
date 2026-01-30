<!--
 * @Description: 登录
 * @Author: huguodong
 * @Date: 2025-07-14 14:28:50
! -->
<template>
  <view class="content">
    <!-- 头部欢迎区域 -->
    <view class="login-header">
      <text class="greeting">
        Hi
      </text>
      <text class="greeting">
        欢迎登录{{ props.sysName }}
      </text>
    </view>

    <!-- 登录表单区域 -->
    <view class="login-form">
      <!-- 登录方式切换 -->
      <view class="login-tabs">
        <text
          class="tab-item" :class="[
            { active: adminType === AdminTypeEnum.login },
          ]"
          @click="adminType = AdminTypeEnum.login"
        >
          账号密码登录
        </text>
        <text
          class="tab-item" :class="[
            { active: adminType === AdminTypeEnum.code },
          ]"
          @click="adminType = AdminTypeEnum.code"
        >
          验证码登录
        </text>
      </view>
      <LoginForm
        v-if="adminType === AdminTypeEnum.login"
        :tenant-option="props.sysTenantOption"
        :tenant-options="props.tenantOptions"
      />
      <LoginPhoneForm
        v-else
        :tenant-option="props.sysTenantOption"
        :tenant-options="props.tenantOptions"
      />

      <!-- 底部链接 -->
      <view class="form-footer" />
    </view>
  </view>
</template>

<script lang="ts" setup>
import type { SysConfig, SysOrg } from '@/api'
import { TenantEnum } from '@/enum'
import { useConfigStore } from '@/store/modules'
import LoginPhoneForm from './components/phone-login/index.vue'
import LoginForm from './components/pwd-login/index.vue'

defineOptions({
  name: 'Login',
})
definePage({
  name: 'login',
  style: {
    // 'custom' 表示开启自定义导航栏，默认 'default'
    navigationStyle: 'custom',
    navigationBarTitleText: '登录',
  },
})

/** 登录参数接口 */
interface LoginProps {
  /** 系统名称 */
  sysName: string
  /** 系统版本 */
  sysVersion: string
  /** 系统logo */
  sysLogo: string
  /** 多租户选项 */
  sysTenantOption: TenantEnum
  /** 多租户列表 */
  tenantOptions: SysOrg.SysOrgInfo[]
}

// 枚举定义
enum AdminTypeEnum {
  login = 'login',
  code = 'code',
}
const config = useConfigStore()
// 默认值
const props = reactive<LoginProps>({
  sysName: 'SimpleAdmin',
  sysVersion: '',
  sysLogo: '',
  sysTenantOption: TenantEnum.CLOSE,
  tenantOptions: config.tenantList,
})

// 状态定义
const adminType = ref<AdminTypeEnum>(AdminTypeEnum.login)

onMounted(() => {
  // 更新系统配置
  updateSysConfig(props, config)
})

/**
 * 更新系统配置
 * @param props 登录参数
 * @param config 配置store
 */
function updateSysConfig(
  props: LoginProps,
  config: ReturnType<typeof useConfigStore>,
) {
  // 定义更新配置的逻辑
  const updateConfig = (res: SysConfig.SysBaseConfig) => {
    console.log('[ updateConfig ] >', res)
    props.sysName = res.SYS_NAME
    props.sysVersion = res.SYS_VERSION
    props.sysLogo = res.SYS_LOGO
    props.sysTenantOption = res.SYS_TENANT_OPTIONS
    // 如果多租户选项是选择就加载租户列表
    if (props.sysTenantOption === TenantEnum.CHOSE) {
      config.setTenantList().then((res) => {
        props.tenantOptions = res
      })
    }
    else {
      props.tenantOptions = []
      config.delTenantId()
    }
  }
  // 先从本地存储获取系统配置,放置网络延迟导致页面组件显示延迟
  config.getSysBaseInfo().then(updateConfig)
}
</script>

<style lang="scss" scoped>
@import './index';
</style>
