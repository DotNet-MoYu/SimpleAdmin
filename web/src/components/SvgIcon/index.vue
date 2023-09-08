<template>
  <svg v-if="renderLocalIcon" aria-hidden="true" width="1em" height="1em" v-bind="bindAttrs">
    <use :xlink:href="symbolId" fill="currentColor" />
  </svg>
  <Icon v-else :icon="icon" width="1em" height="1em" v-bind="bindAttrs" />
</template>

<script setup lang="ts" name="SvgIcon">
import { computed, useAttrs } from "vue";
import { Icon } from "@iconify/vue";

// props
const props = defineProps({
  icon: {
    type: String,
    default: ""
  }
});

const attrs = useAttrs();

const bindAttrs = computed<{ class: string; style: string }>(() => ({
  class: (attrs.class as string) || "",
  style: (attrs.style as string) || ""
}));

const symbolId = computed(() => {
  const defaultLocalIcon = "no-icon";

  const icon = props.icon || defaultLocalIcon;
  return `#local-${icon.split(":")[1]}`;
});

/** 渲染本地icon */
const renderLocalIcon = computed(() => {
  if (props.icon) return props.icon.split(":")[0] === "local"; //如果是iconify图标，就不渲染本地图标
  return false;
});
</script>

<style scoped></style>
