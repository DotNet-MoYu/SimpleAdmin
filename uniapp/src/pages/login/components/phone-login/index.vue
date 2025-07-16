<!-- 账号密码登录组件 components/pwd-login/index.vue -->
<template>
  <wd-form
    ref="loginFormRef"
    :model="loginForm"
    :rules="loginRules"
    @submit="handleLogin"
  >
    <wd-cell-group border class="input-group">
      <view v-if="tenantOption !== TenantEnum.CLOSE">
        <wd-select-picker
          v-model="loginForm.tenantId"
          value-key="id"
          label-key="name"
          type="radio"
          :show-confirm="false"
          placeholder="请选择租户"
          :columns="tenantOptions"
        />
      </view>

      <!-- 手机号输入 -->
      <wd-input
        v-model="loginForm.phone"
        size="large"
        prop="phone"
        clearable
        placeholder="请输入账号/手机号"
      />
      <!-- 新增验证码区域 -->
      <view class="captcha-group">
        <wd-input
          v-model="captchaForm.validCode"
          size="large"
          prop="validCode"
          clearable
          placeholder="请输入图片验证码"
          class="captcha-input"
        />
        <image
          :src="validCodeBase64"
          class="captcha-image"
          mode="widthFix"
          @click="loginCaptcha"
        />
      </view>
      <view class="captcha-group">
        <wd-input
          v-model="loginForm.validCode"
          size="large"
          prop="validCode"
          clearable
          placeholder="请输入短信验证码"
          class="captcha-input"
        />
        <wd-button
          plain
          class="sms-btn"
          :disabled="isCountingDown"
          @click="getPhoneValidCode"
        >
          {{ countdownText }}
        </wd-button>
      </view>
    </wd-cell-group>

    <!-- 登录按钮 -->
    <view style="margin-top: 20px;">
      <wd-button
        class="login-btn"
        type="primary"
        size="large"
        block
        :loading="authStore.loginLoading"
        @click="handleLogin"
      >
        登录
      </wd-button>
    </view>
  </wd-form>
</template>

<script lang="ts" setup>
import type { FormInstance } from 'wot-design-uni/components/wd-form/types'
import type { Login, SysOrg } from '@/api'
import { commonApi, loginApi } from '@/api'
import { SysBaseEnum, TenantEnum } from '@/enum'
import { useAuthStore, useConfigStore } from '@/store/modules'
import { required } from '@/utils/formRules'
import modal from '@/utils/modal'

// props
const props = defineProps<PwdProps>()
const authStore = useAuthStore()
const config = useConfigStore()
const captchaOpen = ref(false) // 是否开启验证码
const validCodeBase64 = ref() // 验证码base64

// 密码登录接口
interface PwdProps {
  /** 多租户选项 */
  tenantOption: TenantEnum
  /** 租户列表 */
  tenantOptions: SysOrg.SysOrgInfo[]
}

const loginFormRef = ref<FormInstance | null>(null) // 表单实例
// 表单数据
const loginForm = reactive<Login.PhoneLoginForm>({
  phone: '18652848888',
  validCode: '', // 验证码
  validCodeReqNo: '', // 验证码请求号
  tenantId: config.tenantId || 0,
  device: 1,
})

// 监听tenantOptions变化
watch(
  () => props.tenantOptions,
  (newVal: SysOrg.SysOrgInfo[]) => {
    if (newVal.length > 0) {
      // 如果没有选择租户就默认选中第一个
      if (!loginForm.tenantId) {
        loginForm.tenantId = newVal[0].id
      }
    }
  },
)

// 验证码表单
const captchaForm = reactive({
  validCode: '', // 验证码
  validCodeReqNo: '', // 验证码请求号
})

// 如果没有租户id就默认选中第一个
if (!config.tenantId) {
  loginForm.tenantId = props.tenantOptions[0]?.id
}

// 表单验证规则
const loginRules = reactive({
  phone: [required('请输入手机号码')],
  validCode: [required('请输入短信验证码')],
  tenantId: [required('请选择租户')],
})

