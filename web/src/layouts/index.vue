<!-- 💥 这里是一次性加载 LayoutComponents -->
<template>
  <el-watermark id="watermark" :font="font" :content="watermark ? ['Simple Admin', 'Happy Working'] : ''">
    <component :is="LayoutComponents[layout]" />
    <ThemeDrawer />
  </el-watermark>
</template>

<script setup lang="ts" name="layout">
import { computed, reactive, watch, type Component } from "vue";
import { LayoutType } from "@/stores/interface";
import { useGlobalStore, useMqttStore, useMessageStore } from "@/stores/modules";
import ThemeDrawer from "./components/ThemeDrawer/index.vue";
import LayoutVertical from "./LayoutVertical/index.vue";
import LayoutClassic from "./LayoutClassic/index.vue";
import LayoutTransverse from "./LayoutTransverse/index.vue";
import LayoutColumns from "./LayoutColumns/index.vue";

const LayoutComponents: Record<LayoutType, Component> = {
  vertical: LayoutVertical,
  classic: LayoutClassic,
  transverse: LayoutTransverse,
  columns: LayoutColumns
};

const globalStore = useGlobalStore();
const isDark = computed(() => globalStore.isDark);
const layout = computed(() => globalStore.layout);
const watermark = computed(() => globalStore.watermark);
const mqttStore = useMqttStore(); //mqttStore
const messageStore = useMessageStore(); //messageStore
const openMqtt = import.meta.env.VITE_MQTT === "true"; //mqtt开关
const font = reactive({ color: "rgba(0, 0, 0, .15)" });
watch(isDark, () => (font.color = isDark.value ? "rgba(255, 255, 255, .15)" : "rgba(0, 0, 0, .15)"), {
  immediate: true
});

onMounted(async () => {
  await initMqtt();
});

onUnmounted(() => {
  if (openMqtt) {
    mqttStore.disconnect(); //销毁mqtt
  }
});

/**初始化mqtt */
async function initMqtt() {
  if (openMqtt) {
    await mqttStore.initMqttClient(); //初始化mqtt
  } else {
    await messageStore.getNewMessageInterval(); //定时获取新消息
  }
}
</script>

<style scoped lang="scss">
.layout {
  min-width: 600px;
}
</style>
