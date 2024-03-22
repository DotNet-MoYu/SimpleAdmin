<!-- 
 * @Description: 代码高亮组件
 * @Author: huguodong 
 * @Date: 2023-12-15 15:37:43
!-->
<template>
  <div :codetype="props.language">
    <highlightjs :language="props.language" :autodetect="!props.language" :code="props.code"></highlightjs>
  </div>
</template>

<script setup lang="ts" name="codeHighLight">
const props = defineProps({
  language: {
    type: String,
    default: () => undefined
  },
  code: {
    type: String,
    default: () => "无"
  }
});
</script>

<style lang="scss" scoped>
/** 滚动条 */
:deep(.hljs, .hljs-container) {
  max-height: 300px !important;
  overflow-x: auto;
}
:deep(.hljs::-webkit-scrollbar) {
  width: 12px !important;
  height: 12px !important;
}
:deep(.hljs::-webkit-scrollbar-thumb) {
  height: 30px !important;
  background: #d1d8e6;
  background-clip: content-box;
  border: 2px solid transparent;
  border-radius: 19px;
  opacity: 0.8;
}
:deep(.hljs::-webkit-scrollbar-thumb:hover) {
  background: #a5b3cf;
  background-clip: content-box;
  border: 2px solid transparent;
}
:deep(.hljs::-webkit-scrollbar-track-piece) {
  width: 30px;
  height: 30px;
  background: #333333;
}
::-webkit-scrollbar-button {
  display: none;
}
</style>
