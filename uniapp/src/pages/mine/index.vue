<!--
 * @Description: 个人中心页面
 * @Author: huguodong
 * @Date: 2025-07-15 15:52:39
! -->
<template>
  <view class="mine-container" :style="{ height: `${windowHeight}px` }">
    <!-- 顶部个人信息 -->
    <view class="header-section" :style="{ paddingTop: `${headerTopPadding}px` }">
      <view class="avatar-section">
        <!-- 如果没有头像，则显示默认图标 -->
        <view v-if="!avatar" class="icon">
          <uni-icons
            type="person"
            size="30"
            color="#808080"
          />
        </view>
        <!-- 如果有头像，则显示头像图片 -->
        <image
          v-if="avatar"
          class="avatar"
          :src="avatar"
          mode="aspectFit"
          @click="handleToAvatar"
        />
        <!-- 如果没有登录，则显示登录提示 -->
        <view v-if="!name" class="login-tip" @click="handleToLogin">
          点击登录
        </view>
        <!-- 如果已登录，则显示用户信息 -->
        <view v-if="name" class="user-info" @click="handleToInfo">
          <view class="title">
            {{ name }}
          </view>
        </view>
      </view>
      <!-- 点击跳转到个人信息页面 -->
      <view class="avatar-title" @click="handleToInfo">
        <text>个人信息</text>
        <uni-icons type="forward" size="20" color="#ffffff" />
      </view>
    </view>

    <view class="content-section">
      <wd-grid :column="4">
        <!-- 首页设置 -->
        <wd-grid-item
          icon-size="36px"
          use-icon-slot
          text="首页设置"
          @itemclick="handleToHomeConfig"
        >
          <template #icon>
            <simple-icon
              background-color="#2979ff"
              icon="ep:home-filled"
              color="#FFFFFF"
              class="h-6 w-6"
            />
          </template>
        </wd-grid-item>

        <!-- 建设中 1 -->
        <wd-grid-item
          icon-size="36px"
          use-icon-slot
          text="建设中"
          @itemclick="handleBuilding"
        >
          <template #icon>
            <simple-icon
              background-color="#fa3534"
              icon="ep:setting"
              color="#FFFFFF"
              class="h-6 w-6"
            />
          </template>
        </wd-grid-item>

        <!-- 建设中 2 -->
        <wd-grid-item
          icon-size="36px"
          use-icon-slot
          text="建设中"
          @itemclick="handleBuilding"
        >
          <template #icon>
            <simple-icon
              background-color="#ff9900"
              icon="ep:setting"
              color="#FFFFFF"
              class="h-6 w-6"
            />
          </template>
        </wd-grid-item>

        <!-- 建设中 3 -->
        <wd-grid-item
          icon-size="36px"
          use-icon-slot
          text="建设中"
          @itemclick="handleBuilding"
        >
          <template #icon>
            <simple-icon
              background-color="#19be6b"
              icon="ep:setting"
              color="#FFFFFF"
              class="h-6 w-6"
            />
          </template>
        </wd-grid-item>
      </wd-grid>
    </view>

    <view class="content-list">
      <!-- 使用uni-list组件展示两个功能模块 -->
      <uni-list>
        <!-- 编辑资料 -->
        <uni-list-item
          :show-extra-icon="true"
          show-arrow
          title="编辑资料"
          clickable
          :extra-icon="{
            color: '#2979ff',
            size: '20',
            type: 'person',
          }"
          @click="handleToEditInfo"
        />
        <!-- 修改密码 -->
        <uni-list-item
          :show-extra-icon="true"
          show-arrow
          title="修改密码"
          clickable
          :extra-icon="{
            color: '#2979ff',
            size: '20',
            type: 'locked',
          }"
          @click="handleToPwd"
        />
      </uni-list>
    </view>
    <!-- 退出登录按钮 -->
    <wd-button class="btn-logout" size="large" block @click="handleLogout">
      退出登录
    </wd-button>
  </view>
</template>

<route lang="jsonc" type="page">
{
  "layout": "tabbar",
  "name": "mine",
  "style": {
    "navigationStyle": "custom",
    "navigationBarTitleText": "工作台"
  }
}
</route>

