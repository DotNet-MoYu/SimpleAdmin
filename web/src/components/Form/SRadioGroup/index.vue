<!-- 单选框组件封装 -->
<template>
  <el-radio-group v-bind="$attrs">
    <template v-if="props.button">
      <el-radio-button v-for="(item, index) in options" :key="index" :value="item[props.valueKey]" v-show="item.show !== false">
        <slot name="radio" :item="item"> {{ item[props.labelKey] }} </slot>
      </el-radio-button>
    </template>
    <template v-else>
      <el-radio v-for="(item, index) in options" :key="index" :value="item[props.valueKey]" v-show="item.show !== false">
        <slot name="radio" :item="item"> {{ item[props.labelKey] }}</slot>
      </el-radio>
    </template>
  </el-radio-group>
</template>

<script setup lang="ts" name="SRadioGroup">
import { SRadioGroupProps } from "./interface";

// 定义组件props
const props = withDefaults(defineProps<SRadioGroupProps>(), {
  options: [] as any,
  valueKey: "value",
  labelKey: "label",
  button: false
});
</script>

<style lang="scss" scoped></style>
