<template>
  <view class="container-icon" :style="{ backgroundColor }">
    <svg-icon v-bind="$attrs" />
  </view>
</template>

<script setup lang="ts" name="SimpleIcon">
import SvgIcon from '@/components/svg-icon/index.vue'

defineProps({
  backgroundColor: {
    type: String,
    required: false,
    default: '#FFFFFF',
  },
})
</script>

<style lang="scss" scoped>
.container-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 80upx;
  height: 80upx;
  border-radius: 25upx;
  box-shadow: 1upx 4upx 5upx rgb(72 22 174 / 30%);
}
</style>