<script setup lang="ts">
import { useRouter } from 'uni-mini-router'
import { loginApi } from '@/api'
import SimpleIcon from '@/components/simple-icon/index.vue'
import { useUserStore } from '@/store/modules'
import modal from '@/utils/modal'
import tab from '@/utils/tab'
// 使用hooks（推荐）
const router = useRouter()
const userStore = useUserStore()
const name = userStore.userInfo?.name

const systemInfo = uni.getSystemInfoSync()

const headerTopPadding = computed(() => {
  // 微信小程序专属处理
  if (process.env.UNI_PLATFORM === 'mp-weixin') {
    const menuButtonInfo = uni.getMenuButtonBoundingClientRect()
    return menuButtonInfo.top + menuButtonInfo.height + 10
  }

  // 其他平台使用状态栏高度 + 自定义导航栏高度的兜底值
  return (systemInfo.statusBarHeight || 20) + 44
})

// 计算头像
const avatar = computed(() => {
  return userStore.userInfo?.avatar
})
// 计算窗口高度
const windowHeight = computed(() => {
  return uni.getSystemInfoSync().windowHeight - 50
})
// 跳转到个人信息页面
function handleToInfo() {
  tab.navigateTo('/pages/mine/info/index')
}
// 跳转到编辑资料页面
function handleToEditInfo() {
  tab.navigateTo('/pages/mine/info/edit')
}
// 跳转到修改密码页面
function handleToPwd() {
  tab.navigateTo('/pages/mine/pwd/index')
}
// 跳转到头像页面
function handleToAvatar() {
  modal.showToast({
    title: '模块建设中',
  })
}
// 跳转到登录页面
function handleToLogin() {
  tab.reLaunch('/pages/login')
}
// 跳转到首页设置页面
function handleToHomeConfig() {
  tab.navigateTo('/pages/mine/home-config/index')
}
// 显示建设中提示
function handleBuilding() {
  modal.msg('模块建设中')
}
// 退出登录
function handleLogout() {
  modal.confirm('确定注销并退出系统吗？').then((res) => {
    if (res.confirm) {
      // 退出
      loginApi
        .logout({ token: userStore.accessToken })
        .then(() => {})
        .finally(() => {
          console.log('退出成功')
          // 清除缓存
          userStore.clearUserStore()
          // 跳转登录页面
          console.log('[ router ] >', router)
          router.replaceAll({ name: 'loading' })
        })
    }
    else if (res.cancel) {
      console.log('用户点击取消')
    }
  })
}
</script>

<style lang="scss">
@mixin circular {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 120upx;
  height: 120upx;
  background-color: #fff;
  border-radius: 100%;
}

.mine-container {
  width: 100%;
  height: 100%;

  .header-section {
    display: flex;
    justify-content: space-between;
    padding: 60upx 30upx;
    margin: 15upx;
    color: white;
    background-color: $uni-color-primary;
    border-radius: 5upx;

    .avatar-section {
      display: flex;
      align-items: center;

      .icon {
        @include circular;
      }

      .avatar {
        @include circular;

        border: 4upx solid #eaeaea;
      }

      .user-info {
        margin-left: 15upx;
        font-size: 18upx;

        .title {
          font-size: 40upx;
          line-height: 40upx;
        }
      }
    }

    .avatar-title {
      display: flex;
      align-items: center;
    }
  }

  .content-section {
    position: relative;
    top: 0;
    margin: 15upx;
    background-color: #fff;

    .grid-item-box {
      /* #ifndef APP-NVUE */
      display: flex;
      flex: 1;

      /* #endif */
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 15px 0;

      .text {
        margin-top: 15rpx;
        font-size: 26rpx;
        text-align: center;
      }
    }
  }

  .content-list {
    margin: 15upx;
  }

  .btn-logout {
    margin: 50upx 15upx;
    background-color: $uni-color-primary;
    border-radius: 12rpx;

    --wot-button-border-radius: 5rpx;
  }
}
</style>
