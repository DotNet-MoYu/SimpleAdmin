<!-- 输入框封装 -->
<template>
  <el-input :placeholder="placeholder" clearable v-bind="$attrs" />
</template>

<script setup lang="ts" name="SInput">
import { formItemContextKey } from "element-plus";

const formItemContext = inject(formItemContextKey, undefined); //表单Item实例

const placeholder = computed(() => {
  return "请填写" + (formItemContext?.label || "");
});
</script>

<style lang="scss" scoped></style>
