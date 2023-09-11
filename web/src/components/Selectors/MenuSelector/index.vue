<!-- 菜单选择器组件 -->
<template>
  <div class="w-full">
    <el-tree-select
      ref="treeRef"
      v-model="valueMenu"
      class="w-full"
      node-key="id"
      check-strictly
      :placeholder="placeholder"
      :data="menuTreeData"
      :clearable="clearable"
      :props="treeProps"
      :render-after-expand="false"
      :default-expanded-keys="defaultExpandedKeys"
      @change="changeMenu"
    >
    </el-tree-select>
  </div>
</template>

<script setup lang="ts" name="MenuSelector">
import { MenuSelectProps } from "./interface";
import { menuTreeApi, Menu } from "@/api";
// 定义组件props
const props = withDefaults(defineProps<MenuSelectProps>(), {
  menuValue: "",
  clearable: true,
  placeholder: "请选择菜单"
});
// 菜单树的引用
const treeRef = ref();
// 定义tree组件自定义 props
const treeProps = {
  label: "title",
  value: "id"
};
// 默认展开的节点(顶级)
const defaultExpandedKeys = ref([0]);
// 组件挂载时获取菜单树数据
onMounted(() => {
  getMenuTree();
});
//菜单树数据
const menuTreeData = ref<Menu.MenuTreeInfo[]>([]);
// 定义响应式变量
const valueMenu = ref(props.menuValue); // 当前选中的菜单名称
//  定义方法
const emit = defineEmits(["update:menuValue", "change"]); // 定义更新父组件数据的方法

/** 获取菜单树 */
function getMenuTree() {
  // 获取菜单树数据
  menuTreeApi({ module: props.module }).then(res => {
    // 加个顶级作为一级菜单
    menuTreeData.value = [
      {
        id: 0,
        parentId: 0,
        title: "顶级",
        children: res.data
      }
    ];
  });
}

/** 选中菜单事件 */
function changeMenu(value: number | string) {
  valueMenu.value = value;
  //更新父组件数据
  emit("update:menuValue", value);
  emit("change", treeRef.value.getCurrentNode().path);
}
</script>

<style scoped lang="scss"></style>