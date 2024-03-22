<!-- 
 * @Description: 组织选择器组件
 * @Author: huguodong 
 * @Date: 2023-12-15 15:40:05
!-->
<template>
  <div class="w-full">
    <el-tree-select
      ref="treeRef"
      v-model="valueOrg"
      class="w-full"
      node-key="id"
      check-strictly
      default-expand-all
      :placeholder="placeholder"
      :data="orgTreeData"
      :clearable="clearable"
      :props="treeProps"
      :render-after-expand="false"
      :default-expanded-keys="defaultExpandedKeys"
      @change="changeOrg"
    >
    </el-tree-select>
  </div>
</template>

<script setup lang="ts" name="OrgSelector">
import { OrgSelectProps } from "./interface";
import { SysOrg, sysOrgApi } from "@/api";
// 定义组件props
const props = withDefaults(defineProps<OrgSelectProps>(), {
  orgValue: "",
  clearable: true,
  placeholder: "请选择部门",
  showAll: true,
  orgTreeApi: sysOrgApi.tree
});
// 组织树的引用
const treeRef = ref();
// 定义tree组件自定义 props
const treeProps = {
  label: "name",
  value: "id"
};
// 默认展开的节点(顶级)
const defaultExpandedKeys = ref([0]);
// 组件挂载时获取组织树数据
onMounted(() => {
  getOrgTree();
});
//组织树数据
const orgTreeData = ref<SysOrg.SysOrgTree[]>([]);
// 定义响应式变量
const valueOrg = ref(props.orgValue); // 当前选中的组织名称
//  定义方法
const emit = defineEmits(["update:orgValue", "change"]); // 定义更新父组件数据的方法

/** 获取组织树 */
function getOrgTree() {
  // 获取组织树数据
  props.orgTreeApi().then(res => {
    //如果showAll为true,则在根节点前加一个全部节点
    if (props.showAll) {
      // 加个顶级作为一级组织
      orgTreeData.value = [
        {
          id: 0,
          parentId: 0,
          name: "顶级",
          children: res.data
        }
      ];
    } else {
      orgTreeData.value = res.data; //不加顶级
      valueOrg.value = orgTreeData.value[0].id; //默认选中第一个
      emit("update:orgValue", orgTreeData.value[0].id); //更新父组件数据
    }
  });
}

/** 选中组织事件 */
function changeOrg(value: number | string) {
  valueOrg.value = value; //更新当前选中的组织名称
  emit("update:orgValue", value); //更新父组件数据
  emit("change", treeRef.value.getCurrentNode().path); //触发change事件
}
</script>

<style scoped lang="scss"></style>
