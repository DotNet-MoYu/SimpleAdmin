<!-- 
 * @Description: 日期选择器
 * @Author: huguodong 
 * @Date: 2023-12-15 15:37:57
!-->
<template>
  <el-date-picker
    type="date"
    clearable
    class="w-full"
    :placeholder="placeholder"
    :disabled-date="disabledDate"
    :shortcuts="shortcuts"
    value-format="YYYY-MM-DD"
    :v-bind="$attrs"
  />
</template>

<script setup lang="ts" name="SDatePicker">
import { formItemContextKey } from "element-plus";

const formItemContext = inject(formItemContextKey, undefined); //表单Item实例

const placeholder = computed(() => {
  return "请选择" + formItemContext?.label;
});
/**
 * 快捷选项
 */
const shortcuts = [
  {
    text: "今天",
    value: new Date()
  },
  {
    text: "昨天",
    value: () => {
      const date = new Date();
      date.setTime(date.getTime() - 3600 * 1000 * 24);
      return date;
    }
  },
  {
    text: "一周前",
    value: () => {
      const date = new Date();
      date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
      return date;
    }
  }
];

/**
 *  禁止选择的日期
 */
const disabledDate = (time: Date) => {
  return time.getTime() > Date.now();
};
</script>

<style lang="scss" scoped></style>
