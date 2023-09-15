<!-- 表单Item组件封装 -->
<template>
  <el-form-item ref="formItemRef" v-bind="$attrs">
    <slot></slot>
    <template #label>
      <el-space :size="3">
        <el-tooltip v-if="tooltip" effect="dark" :content="props.tooltip" placement="top">
          <svg-icon icon="uiw:question-circle-o"></svg-icon>
        </el-tooltip>
        {{ $attrs.label }}
      </el-space>
    </template>
  </el-form-item>
</template>

<script setup lang="ts" name="SFormItem">
import { FormItemContext } from "element-plus";

const formItemRef = ref<FormItemContext>();

interface Props {
  tooltip?: string | undefined;
}

//默认值
const props = withDefaults(defineProps<Props>(), {
  tooltip: undefined
});

console.log("[ props ] >", props);
</script>

<style lang="scss" scoped></style>
