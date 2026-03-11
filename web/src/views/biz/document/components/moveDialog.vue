<template>
  <form-container v-model="visible" title="移动文件" form-size="520px">
    <div class="move-dialog">
      <el-form label-width="88px" label-suffix=" :">
        <el-form-item label="父级目录">
          <el-tree-select
            v-model="targetParentId"
            class="w-full"
            check-strictly
            clearable
            default-expand-all
            node-key="id"
            :data="treeOptions"
            :props="treeProps"
            :render-after-expand="false"
            placeholder="请选择目标目录"
            @change="handleTreeChange"
          />
        </el-form-item>
      </el-form>
    </div>
    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" @click="handleSubmit">确定</el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { BizDocument, documentApi } from "@/api";

interface OpenPayload {
  ids: number[];
  names: string[];
  successful?: () => void;
}

type TreeOption = BizDocument.TreeNode & { children?: TreeOption[] };

const visible = ref(false);
const targetParentId = ref<number>(0);
const moveIds = ref<number[]>([]);
const successfulRef = ref<(() => void) | undefined>();
const treeOptions = ref<TreeOption[]>([]);

const treeProps = {
  value: "id",
  label: "name",
  children: "children"
} as const;

async function onOpen(payload: OpenPayload) {
  moveIds.value = payload.ids;
  successfulRef.value = payload.successful;
  targetParentId.value = 0;
  await loadTreeOptions();
  visible.value = true;
}

async function loadTreeOptions() {
  const { data } = await documentApi.tree();
  treeOptions.value = [
    {
      id: 0,
      parentId: -1,
      rootId: 0,
      name: "根目录",
      isRoot: true,
      children: data
    }
  ];
}

function handleTreeChange(value?: number | string) {
  targetParentId.value = value == null || value === "" ? 0 : Number(value);
}

async function handleSubmit() {
  // 是否允许移动到根目录、是否会形成循环引用，都以后端校验为准，前端只传递目标节点。
  await documentApi.move({ ids: moveIds.value, targetParentId: targetParentId.value });
  visible.value = false;
  successfulRef.value?.();
}

defineExpose({ onOpen });
</script>

<style scoped lang="scss">
.move-dialog {
  min-height: 56px;
}
</style>
