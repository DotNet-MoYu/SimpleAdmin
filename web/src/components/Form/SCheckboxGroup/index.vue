<!-- 多选框组件封装 -->
<template>
  <el-checkbox-group v-bind="$attrs">
    <template v-if="props.button">
      <el-checkbox-button v-for="(item, index) in options" :key="index" :label="item[props.value]" v-show="item.show !== false">
        <slot name="checkbox" :item="item">
          {{ item[props.label] }}
        </slot>
      </el-checkbox-button>
    </template>
    <template v-else>
      <el-checkbox v-for="(item, index) in options" :key="index" :label="item[props.value]" v-show="item.show !== false">
        <slot name="checkbox" :item="item">
          {{ item[props.label] }}
        </slot>
      </el-checkbox>
    </template>
  </el-checkbox-group>
</template>

<script setup lang="ts" name="SCheckboxGroup">
import { SCheckboxGroupProps } from "./interface";

// 定义组件props
const props = withDefaults(defineProps<SCheckboxGroupProps>(), {
  options: [] as any,
  value: "value",
  label: "label",
  button: false,
  group: false
});
</script>

<style lang="scss" scoped></style>
