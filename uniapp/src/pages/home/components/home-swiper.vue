<template>
  <wd-swiper
    v-model:current="current"
    :list="swiperList"
    autoplay
    @click="handleClick"
    @change="onChange"
  />
</template>

<script setup lang="ts">
const current = ref<number>(0)

const swiperList = ref([
  'https://picsum.photos/800/400?random=1',
  'https://picsum.photos/800/400?random=2',
  'https://picsum.photos/800/400?random=3',
])
function handleClick(e: any) {
  console.log(e)
}
function onChange(e: any) {
  console.log(e)
}
</script>

<style></style>
