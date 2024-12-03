<!-- 
 * @Description: 日期选择器
 * @Author: huguodong 
 * @Date: 2023-12-15 15:37:57
!-->
<template>
  <el-date-picker
    clearable
    class="w-full"
    :placeholder="placeholder"
    :disabled-date="disabledDateData"
    :shortcuts="shortcutsData"
    :v-bind="$attrs"
    :value-format="props.format"
  />
</template>

<script setup lang="ts" name="SDatePicker">
import { formItemContextKey } from "element-plus";

const formItemContext = inject(formItemContextKey, undefined); //表单Item实例

const placeholder = computed(() => {
  return "请选择" + formItemContext?.label;
});

//接口
interface Props {
  /** 是否显示快捷选项 */
  showShortcuts?: boolean;
  /** 快捷选项 */
  shortcuts?: any[];
  /** 禁止选择的日期 */
  disabledDate?: (time: Date) => boolean;
  /** 禁止选择今天之后的日期 */
  disabledDateAfter?: boolean;
  /** 禁止选择今天之前的日期 */
  disabledDateBefore?: boolean;
  /** 日期格式 */
  format?: string;
}
//默认值
const props = withDefaults(defineProps<Props>(), {
  shortcuts: () => [],
  showShortcuts: true,
  disabledDate: undefined,
  disabledDateAfter: false,
  disabledDateBefore: true,
  format: "YYYY-MM-DD HH:mm:ss"
});

const shortcutsData = computed(() => {
  //是否显示快捷选项
  if (props.showShortcuts) {
    //如果有快捷选项
    if (props.shortcuts.length > 0) {
      return props.shortcuts;
    } else {
      return defaultShortcuts;
    }
  }
});

/**
 * 快捷选项
 */
const defaultShortcuts = [
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
 *  禁止选择的日期,在当前日期之后
 */
const disabledDateData = (time: Date) => {
  //如果有自定义禁止选择的日期
  if (props.disabledDate) {
    return props.disabledDate;
  } else {
    //如果没有自定义禁止选择的日期，则使用默认的禁止选择的日期
    if (props.disabledDateAfter) {
      return time.getTime() > Date.now();
    } else if (props.disabledDateBefore) {
      return time.getTime() < Date.now();
    }
  }
};
</script>

<style lang="scss" scoped></style>
