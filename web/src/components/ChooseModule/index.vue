<!-- 
 * @Description: 选择模块组件
 * @Author: huguodong 
 * @Date: 2023-12-15 15:37:35
!-->
<template>
  <el-dialog
    v-model="showModal"
    title="选择应用"
    style="width: 750px; margin: auto"
    align-center
    destroy-on-close
    draggable
    :close-on-click-modal="false"
    :close-on-press-escape="false"
    :before-close="close"
  >
    <el-empty v-if="!noModule" :image-size="200" description="未找到模块!" />
    <n-space vertical v-else>
      <CheckCard :options="auth.moduleList" @change="checkCardChange" />
    </n-space>
    <template #footer>
      <n-space class="float-right">
        <el-checkbox v-model="moduleInfo.ifDefault" :disabled="disabled" class="mt-1">设为默认</el-checkbox>
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" :disabled="disabled" @click="ok">确定</el-button>
      </n-space>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { NSpace } from "naive-ui";
import CheckCard from "../CheckCard/index.vue";
import { useAuthStore } from "@/stores/modules";
import { UserCenter } from "@/api/interface";

const disabled = ref(true); // 确定按钮是否禁用
const auth = useAuthStore();

/** props */
const props = defineProps({
  /** 显示dialog */
  showModal: {
    type: Boolean,
    default: false
  }
});

const showModal = ref(props.showModal); // 模态框是否显示

// 模块信息
const moduleInfo = reactive<UserCenter.ResModuleDefault>({
  id: 0,
  ifDefault: false
});

/** 监听模块列表的变化 **/
const noModule = computed(() => {
  return auth.moduleList.length > 0;
});

// 模块选择卡片改变事件
function checkCardChange(value: string[] | number[]) {
  disabled.value = value[0] === undefined;
  moduleInfo.id = value[0];
}

/** 选择确定 **/
function cancel() {
  showModal.value = false;
  auth.showChooseModule = false;
}

/** 选择确定 **/
function ok() {
  showModal.value = false;
  auth.chooseModule(moduleInfo);
  auth.handleActionAfterLogin();
}

/** 关闭dialog **/
function close() {
  auth.showChooseModule = false;
  showModal.value = false;
}

function openDialog() {
  showModal.value = true;
}

// 暴露方法
defineExpose({ openDialog });
</script>

<style scoped></style>
