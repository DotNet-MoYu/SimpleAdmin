<!-- 
 * @Description: 复制
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:25
!-->
<template>
  <form-container v-model="visible" title="复制组织" form-size="600px">
    <el-form ref="orgFormRef" :rules="rules" :model="copyProps" label-width="auto">
      <s-form-item label="目标组织" prop="targetId">
        <org-selector v-model:org-value="copyProps.targetId" />
      </s-form-item>
      <s-form-item label="包含子集" prop="containsChild" :tooltip="{ content: '将组织下的子机构也复制一份到目标组织' }">
        <s-radio-group v-model="copyProps.containsChild" :options="yesOptions" button />
      </s-form-item>
      <s-form-item label="包含职位" prop="containsPosition" :tooltip="{ content: '将组织下的子机构也复制一份到目标组织' }">
        <s-radio-group v-model="copyProps.containsPosition" :options="yesOptions" button />
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { sysOrgApi } from "@/api";
import { SysDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库
// 是否选项
const yesOptions = dictStore.getDictList(SysDictEnum.YES_NO);

// 表单参数
interface copyProps {
  /** 目标组织id */
  targetId: string | number;
  /** 选择的组织id */
  ids: string[] | number[];
  /** 是否包含子集 */
  containsChild: boolean;
  /** 是否包含职位 */
  containsPosition: boolean;
  /** 成功后的回调 */
  successful?: () => void;
}

// 表单参数
const copyProps = reactive<copyProps>({
  targetId: 0,
  ids: [],
  containsChild: true,
  containsPosition: true
});

// 表单验证规则
const rules = reactive({
  targetId: [required("请选择目标组织")],
  containsChild: [required("请选择是否包含子集")],
  containsPosition: [required("请选择是否包含职位")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(ids: string[] | number[], successful: () => void) {
  visible.value = true; //显示表单
  copyProps.ids = ids;
  copyProps.successful = successful;
}

// 提交数据（新增/编辑）
const orgFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  orgFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysOrgApi
      .copy(copyProps)
      .then(() => {
        copyProps.successful!(); //调用父组件的successful方法
      })
      .finally(() => {
        onClose();
      });
  });
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
