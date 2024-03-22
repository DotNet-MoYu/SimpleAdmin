<!-- 
 * @Description: 表单Item组件封装
 * @Author: huguodong 
 * @Date: 2023-12-15 15:38:14
!-->
<template>
  <el-form-item ref="formItemRef" v-bind="$attrs">
    <slot></slot>
    <template #label>
      <el-space :size="3" v-if="tooltip">
        <el-tooltip effect="dark" :content="props.tooltip" placement="top">
          <svg-icon icon="uiw:question-circle-o"></svg-icon>
        </el-tooltip>
        {{ $attrs.label }}
      </el-space>
      <span v-else-if="$attrs.label">{{ $attrs.label }}{{ formContext?.labelSuffix }}</span>
    </template>
  </el-form-item>
</template>

<script setup lang="ts" name="SFormItem">
import { FormItemContext, formContextKey } from "element-plus";

const formContext = inject(formContextKey, undefined); //表单实例
const formItemRef = ref<FormItemContext>(); //表单Item实例

interface Props {
  tooltip?: string | undefined;
}

//默认值
const props = withDefaults(defineProps<Props>(), {
  tooltip: undefined
});
</script>

<style lang="scss" scoped></style>
