<template>
  <div class="footer flx-center">
    <a :href="props.sysCopyrightUrl" target="_blank"> {{ props.sysCopyright }} </a>
    <a v-for="link in props.footerLinks" :key="link.name" :href="link.url" target="_blank" class="mx-1">&nbsp;|&nbsp;{{ link.name }}</a>
  </div>
</template>
<script setup lang="ts">
import { SysConfig } from "@/api";
import { useConfigStore } from "@/stores/modules";
const configStore = useConfigStore();

interface Footer {
  /** 系统名称 */
  sysCopyright: string;
  /** 系统版本 */
  sysCopyrightUrl: string;
  /** 底部超链接 */
  footerLinks: SysConfig.FooterLinkProps[];
}

//默认值
const props = reactive<Footer>({
  sysCopyright: "",
  sysCopyrightUrl: "",
  footerLinks: []
});

onMounted(() => {
  // 获取系统配置,设置版权信息
  configStore.getSysBaseInfo().then(res => {
    props.sysCopyright = res.SYS_COPYRIGHT;
    props.sysCopyrightUrl = res.SYS_COPYRIGHT_URL;
    props.footerLinks = res.SYS_FOOTER_LINKS;
  });
});
</script>
<style scoped lang="scss">
@use "./index";
</style>