// 登录处理
async function handleLogin() {
  try {
    // 表单验证
    loginFormRef.value
      ?.validate()
      .then(async ({ valid, errors }) => {
        if (valid) {
          const params: Login.PhoneLoginForm = {
            phone: loginForm.phone, // 手机号码
            validCode: '',
            validCodeReqNo: '',
            tenantId: loginForm.tenantId || 0,
            device: 1,
          }
          // #ifdef MP
          params.device = 2 // 如果是小程序
          // #endif
          // 执行登录
          await authStore
            .loginPhone({
              ...loginForm,
            })
            .catch(async () => {
              await loginCaptcha() // 加载验证码
            }) // 调用登录接口
        }
        else {
          console.log('error submit!!', errors)
        }
      })
      .catch((error) => {
        console.log(error, 'error')
      })
  }
  catch (error) {
    console.error('登录失败', error)
  }
}

// 倒计时秒数
const countdownSeconds = ref(0)
// 倒计时计时器
let countdownTimer: ReturnType<typeof setInterval> | null = null

// 计算属性
const isCountingDown = computed(() => countdownSeconds.value > 0)
const countdownText = computed(() =>
  isCountingDown.value ? `${countdownSeconds.value}秒后重发` : '获取验证码',
)

/** 清除计时器 */
function clearCountdownTimer() {
  if (countdownTimer) {
    clearInterval(countdownTimer)
    countdownTimer = null
  }
}

/** 加载图片验证码 */
async function loginCaptcha() {
  const { data } = await loginApi.picCaptcha() // 获取验证码
  if (data) {
    validCodeBase64.value = data.validCodeBase64 // 验证码base64
    captchaForm.validCodeReqNo = data.validCodeReqNo // 验证码请求号
  }
}

/** 计时 */
async function getPhoneValidCode() {
  // 判断是否填了验证码
  if (captchaForm.validCode === '') {
    modal.msg('请输入图片验证码')
    return
  }
  await loginApi
    .getPhoneValidCode({
      phone: loginForm.phone,
      validCode: captchaForm.validCode,
      validCodeReqNo: captchaForm.validCodeReqNo,
    })
    .then((res) => {
      console.log('[ res.data ] >', res.data)
      loginForm.validCodeReqNo = res.data
      // 提示发送成功
      modal.msg('验证码发送成功')
      // 如果没有输入手机号或者图片验证码就提示
      if (isCountingDown.value)
        return
      countdownSeconds.value = 60
      countdownTimer = setInterval(() => {
        countdownSeconds.value--
        if (countdownSeconds.value <= 0) {
          clearCountdownTimer()
        }
      }, 1000)
    })
    .catch(async () => {
      // 发送失败
      modal.msg('短信验证码发送失败')
      await loginCaptcha() // 加载验证码
    })
}

// 初始化时获取验证码
onMounted(() => {
  // 获取验证码开关
  commonApi.loginPolicy().then(async (res) => {
    // 如果验证码开关是开就加载验证码
    if (res.data) {
      captchaOpen.value
                = res.data.find(
          item => item.configKey === SysBaseEnum.LOGIN_CAPTCHA_OPEN,
        )?.configValue === 'true' // 判断是否开启验证码
      if (captchaOpen.value) {
        await loginCaptcha() // 加载验证码
      }
    }
  })
})
</script>

<style lang="scss" scoped>
// 输入框组

.form-options {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20rpx;
  margin: 30rpx 0 20rpx;

  .forgot-password {
    font-size: 26rpx;
    color: $uni-color-primary;
  }
}

.login-btn {
  border-radius: 12rpx;
  --wot-button-border-radius: 12rpx;
}

// 验证码输入组
.captcha-group {
  display: flex;
  gap: 20rpx;
  align-items: center;

  .captcha-input {
    flex: 1;
  }

  .captcha-image {
    width: 200rpx;
    height: 80rpx;
    background-color: #f5f7fa;
    border: 1rpx solid #dcdfe6;
    border-radius: 8rpx;
  }
}

.sms-btn {
  // position: absolute;
  // right: 20rpx;
  // top: 50%;
  // transform: translateY(-50%);
  font-size: 28rpx;
  color: $uni-color-primary;

  // background: none;
  // border: none;
  // padding: 10rpx 15rpx;
  border-radius: 6rpx;

  &:disabled {
    color: #999;
    background-color: #f1f1f1;
  }
}
</style>
