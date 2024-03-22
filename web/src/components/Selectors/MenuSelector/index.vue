<!-- 
 * @Description: 菜单选择器组件
 * @Author: huguodong 
 * @Date: 2023-12-15 15:39:58
!-->
<template>
  <div class="w-full">
    <el-tree-select
      ref="treeRef"
      v-model="valueMenu"
      class="w-full"
      node-key="id"
      :multiple="multiple"
      :check-strictly="checkStrictly"
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
import { Menu } from "@/api";
// 定义组件props
const props = withDefaults(defineProps<MenuSelectProps>(), {
  clearable: true,
  placeholder: "请选择菜单",
  multiple: false,
  checkStrictly: true,
  showAll: false
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

// 监听父组件传递的值变化
watch(
  () => props.menuValue,
  (val: number | string | string[] | number[]) => {
    valueMenu.value = val;
  }
);

/** 获取菜单树 */
function getMenuTree() {
  // 获取菜单树数据
  props.menuTreeApi().then(res => {
    if (props.showAll) {
      // 加个顶级作为一级菜单
      menuTreeData.value = [
        {
          id: 0,
          parentId: 0,
          title: "顶级",
          children: res.data
        }
      ];
    } else {
      menuTreeData.value = res.data;
    }
  });
}

/** 选中菜单事件 */
function changeMenu(value: number | string | string[] | number[]) {
  valueMenu.value = value;
  console.log("[ value ] >", value);
  //更新父组件数据
  emit("update:menuValue", value);
  emit("change", treeRef.value.getCurrentNode().path);
}

// 暴露给父组件的方法
</script>

<style scoped lang="scss"></style>
