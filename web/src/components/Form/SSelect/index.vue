<!-- 
 * @Description: 选择框组件封装
 * @Author: huguodong 
 * @Date: 2023-12-15 15:38:32
!-->
<template>
  <el-select :placeholder="placeholder" class="w-full" v-bind="$attrs" clearable>
    <el-option v-for="(item, index) in options" :key="index" :label="item[props.label]" :value="item[props.value]" />
  </el-select>
</template>

<script setup lang="ts" name="SSelect">
import { SSelectProps } from "./interface";
import { formItemContextKey } from "element-plus";

const formItemContext = inject(formItemContextKey, undefined); //表单Item实例

// 定义组件props
const props = withDefaults(defineProps<SSelectProps>(), {
  options: [] as any,
  value: "value",
  label: "label",
  button: false
});

const placeholder = computed(() => {
  return "请选择" + (formItemContext?.label || "");
});
</script>

<style lang="scss" scoped></style>
