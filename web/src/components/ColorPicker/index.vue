<template>
  <div class="color-picker" @click="forceResize">
    <color-picker v-bind="$attrs" format="hex" :pure-color="props.value" @update:pure-color="update" />
  </div>
</template>

<script setup>
import { ColorPicker } from "vue3-colorpicker";
import "vue3-colorpicker/style.css";

const emit = defineEmits(["update:value"]);

const props = defineProps({
  value: {
    type: String,
    default: "#1890ff"
  }
});

onMounted(() => {
  const currentColor = document.querySelector(".current-color");
  if (currentColor) {
    currentColor.textContent = props.value;
  }
});

const forceResize = () => {
  window.dispatchEvent(new Event("resize"));
};

const update = val => {
  console.log("color-picker", val);
  const currentColor = document.querySelector(".current-color");
  if (currentColor) {
    currentColor.textContent = val;
  }
  emit("update:value", val);
};
</script>

<style scoped lang="scss">
.color-picker {
  .vc-color-wrap {
    width: auto;
  }
  .current-color {
    padding: 0 10px;
    color: #ffffff;
  }
}
</style>
