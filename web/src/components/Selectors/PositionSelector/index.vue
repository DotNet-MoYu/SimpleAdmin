<!-- 组织选择器  -->
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

const options = ref<SysPosition.SysPositionSelector[]>([]);
// 定义组件props
const props = withDefaults(defineProps<PositionSelectProps>(), {
  permission: false
});

// 组件挂载时获取组织树
onMounted(async () => {
  await getPositionSelector().then(res => {
    options.value = res.data;
  });
});

/** 获取组织树 */
function getPositionSelector(): Promise<any> {
  if (props.permission) {
    return sysPositionApi.positionSelector();
  }
  return sysPositionApi.positionSelector();
}

/** 点击事件 */
// const handleChange = (value: any) => {
//   console.log(value);
// };
</script>

<style lang="scss" scoped></style>
