<!-- 
 * @Description: 预览
 * @Author: huguodong 
 * @Date: 2025-02-10 10:52:26
!-->
<template>
  <form-container v-model="visible" title="代码生成预览" form-size="1200px" destroy-on-close>
    <el-tabs v-model="previewProps.codeTypeActiveKey" class="-mt-4" @tab-change="onTabChange">
      <el-tab-pane
        v-for="codeType in previewProps.codeTypeArray"
        :key="codeType.codeTypeKey"
        :name="codeType.codeTypeKey"
        :label="codeType.codeTypeTitle"
      >
        <div class="tabs-wrapper">
          <el-tabs v-model="previewProps.codeListActiveKey" tab-position="left" type="card" style="height: 100%">
            <el-tab-pane v-for="pan in codeType.codeTypeList" :key="pan.codeFileName" :name="pan.codeFileName" :label="pan.codeFileName">
              <div style="height: calc(100vh - 160px); overflow: auto">
                <el-input v-model="pan.codeFileContent" type="textarea" autosize />
              </div>
            </el-tab-pane>
          </el-tabs>
        </div>
      </el-tab-pane>
    </el-tabs>
    <template #footer>
      <el-button @click="onClose"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { GenCode, genBasicApi } from "@/api";
import { TabPaneName } from "element-plus";
const visible = ref(false); //是否显示表单

// 选项接口
interface PreviewProps {
  /*代码类型选项 */
  codeTypeActiveKey: string;
  /*代码文件选项 */
  codeListActiveKey: string;
  /*代码预览数组 */
  codeTypeArray: { codeTypeKey: string; codeTypeTitle: string; codeTypeList: GenCode.GenBaseCodeResult[] }[];
}

// 表单参数
const previewProps: PreviewProps = reactive({
  codeTypeArray: [],
  codeTypeActiveKey: "",
  codeListActiveKey: ""
});

/**
 * 打开表单
 * @param genConfig 表单参数
 */
function onOpen(basic: GenCode.GenBasic) {
  visible.value = true; //显示抽屉
  console.log("basic", basic);

  genBasicApi.previewGen({ id: basic.id }).then(res => {
    if (res.data) {
      previewProps.codeTypeArray = [
        {
          codeTypeKey: "frontend",
          codeTypeTitle: "前端代码",
          codeTypeList: res.data.codeFrontendResults
        },
        {
          codeTypeKey: "backend",
          codeTypeTitle: "后端代码",
          codeTypeList: res.data.codeBackendResults
        },
        {
          codeTypeKey: "sqlend",
          codeTypeTitle: "SQL文件",
          codeTypeList: res.data.sqlResults
        }
      ];
      previewProps.codeTypeActiveKey = previewProps.codeTypeArray[0].codeTypeKey; //默认选中第一个
      previewProps.codeListActiveKey = previewProps.codeTypeArray[0].codeTypeList[0].codeFileName; //默认选中第一个
    } else {
      ElMessage.warning("预览失败：请检查问题");
    }
  });
}

/**
 * tab切换
 * @param key 选中的key
 */
function onTabChange(key: TabPaneName) {
  previewProps.codeListActiveKey = previewProps.codeTypeArray.find(f => f.codeTypeKey === key)?.codeTypeList[0]?.codeFileName || "";
}

/**
 * 关闭表单
 */
function onClose() {
  visible.value = false; //关闭抽屉
}

defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped>
.tabs-wrapper {
  height: calc(100vh - 260px);
  overflow: auto;
}
</style>
