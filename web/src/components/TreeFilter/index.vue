<template>
  <div class="card filter">
    <h4 v-if="title" class="title sle">
      {{ title }}
    </h4>
    <el-input v-model="filterText" placeholder="输入关键字进行过滤" clearable />
    <el-scrollbar :style="{ height: title ? `calc(100% - 95px)` : `calc(100% - 56px)` }">
      <el-tree
        ref="treeRef"
        :default-expand-all="defaultExpandAll"
        :node-key="id"
        :data="multiple ? treeData : treeAllData"
        :show-checkbox="multiple"
        :check-strictly="false"
        :current-node-key="!multiple ? selected : ''"
        :highlight-current="!multiple"
        :expand-on-click-node="false"
        :check-on-click-node="multiple"
        :props="defaultProps"
        :filter-node-method="filterNode"
        :default-checked-keys="multiple ? selected : []"
        :default-expanded-keys="getDefaultExpandKeys"
        @node-click="handleNodeClick"
        @check="handleCheckChange"
      >
        <template #default="scope">
          <span class="el-tree-node__label">
            <slot :row="scope">
              {{ scope.node.label }}
            </slot>
          </span>
        </template>
      </el-tree>
    </el-scrollbar>
  </div>
</template>

<script setup lang="ts" name="TreeFilter">
import { ref, watch, onBeforeMount, nextTick } from "vue";
import { ElTree } from "element-plus";

// 接收父组件参数并设置默认值
interface TreeFilterProps {
  requestApi?: (data?: any) => Promise<any>; // 请求分类数据的 api ==> 非必传
  data?: { [key: string]: any }[]; // 分类数据，如果有分类数据，则不会执行 api 请求 ==> 非必传
  title?: string; // treeFilter 标题 ==> 非必传
  id?: string; // 选择的id ==> 非必传，默认为 “id”
  label?: string; // 显示的label ==> 非必传，默认为 “label”
  multiple?: boolean; // 是否为多选 ==> 非必传，默认为 false
  defaultValue?: any; // 默认选中的值 ==> 非必传
  defaultExpandAll?: boolean; // 是否默认展开所有节点 ==> 非必传，默认为 true
  defaultExpandLevel?: number; // 默认展开的层级 ==> 非必传，默认为 1,如果 defaultExpandAll 为 true，则此参数无效
}
const props = withDefaults(defineProps<TreeFilterProps>(), {
  id: "id",
  label: "label",
  multiple: false,
  defaultExpandAll: true,
  defaultExpandLevel: 1
});

const defaultProps = {
  children: "children",
  label: props.label
};

const treeRef = ref<InstanceType<typeof ElTree>>();
const treeData = ref<{ [key: string]: any }[]>([]);
const treeAllData = ref<{ [key: string]: any }[]>([]);

const selected = ref();
const setSelected = () => {
  if (props.multiple) selected.value = Array.isArray(props.defaultValue) ? props.defaultValue : [props.defaultValue];
  else selected.value = typeof props.defaultValue === "string" ? props.defaultValue : "";
};

onBeforeMount(async () => {
  setSelected();
  await getRequestData();
});

// 使用 nextTick 防止打包后赋值不生效，开发环境是正常的
watch(
  () => props.defaultValue,
  () => nextTick(() => setSelected()),
  { deep: true, immediate: true }
);

watch(
  () => props.data,
  () => {
    if (props.data?.length) {
      treeData.value = props.data;
      treeAllData.value = [{ id: "", [props.label]: "全部" }, ...props.data];
    }
  },
  { deep: true, immediate: true }
);

const filterText = ref("");
watch(filterText, val => {
  treeRef.value!.filter(val);
});

// 过滤
const filterNode = (value: string, data: { [key: string]: any }, node: any) => {
  if (!value) return true;
  let parentNode = node.parent,
    labels = [node.label],
    level = 1;
  while (level < node.level) {
    labels = [...labels, parentNode.label];
    parentNode = parentNode.parent;
    level++;
  }
  return labels.some(label => label.indexOf(value) !== -1);
};

// emit
const emit = defineEmits<{
  change: [value: any, data?: any];
}>();

// 单选
const handleNodeClick = (data: { [key: string]: any }) => {
  if (props.multiple) return;
  emit("change", data[props.id], data);
};

// 多选
const handleCheckChange = () => {
  emit("change", treeRef.value?.getCheckedKeys());
};

const refresh = async () => {
  treeRef.value?.setCheckedKeys([]);
  treeRef.value?.setCurrentKey("");
  setSelected();
  await getRequestData();
};

const getRequestData = async () => {
  const { data } = await props.requestApi!();
  treeData.value = data;
  treeAllData.value = [{ id: "", [props.label]: "全部" }, ...data];
};

/** 获取默认展开层级 */
const getDefaultExpandKeys = computed(() => {
  //判断是否为默认展开全部
  if (!props.defaultExpandAll) {
    //根据默认展开层级,将对应的id放入数组
    let ids: any[] = [];
    const getIds = (data: any[], level: number) => {
      data.forEach((item: any) => {
        if (item.children && item.children.length > 0 && level >= 1) {
          //递归调用
          getIds(item.children, level - 1);
          ids.push(item.id);
        }
      });
    };
    //调用函数传入数据和默认展开层级
    getIds(treeAllData.value, props.defaultExpandLevel);
    return ids;
  }
  return [];
});

// 暴露给父组件使用
defineExpose({ treeData, treeAllData, treeRef, refresh });
</script>

<style scoped lang="scss">
@import "./index.scss";
</style>
