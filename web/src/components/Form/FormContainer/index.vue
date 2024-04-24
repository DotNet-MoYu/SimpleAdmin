<!-- 
 * @Description: 表单容器组件
 * @Author: huguodong 
 * @Date: 2023-12-15 15:38:02
!-->
<template>
  <div class="form-container">
    <!-- 抽屉 -->
    <el-drawer v-if="globalStore.drawerForm" v-model="visible" :destroy-on-close="true" :size="formProps.formSize" v-bind="$attrs" @close="close">
      <template v-for="slotKey in slotKeys" #[slotKey]> <slot :name="slotKey" /></template>
    </el-drawer>
    <!-- 对话框 -->
    <el-dialog v-else top="50px" :visible="visible" :destroy-on-close="true" draggable v-bind="$attrs" @close="close" :width="formProps.formSize">
      <template v-for="slotKey in slotKeys" #[slotKey]> <slot :name="slotKey" /></template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="FormContainer">
import { useGlobalStore } from "@/stores/modules";

const visible = ref(false); // 是否显示表单容器
const globalStore = useGlobalStore(); // 全局状态管理
const instance = getCurrentInstance(); // 当前实例
const slotKeys = computed(() => Object.keys(instance?.slots || [])); // 插槽名称列表
const emit = defineEmits({ close: null }); // 自定义事件
const formProps = defineProps({
  /** 表单大小 */
  formSize: {
    type: String,
    default: "600px"
  }
});

/** 关闭表单容器 */
function close() {
  visible.value = false;
  emit("close");
}
</script>

<style lang="scss" scoped>
.form-container {
  :deep(.el-dialog__body) {
    margin-top: 0;
  }
  :deep(.el-drawer__body) {
    margin-top: -20px;
  }
}
</style>
