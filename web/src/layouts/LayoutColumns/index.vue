<!-- 分栏布局 -->
<template>
  <el-container class="layout">
    <div class="aside-split">
      <div class="logo flx-center">
        <img class="logo-img" :src="sysInfo.SYS_LOGO" alt="logo" />
      </div>
      <el-scrollbar>
        <div class="split-list">
          <div
            v-for="item in menuList"
            :key="item.path"
            class="split-item"
            :class="{
              'split-active': splitActive === item.path || splitActive.includes(item.path)
            }"
            @click="changeSubMenu(item)"
          >
            <svg-icon :icon="item.meta.icon" class="el-icon" />
            <span class="title">{{ item.meta.title }}</span>
          </div>
        </div>
      </el-scrollbar>
    </div>
    <el-aside :class="{ 'not-aside': !subMenuList.length }" :style="{ width: isCollapse ? '65px' : '210px' }">
      <div class="logo flx-center">
        <span v-show="subMenuList.length" class="logo-text">{{ isCollapse ? "S" : sysInfo.SYS_NAME }}</span>
      </div>
      <el-scrollbar>
        <el-menu :router="false" :default-active="activeMenu" :collapse="isCollapse" :unique-opened="accordion" :collapse-transition="false">
          <SubMenu :menu-list="subMenuList" />
        </el-menu>
      </el-scrollbar>
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

<script setup lang="ts" name="layoutColumns">
import { ref, computed, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAuthStore, useGlobalStore, useConfigStore } from "@/stores/modules";

import Main from "@/layouts/components/Main/index.vue";
import ToolBarLeft from "@/layouts/components/Header/ToolBarLeft.vue";
import ToolBarRight from "@/layouts/components/Header/ToolBarRight.vue";
import SubMenu from "@/layouts/components/Menu/SubMenu.vue";

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const globalStore = useGlobalStore();
const configStore = useConfigStore();
const accordion = computed(() => globalStore.accordion);
const isCollapse = computed(() => globalStore.isCollapse);
const menuList = computed(() => authStore.showMenuListGet);
const activeMenu = computed(() => (route.meta.activeMenu ? route.meta.activeMenu : route.path) as string);
const sysInfo = computed(() => configStore.sysBaseInfoGet);
const subMenuList = ref<Menu.MenuOptions[]>([]);
const splitActive = ref("");
watch(
  () => [menuList, route],
  () => {
    // 当前菜单没有数据直接 return
    if (!menuList.value.length) return;
    splitActive.value = route.path;
    const menuItem = menuList.value.filter((item: Menu.MenuOptions) => {
      const parts = route.path.split("/"); // 将路由按照 '/' 分割成数组
      return route.path === item.path || parts.slice(0, parts.length - 1).join("/") == item.path || `/${parts[1]}` === item.path;
    });
    if (menuItem.length > 0 && menuItem[0].children?.length) return (subMenuList.value = menuItem[0].children);
    subMenuList.value = [];
  },
  {
    deep: true,
    immediate: true
  }
);

// change SubMenu
const changeSubMenu = (item: Menu.MenuOptions) => {
  splitActive.value = item.path;
  if (item.children?.length) return (subMenuList.value = item.children);
  subMenuList.value = [];
  router.push(item.path);
};
</script>

<style scoped lang="scss">
@use "./index";
</style>
