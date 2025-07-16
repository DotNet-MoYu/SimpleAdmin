<!-- 
 * @Description: 图标选择组件
 * @Author: huguodong 
 * @Date: 2023-12-15 15:38:47
!-->
<template>
  <div class="icon-box">
    <el-input
      ref="inputRef"
      v-model="valueIcon"
      v-bind="$attrs"
      :placeholder="placeholder"
      :clearable="clearable"
      @clear="clearIcon"
      @click="openDialog"
    >
      <template #append>
        <el-button>
          <template #icon> <svg-icon :icon="iconValue" /></template>
        </el-button>
      </template>
    </el-input>
    <el-dialog v-model="dialogVisible" :title="title" top="50px" width="66%">
      <el-tabs tab-position="left" v-model="activeName" class="min-h-500px" stretch>
        <el-input v-model="inputValue" placeholder="搜索图标" size="large" :prefix-icon="Search" />
        <keep-alive>
          <el-tab-pane v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.prefix">
            <el-scrollbar v-if="getIconsByTab.length">
              <div class="icon-list">
                <div v-for="item in getIconsByTab" :key="item" class="icon-item" @click="selectIcon(getIcon(item))">
                  <svg-icon :icon="getIcon(item)" class="h-12 w-12"></svg-icon>
                  <span>{{ item }}</span>
                </div>
              </div>
            </el-scrollbar>
            <el-empty v-else description="未搜索到您要找的图标~" />
          </el-tab-pane>
        </keep-alive>
        <el-tab-pane label="更多图标" name="more">
          <div v-if="inputValue.length" class="icon-list">
            <div class="icon-item" @click="selectIcon(inputValue)">
              <svg-icon :icon="inputValue" class="h-12 w-12"></svg-icon>
              <span>{{ inputValue }}</span>
            </div>
          </div>
          <el-empty v-else description="请输入您想要的图标iconify代码,如mdi:home-variant">
            <span>iconify地址:</span>
            <el-link type="primary" target="_blank" :underline="false" href="https://icones.js.org/">{{ "https://icones.js.org/" }}</el-link>
          </el-empty>
        </el-tab-pane>
      </el-tabs>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="SelectIconPlus">
import { Search } from "@element-plus/icons-vue";
import antJson from "@iconify/json/json/ant-design.json";
import epJson from "@iconify/json/json/ep.json";
import etJson from "@iconify/json/json/et.json";
import uiwJson from "@iconify/json/json/uiw.json";
import zondJson from "@iconify/json/json/zondicons.json";
import evaJson from "@iconify/json/json/eva.json";
import flatJson from "@iconify/json/json/flat-color-icons.json";
import lineMdJson from "@iconify/json/json/line-md.json";
import { SelectIconProps, SelectIconIconsInfo, SelectIconTabs, IconPrefixType } from "./interface";

// 定义组件props
const props = withDefaults(defineProps<SelectIconProps>(), {
  iconValue: "",
  title: "图标选择",
  clearable: true,
  placeholder: "请选择图标"
});

// 定义响应式变量
const valueIcon = ref(props.iconValue); // 当前选中的图标名称
const dialogVisible = ref(false); // 图标选择弹窗是否可见
const inputValue = ref(""); // 搜索框的值

// 定义方法
const emit = defineEmits(["update:iconValue"]); // 定义更新父组件数据的方法
const openDialog = () => (dialogVisible.value = true); // 打开图标选择弹窗
const selectIcon = (item: string) => {
  // 选择图标并更新父组件数据
  dialogVisible.value = false;
  valueIcon.value = item;
  emit("update:iconValue", item);
  setTimeout(() => inputRef.value.blur(), 0);
};

const clearIcon = () => {
  // 清空图标并更新父组件数据
  valueIcon.value = "";
  emit("update:iconValue", "");
  setTimeout(() => inputRef.value.blur(), 0);
};

// 定义引用变量
const inputRef = ref(); // 搜索框的引用

// 图标库的json文件列表
const jsonList = [antJson, epJson, etJson, zondJson, evaJson, flatJson, uiwJson, lineMdJson];
// 标签页列表
const tabs: SelectIconTabs[] = [
  {
    label: "Element-Plus",
    prefix: "ep"
  },
  {
    label: "Ant-Design",
    prefix: "ant-design"
  },
  {
    label: "Zond-Icons",
    prefix: "zondicons"
  },
  {
    label: "Eva-Icons",
    prefix: "eva"
  },
  {
    label: "Uiw-Icons",
    prefix: "uiw"
  },
  {
    label: "Material-Line-Icons",
    prefix: "line-md"
  },
  {
    label: "Flat-Color-Icons",
    prefix: "flat-color-icons"
  },
  {
    label: "本地图标",
    prefix: "local"
  }
];
const activeName = ref<IconPrefixType>("ep"); // 当前标签页
const iconsList: SelectIconIconsInfo[] = []; // 图标列表
// 生命周期钩子函数 ,组件挂载后执行
onMounted(() => {
  // 遍历标签页列表
  tabs.forEach(el => {
    let iconJson = jsonList.find(item => item.prefix == el.prefix); // 获取对应标签页的json文件
    if (iconJson) {
      // 如果存在对应的json文件
      let result: SelectIconIconsInfo = { prefix: el.prefix, icons: [] }; // 定义图标信息对象
      for (const iconName in iconJson.icons) {
        // 遍历json文件中的图标
        result.icons.push(`${iconName}`); // 将图标名称加入图标信息对象的图标列表中
      }
      iconsList.push(result); // 将图标信息对象加入图标列表中
    } else if (el.prefix == "local") {
      // 如果是本地标签页
      const files = import.meta.glob("@/assets/svg/*"); // 获取src/assets/icons下的所有svg文件
      const fileList = Object.keys(files); // 将文件路径转换为数组
      const fileNames = fileList.map(path => {
        // 遍历文件路径数组，提取文件名
        const parts = path.split("/"); // 将文件路径按照/分割为数组
        const fileNameWithExtension = parts[parts.length - 1]; // 获取数组最后一个元素，即文件名
        const fileNameWithoutExtension = fileNameWithExtension.split(".")[0]; // 将文件名按照.分割为数组，获取第一个元素，即文件名
        return fileNameWithoutExtension;
      });
      iconsList.push({ prefix: "local", icons: fileNames }); // 将svg文件名加入图标信息对象的图标列表中
    }
  });
});

//根据当前标签页和搜索框的值获取图标列表
const getIconsByTab = computed((): string[] => {
  let iconsListPrefix = iconsList.find(item => item.prefix == activeName.value); // 获取当前标签页的图标信息对象
  let icons = iconsListPrefix?.icons || []; // 获取当前标签页的图标列表
  if (!inputValue.value) return icons; // 如果搜索框为空，直接返回当前标签页的图标信息对象
  let result: string[] = icons.filter(item => item.includes(inputValue.value));
  return result;
});

// 获取图标名称
const getIcon = computed(() => (name: string): string => {
  // 返回一个字符串，其中包含当前激活的名称和图标的名称
  return `${activeName.value}:${name}`;
});
</script>

<style scoped lang="scss">
@use "./index";
</style>
