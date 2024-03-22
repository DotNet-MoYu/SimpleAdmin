<!-- 
 * @Description: 组织选择器
 * @Author: huguodong 
 * @Date: 2023-12-15 15:40:18
!-->
<template>
  <el-cascader filterable clearable :options="options" :props="cascaderProps" v-bind="$attrs" class="w-full" />
</template>

<script setup lang="ts">
import { PositionSelectProps } from "./interface";
import { SysPosition, sysPositionApi } from "@/api";
// 定义组件props
const cascaderProps = {
  expandTrigger: "hover" as const,
  value: "id",
  label: "name"
};
// 选择器数据
const options = ref<SysPosition.SysPositionSelector[]>([]);
// 定义组件props
const props = withDefaults(defineProps<PositionSelectProps>(), {
  selectorApi: sysPositionApi.selector
});

// 组件挂载时获取选择器数据
onMounted(async () => {
  props.selectorApi().then(res => {
    options.value = res.data;
  });
});

/** 点击事件 */
// const handleChange = (value: any) => {
//   console.log(value);
// };
</script>

<style lang="scss" scoped></style>
