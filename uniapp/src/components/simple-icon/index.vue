<template>
  <view
    class="container-icon"
    :style="{ backgroundColor, width: size, height: size }"
  >
    <svg-icon v-bind="$attrs" :size="iconSize" />
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
  size: {
    type: String,
    required: false,
    default: '80upx',
  },
  iconSize: {
    type: String,
    required: false,
    default: '48upx',
  },
})
</script>

<style lang="scss" scoped>
.container-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 25upx;
  box-shadow: 1upx 4upx 5upx rgb(72 22 174 / 30%);
}
</style>
