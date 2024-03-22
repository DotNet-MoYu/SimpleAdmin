<!-- 
 * @Description: 可选卡片
 * @Author: huguodong 
 * @Date: 2023-12-15 15:34:54 
!-->
<template>
  <n-space>
    <!-- 遍历选项列表 -->
    <n-el v-for="(item, index) in props.options" :key="index" tag="div" class="check-list" :style="{ width: getListWidth }">
      <!-- 选项列表 -->
      <n-list
        :bordered="props.bordered"
        :hoverable="props.hoverable"
        :clickable="item.disabled !== true"
        :class="{
          'check-list-checked': isChecked(item.id),
          'check-list-checked-disabled': item.disabled === true,
          '.check-list': true
        }"
        @click="handleChecked(item)"
      >
        <n-list-item class="check-list">
          <!-- 选项前缀 -->
          <template #prefix>
            <svg-icon :icon="getAvatar(item.icon)" class="h-12 w-12" />
          </template>
          <!-- 选项标题和描述 -->
          <n-thing :title="item.title" :description="item.description"></n-thing>
          <!-- <el-space direction="vertical">
            <el-text class="mx-1">Default</el-text>
            <el-text class="mx-1">Default</el-text>
          </el-space> -->
        </n-list-item>
      </n-list>
    </n-el>
  </n-space>
</template>

<script setup lang="ts">
import { NSpace, NEl, NList, NListItem, NThing } from "naive-ui";
import { cssUnit } from "@/utils/calculate";
import { basicProps } from "./props";

const emit = defineEmits(["change"]); // 定义组件的事件
const props = defineProps(basicProps); // 定义组件的props
const checkedValues = ref<number[] | string[]>([]); // 选中的选项列表

// 计算选项列表的宽度
const getListWidth = computed(() => {
  return cssUnit(props.width) ?? "auto";
});

// 计算选项列表的宽度
const getAvatar = computed(() => (icon?: string) => {
  if (icon && icon.length > 0) {
    return icon;
  }

  return "local:logo";
});

// 监听props.value的变化
watch(
  () => props.value,
  (values: any) => {
    if (!values) return;
    // 如果是单选，则只选中最后一个选项
    checkedValues.value = !props.multiple ? [values[values.length - 1]] : values;
  },
  {
    immediate: true
  }
);

// 处理选项的选中状态
function handleChecked(item: CheckCard.OptionsConfig) {
  const id = item.id;
  if (item.disabled === true) return; // 如果选项被禁用，则不处理
  // 如果是多选,则判断选项是否已经被选中，如果已经被选中，则从选中列表中移除，否则添加到选中列表中
  if (props.multiple) {
    if (checkedValues.value?.includes(id as never)) {
      checkedValues.value.splice(checkedValues.value.indexOf(id as never), 1);
    } else {
      checkedValues.value?.push(id as never);
    }
  } else {
    checkedValues.value = checkedValues.value?.includes(id as never) ? [] : [id as never];
  }
  emit("change", checkedValues.value);
}

// 判断选项是否被选中
function isChecked(value: number | string) {
  return checkedValues.value?.includes(value as never);
}
</script>

<style scoped>
.check-list {
  background-color: var(--el-bg-color);
}
:deep(.n-thing-header__title) {
  color: var(--el-text-color-primary) !important;
}
:deep(.n-list) {
  border: 1px solid var(--el-border-color-lighter);
}
.check-list ul :deep(.n-thing-main__description) {
  color: var(--text-color-3);
}
.check-list-checked-disabled {
  cursor: not-allowed !important;
  background-color: var(--el-border-color-lighter);
  border-color: var(--el-border-color-lighter);
}
.check-list-checked {
  position: relative;
  border-color: var(--el-color-primary);
}
.check-list-checked::after {
  position: absolute;
  top: 2px;
  right: 2px;
  width: 0;
  height: 0;
  content: "";
  border: 6px solid var(--el-color-primary);
  border-bottom: 6px solid transparent;
  border-left: 6px solid transparent;
  border-top-right-radius: 2px;
}
</style>
