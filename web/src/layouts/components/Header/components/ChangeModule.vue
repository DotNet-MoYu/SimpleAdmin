<template>
  <div>
    <el-tooltip content="切换模块" placement="bottom" effect="light">
      <i :class="'iconfont icon-change'" class="toolBar-icon" @click="changeModule"></i>
    </el-tooltip>
    <ChooseModule ref="changeModuleRef" />
  </div>
</template>

<script setup lang="ts">
import ChooseModule from "@/components/ChooseModule/index.vue";
const changeModuleRef = ref<InstanceType<typeof ChooseModule> | null>(null);

/** 切换应用 **/
function changeModule() {
  changeModuleRef.value?.openDialog();
}
</script>
