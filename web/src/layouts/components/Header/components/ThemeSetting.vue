<template>
  <div class="theme-setting">
    <el-tooltip content="切换主题" placement="bottom" effect="light">
      <i :class="'iconfont icon-zhuti'" class="toolBar-icon" @click="openDrawer"></i>
    </el-tooltip>
  </div>
</template>

<script setup lang="ts">
import mittBus from "@/utils/mittBus";
const openDrawer = () => {
  mittBus.emit("openThemeDrawer");
};
</script>
