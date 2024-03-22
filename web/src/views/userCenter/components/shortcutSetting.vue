<!-- 
 * @Description: 快捷设置
 * @Author: huguodong 
 * @Date: 2024-03-06 16:09:43
!-->
<template>
  <el-form ref="formRef" :model="formData" label-position="right" label-width="170px" class="pt-4 w-800px" label-suffix=" :" size="large">
    <s-form-item label="默认快捷方式" prop="shortcuts">
      <MenuSelector v-model:menu-value="formData.shortcuts" multiple :check-strictly="false" :menu-tree-api="userCenterApi.shortcutTree" />
    </s-form-item>

    <el-form-item>
      <el-button type="primary" :loading="loading" @click="handleSubmit"> 保存基本信息 </el-button>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { userCenterApi } from "@/api";

const loading = ref(false); // 加载状态
const formData = reactive({
  shortcuts: []
});
onMounted(() => {
  userCenterApi.loginWorkbench().then(res => {
    // 设置组件回显
    formData.shortcuts = JSON.parse(res.data).shortcut;
    console.log("[  formData.shortcuts ] >", formData.shortcuts);
  });
});

/** 更新快捷方式 */
function handleSubmit() {
  const workbenchData = {
    shortcut: formData.shortcuts
  };
  loading.value = true;
  userCenterApi.updateUserWorkbench({ WorkbenchData: JSON.stringify(workbenchData) }).finally(() => {
    loading.value = false;
  });
}
</script>

<style lang="scss" scoped></style>
