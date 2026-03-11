<template>
  <form-container v-model="visible" :title="dialogTitle" form-size="520px" :destroy-on-close="false">
    <el-form ref="formRef" :model="formData" :rules="rules" label-width="100px" label-suffix=" :">
      <el-form-item label="当前名称">
        <el-input :model-value="sourceName" disabled />
      </el-form-item>
      <el-form-item :label="nameLabel" prop="name">
        <el-input v-model="formData.name" clearable maxlength="100" :placeholder="namePlaceholder">
          <template v-if="suffixText" #append>.{{ suffixText }}</template>
        </el-input>
        <div v-if="suffixText" class="form-tip">文件扩展名会保留为 .{{ suffixText }}</div>
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
      <el-button type="primary" @click="handleSubmit">保存</el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { BizDocument, documentApi } from "@/api";
import { useDictStore } from "@/stores/modules";
import { required } from "@/utils/formRules";
import type { FormInstance } from "element-plus";

interface OpenPayload {
  record: BizDocument.DocumentInfo;
  successful?: () => void;
}

const visible = ref(false);
const dialogTitle = ref("编辑文档");
const sourceName = ref("");
const suffixText = ref("");
const isFile = ref(false);
const successfulRef = ref<(() => void) | undefined>();
const formRef = ref<FormInstance>();
const dictStore = useDictStore();
const docLabelOptions = computed(() => dictStore.getDictList("doc_label"));
const formData = reactive({
  id: 0,
  name: "",
  label: "",
  remark: ""
});

const rules = reactive({
  name: [
    required("请输入名称"),
    {
      validator: (_rule: unknown, value: string, callback: (error?: Error) => void) => {
        if (isFile.value && value?.includes(".")) {
          callback(new Error("文件名称无需填写扩展名"));
          return;
        }
        callback();
      },
      trigger: ["blur", "change"]
    }
  ]
});

const nameLabel = computed(() => (isFile.value ? "文件名称" : "文件夹名称"));
const namePlaceholder = computed(() => (isFile.value ? "请输入文件名称，不含扩展名" : "请输入文件夹名称"));

function onOpen(payload: OpenPayload) {
  isFile.value = payload.record.nodeType === 2;
  suffixText.value = payload.record.nodeType === 2 ? payload.record.suffix || "" : "";
  dialogTitle.value = payload.record.nodeType === 2 ? "编辑文件" : "编辑文件夹";
  sourceName.value = payload.record.name;
  successfulRef.value = payload.successful;
  formData.id = payload.record.id;

  // 文件改名只允许用户改“主文件名”，扩展名仍由后端基于原 suffix 拼回去。
  formData.name =
    payload.record.nodeType === 2 && payload.record.suffix
      ? payload.record.name.replace(new RegExp(`\\.${payload.record.suffix}$`, "i"), "")
      : payload.record.name;
  formData.label = String(payload.record.label ?? "");
  formData.remark = payload.record.remark || "";
  visible.value = true;
}

function handleSubmit() {
  formRef.value?.validate(async valid => {
    if (!valid) return;
    await documentApi.rename({
      id: formData.id,
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

<style scoped lang="scss">
.form-tip {
  margin-top: 6px;
  font-size: 12px;
  color: var(--el-text-color-secondary);
}
</style>
