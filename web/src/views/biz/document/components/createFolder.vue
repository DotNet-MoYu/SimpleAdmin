<template>
  <form-container v-model="visible" :title="title" form-size="520px" :destroy-on-close="false">
    <el-form ref="formRef" :model="formData" :rules="rules" label-width="100px" label-suffix=" :">
      <el-form-item label="上级目录">
        <el-input :model-value="parentName || '根目录'" disabled />
      </el-form-item>
      <el-form-item label="文件夹名称" prop="name">
        <el-input v-model="formData.name" clearable maxlength="100" placeholder="请输入文件夹名称" />
      </el-form-item>
      <el-form-item label="标签">
        <el-select v-model="formData.label" clearable filterable placeholder="请选择标签">
          <el-option v-for="item in docLabelOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
      </el-form-item>
      <el-form-item label="备注">
        <el-input v-model="formData.remark" type="textarea" :rows="3" maxlength="200" placeholder="请输入备注，选填" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" @click="handleSubmit">确定</el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { documentApi } from "@/api";
import { useDictStore } from "@/stores/modules";
import { required } from "@/utils/formRules";
import type { FormInstance } from "element-plus";

interface OpenPayload {
  parentId: number;
  parentName?: string;
  successful?: () => void;
}

const visible = ref(false);
const title = ref("新建文件夹");
const parentName = ref("");
const successfulRef = ref<(() => void) | undefined>();
const formRef = ref<FormInstance>();
const dictStore = useDictStore();
const docLabelOptions = computed(() => dictStore.getDictList("doc_label"));
const formData = reactive({
  parentId: 0,
  name: "",
  label: "",
  remark: ""
});

const rules = reactive({
  name: [required("请输入文件夹名称")]
});

function resetForm() {
  formData.parentId = 0;
  formData.name = "";
  formData.label = "";
  formData.remark = "";
  parentName.value = "";
}

function onOpen(payload: OpenPayload) {
  resetForm();
  title.value = payload.parentId === 0 ? "新建根目录" : "新建文件夹";
  formData.parentId = payload.parentId;
  parentName.value = payload.parentName ?? "";
  successfulRef.value = payload.successful;
  visible.value = true;
}

function handleSubmit() {
  formRef.value?.validate(async valid => {
    if (!valid) return;

    // 根目录 / 子目录的真正权限判断仍以后端为准，弹窗只负责把当前上下文原样提交。
    await documentApi.addFolder({
      parentId: formData.parentId,
      name: formData.name,
      label: formData.label || undefined,
      remark: formData.remark
    });
    visible.value = false;
    successfulRef.value?.();
  });
}

defineExpose({ onOpen });
</script>
