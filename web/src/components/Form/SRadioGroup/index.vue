<!-- 单选框组件封装 -->
<template>
  <el-radio-group v-bind="$attrs">
    <template v-if="props.button">
      <el-radio-button v-for="(item, index) in options" :key="index" :label="item[props.value]">
        <slot name="radio" :item="item">
          {{ item[props.label] }}
        </slot>
      </el-radio-button>
    </template>
    <template v-else>
      <el-radio v-for="(item, index) in options" :key="index" :label="item[props.value]">
        <slot name="radio" :item="item">
          {{ item[props.label] }}
        </slot>
      </el-radio>
    </template>
  </el-radio-group>
</template>

<script setup lang="ts">
import { SRadioGroupProps } from "./interface";

// 定义组件props
const props = withDefaults(defineProps<SRadioGroupProps>(), {
  options: [] as any,
  value: "value",
  label: "label",
  button: false
});
</script>

<style lang="scss" scoped></style>
