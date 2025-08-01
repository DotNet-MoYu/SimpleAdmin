<template>
  <!-- 抽屉方式 -->
  <el-drawer v-if="globalStore.drawerForm" v-model="visible" :destroy-on-close="true" :size="formProps.formSize" v-bind="$attrs" @close="close">
    <!-- header 插槽 -->
    <template #header>
      <slot name="header" />
    </template>

    <!-- 动态插槽（除特殊插槽） -->
    <template v-for="slotKey in filteredSlotKeys" #[slotKey]>
      <slot :name="slotKey" />
    </template>

    <!-- footer 插槽 -->
    <template #footer>
      <slot name="footer" />
    </template>
  </el-drawer>

  <!-- 对话框方式 -->
  <el-dialog v-else top="50px" v-model="visible" :destroy-on-close="true" draggable v-bind="$attrs" :width="formProps.formSize" @close="close">
    <!-- title 插槽 -->
    <template #header>
      <slot name="title" />
    </template>

    <!-- 动态插槽（除特殊插槽） -->
    <template v-for="slotKey in filteredSlotKeys" #[slotKey]>
      <slot :name="slotKey" />
    </template>

    <!-- footer 插槽 -->
    <template #footer>
      <slot name="footer" />
    </template>
  </el-dialog>
</template>

<script setup lang="ts" name="FormContainer">
import { useGlobalStore } from "@/stores/modules";

const visible = ref(false);
const globalStore = useGlobalStore();
const emit = defineEmits(["close"]);

const formProps = defineProps({
  formSize: {
    type: String,
    default: "600px"
  }
});

const instance = getCurrentInstance();

/** 所有具名插槽键 */
const slotKeys = computed(() => Object.keys(instance?.slots || {}));

/** 过滤掉 header/footer/title 等固定插槽 */
const filteredSlotKeys = computed(() => slotKeys.value.filter(k => !["header", "footer", "title"].includes(k)));

/** 关闭弹窗 */
function close() {
  visible.value = false;
  emit("close");
}
</script>

<style scoped lang="scss"></style>
