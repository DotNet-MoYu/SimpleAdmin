<!-- 操作按钮封装 -->
<template>
  <el-button type="primary" :icon="icon" v-bind="$attrs">
    <slot>{{ getTitle }}</slot>
  </el-button>
</template>

<script setup lang="ts" name="SButton">
import { CirclePlus, EditPen, Delete, View } from "@element-plus/icons-vue";
// import { useAuthButtons } from "@/hooks/useAuthButtons";
import { FormOptEnum } from "@/enums";
// const instance = getCurrentInstance(); // 当前实例
// const slotKeys = computed(() => Object.keys(instance?.slots || [])); // 插槽名称列表
//接口
interface Props {
  opt?: FormOptEnum; //操作
  prefix?: string; //前缀
  suffix?: string; //标题
  permission?: string[] | string;
}
//默认值
const props = withDefaults(defineProps<Props>(), {
  opt: FormOptEnum.ADD,
  suffix: ""
});

//根据操作类型绑定按钮图标
const icon = computed(() => {
  switch (props.opt) {
    case FormOptEnum.ADD:
      return CirclePlus;
    case FormOptEnum.EDIT:
      return EditPen;
    case FormOptEnum.DELETE:
      return Delete;
    case FormOptEnum.VIEW:
      return View;
    default:
      return CirclePlus;
  }
});

//获取按钮标题,如果没有前缀,默认就是操作类型+标题
const getTitle = computed(() => {
  if (props.prefix) {
    return props.prefix + props.suffix;
  } else {
    return props.opt + props.suffix;
  }
});
</script>

<style lang="scss" scoped></style>
