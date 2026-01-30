<!--
 * @Description:
 * @Author: huguodong
 * @Date: 2025-07-12 16:44:27
! -->
<template>
  <view>
    <!-- 轮播图 -->
    <view>
      <home-swiper />
    </view>
    <!-- 图表 插件地址：https://ext.dcloud.net.cn/plugin?id=271 官方地址：https://www.ucharts.cn/v2/#/demo/index -->
    <view class="item">
      <home-chart />
    </view>
  </view>
</template>

<script setup lang="ts">
import HomeChart from './components/home-chart.vue'
import HomeSwiper from './components/home-swiper.vue'

definePage(
  {
    layout: 'default',
    name: 'home',
    style: {
    // 'custom' 表示开启自定义导航栏，默认 'default'
      navigationStyle: 'custom',
      navigationBarTitleText: '首页',
    },
  },
)
</script>

<style lang="scss" scoped>
.item {
  padding: 30upx 15upx;
  margin: 15upx 0;
  background-color: #fff;
}
</style>
