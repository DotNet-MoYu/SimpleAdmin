<template>
  <l-icon :name="icon" v-bind="$attrs" />
</template>

<script setup lang="ts" name="SvgIcon">
const props = defineProps({
  icon: {
    type: String,
    default: '',
  },
})
</script>

<style scoped></style>
