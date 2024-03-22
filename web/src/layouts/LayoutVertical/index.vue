<!-- 纵向布局 -->
<template>
  <el-container class="layout">
    <el-aside>
      <div class="aside-box" :style="{ width: isCollapse ? '65px' : '210px' }">
        <div class="logo flx-center">
          <img class="logo-img" :src="sysInfo.SYS_LOGO" alt="logo" />
          <span v-show="!isCollapse" class="logo-text">{{ sysInfo.SYS_NAME }}</span>
        </div>
        <el-scrollbar>
          <el-menu :router="false" :default-active="activeMenu" :collapse="isCollapse" :unique-opened="accordion" :collapse-transition="false">
            <SubMenu :menu-list="menuList" />
          </el-menu>
        </el-scrollbar>
      </div>
    </el-aside>
    <el-container>
      <el-header>
        <ToolBarLeft />
        <ToolBarRight />
      </el-header>
      <Main />
    </el-container>
  </el-container>
</template>

<script setup lang="ts" name="layoutVertical">
import { computed } from "vue";
import { useRoute } from "vue-router";
import { useAuthStore, useGlobalStore, useConfigStore } from "@/stores/modules";
import Main from "@/layouts/components/Main/index.vue";
import ToolBarLeft from "@/layouts/components/Header/ToolBarLeft.vue";
import ToolBarRight from "@/layouts/components/Header/ToolBarRight.vue";
import SubMenu from "@/layouts/components/Menu/SubMenu.vue";

const route = useRoute();
const authStore = useAuthStore();
const globalStore = useGlobalStore();
const configStore = useConfigStore();
const accordion = computed(() => globalStore.accordion);
const isCollapse = computed(() => globalStore.isCollapse);
const menuList = computed(() => authStore.showMenuListGet);
const activeMenu = computed(() => (route.meta.activeMenu ? route.meta.activeMenu : route.path) as string);
const sysInfo = computed(() => configStore.sysBaseInfoGet);
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
