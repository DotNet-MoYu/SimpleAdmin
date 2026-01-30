<template>
  <div class="load-contain">
    <div class="loading-wave">
      <div class="loading-bar" />
      <div class="loading-bar" />
      <div class="loading-bar" />
      <div class="loading-bar" />
    </div>
    <!-- 添加的WotUI重试按钮 -->
    <wd-button
      v-if="showRetryBtn"
      class="retry-btn"
      type="primary"
      size="medium"
      @click="retryRequest"
    >
      重新请求系统信息
    </wd-button>
    <wd-toast />
  </div>
</template>

<script lang="ts" setup>
import { useRouter } from 'uni-mini-router'
import { useToast } from 'wot-design-uni'
import { useConfigStore, useUserStore } from '@/store'

defineOptions({
  name: 'Home',
})
definePage({
  // 使用 type: "home" 属性设置首页，其他页面不需要设置，默认为page
  type: 'home',
  style: {
    // 'custom' 表示开启自定义导航栏，默认 'default'
    navigationStyle: 'custom',
    navigationBarTitleText: '首页',
  },
})

const router = useRouter()
const configStore = useConfigStore()
const userStore = useUserStore()
const toast = useToast()
// 添加重试按钮状态控制
const showRetryBtn = ref(false)

onMounted(() => {
  console.log('loading页面加载完毕')
  getSysInfo()
})

/**
 * @func 请求系统信息
 */
function getSysInfo() {
  showRetryBtn.value = false // 请求开始隐藏重试按钮
  configStore
    .setSysBaseInfo()
    .then(() => {
      // 请求成功后隐藏重试按钮
      showRetryBtn.value = true
      console.log('系统信息加载成功')
      // 如果有token跳转首页
      if (userStore.accessToken) {
        console.log('跳转首页')
        router.pushTab({
          name: 'home',
        })
      }
      else {
        console.log('跳转登录页')
        // 如果没有token跳转登录页
        router.replace({
          name: 'login',
        })
      }
    })
    .catch((err) => {
      console.error('获取系统信息失败:', err)
      toast.error('获取系统信息失败，请重试')
      showRetryBtn.value = true // 请求失败显示重试按钮
    })
}

/**
 * @func 重新请求系统信息
 */
function retryRequest() {
  showRetryBtn.value = false // 隐藏重试按钮
  getSysInfo() // 重新发起请求
}
</script>

<style lang="scss" scoped>
.load-contain {
  display: flex;
  flex-direction: column; /* 改为纵向布局 */
  justify-content: center;
  align-items: center;
  height: 100vh;
  padding: 20px;
}

.loading-wave {
  display: flex;
  justify-content: center;
  align-items: flex-end;
  width: 300px;
  height: 60px;
  margin-bottom: 40px; /* 添加底部间距 */
}

.loading-bar {
  margin: 0 5px;
  width: 20px;
  height: 10px;
  border-radius: 5px;
  background-color: $uni-color-primary;
  animation: loading-wave-animation 1s ease-in-out infinite;
}

.loading-bar:nth-child(2) {
  animation-delay: 0.1s;
}

.loading-bar:nth-child(3) {
  animation-delay: 0.2s;
}

.loading-bar:nth-child(4) {
  animation-delay: 0.3s;
}

/* 添加重试按钮样式 */
.retry-btn {
  width: 130px;
  border-radius: 30px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.retry-btn:active {
  transform: translateY(2px);
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

@keyframes loading-wave-animation {
  0% {
    height: 10px;
  }

  50% {
    height: 50px;
  }

  100% {
    height: 10px;
  }
}
</style>
