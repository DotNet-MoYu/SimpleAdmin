<template>
  <div class="main-box document-page">
    <TreeFilter
      class="document-tree"
      title="文件夹"
      label="name"
      :show-all="false"
      :default-expand-all="false"
      :data="treeData"
      @change="handleTreeChange"
    />

    <div class="table-box">
      <div class="card mb-12px">
        <div class="toolbar-row">
          <div class="left-actions">
            <s-button suffix="文件夹" @click="openCreateFolder" />
            <el-dropdown split-button @click="triggerFileUpload" @command="handleUploadCommand">
              上传
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="file">上传文件</el-dropdown-item>
                  <el-dropdown-item command="folder">上传文件夹</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </div>
        <div class="breadcrumb-row">
          <el-button v-if="currentParentId !== 0" link type="primary" @click="goBack">返回上级</el-button>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item>
              <el-link type="primary" underline="never" @click="navigateToFolder(0)">全部文件</el-link>
            </el-breadcrumb-item>
            <el-breadcrumb-item v-for="item in breadcrumbList" :key="item.id">
              <el-link type="primary" underline="never" @click="navigateToFolder(item.id)">{{ item.name }}</el-link>
            </el-breadcrumb-item>
          </el-breadcrumb>
          <span class="current-tip">当前位置：{{ currentFolderLabel }}</span>
        </div>
        <el-alert
          title="超级管理员可创建根目录并分配给用户或角色，普通用户可管理被授权根目录下的内容，但不能变更根目录授权本身。"
          type="info"
          :closable="false"
          show-icon
        />
      </div>

      <ProTable ref="proTable" :columns="columns" :init-param="initParam" :request-api="documentApi.page" @row-dblclick="handleRowDblClick">
        <template #tableHeader="scope">
          <div class="table-header-wrap">
            <div class="table-header-actions">
              <s-button
                plain
                :opt="FormOptEnum.DELETE"
                prefix="批量删除"
                :disabled="!scope.isSelected || scope.selectedList.some(item => !item.canDelete)"
                @click="onBatchDelete(scope.selectedListIds)"
              />
              <s-button
                plain
                :icon="Sort"
                :disabled="!scope.isSelected || scope.selectedList.some(item => !item.canMove)"
                @click="onBatchMove(scope.selectedList)"
              >
                批量移动
              </s-button>
              <s-button plain :icon="CloseBold" :disabled="!scope.isSelected" @click="proTable?.clearSelection()">清空选择</s-button>
            </div>
            <div class="selection-meta">
              <el-icon><InfoFilled /></el-icon>
              <span>已选择：{{ scope.selectedListIds.length }}</span>
            </div>
          </div>
        </template>

        <template #name="scope">
          <div class="name-cell">
            <el-icon class="mr-6px" :size="18">
              <FolderOpened v-if="scope.row.nodeType === 1" />
              <DocumentIcon v-else />
            </el-icon>
            <el-link underline="never" type="primary" @click="handleNameClick(scope.row)">{{ scope.row.name }}</el-link>
          </div>
        </template>

        <template #fileTypeLabel="scope">
          <el-tag :type="fileTypeTagType(scope.row.fileTypeLabel)">
            {{ scope.row.fileTypeLabel || (scope.row.nodeType === 1 ? "文件夹" : "文件") }}
          </el-tag>
        </template>

        <template #label="scope">
          <el-tag v-if="scope.row.label" type="info" effect="plain">{{ resolveLabel(scope.row.label) }}</el-tag>
          <span v-else class="text-#909399">-</span>
        </template>

        <template #remark="scope">
          <span class="remark-text">{{ scope.row.remark || "-" }}</span>
        </template>

        <template #operation="scope">
          <el-space wrap>
            <s-button v-if="scope.row.canRename" link :opt="FormOptEnum.EDIT" @click="openRename(scope.row)">编辑</s-button>
            <el-dropdown v-if="hasMoreActions(scope.row)" @command="handleCommand">
              <el-link type="primary" underline="never" :icon="ArrowDown"> 更多 </el-link>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item v-if="scope.row.nodeType === 2" :command="command(scope.row, cmdEnum.Preview)">预览</el-dropdown-item>
                  <el-dropdown-item v-if="scope.row.nodeType === 2" :command="command(scope.row, cmdEnum.Download)">下载</el-dropdown-item>
                  <el-dropdown-item v-if="scope.row.nodeType === 1" :command="command(scope.row, cmdEnum.FolderDownload)">下载</el-dropdown-item>
                  <el-dropdown-item v-if="scope.row.canMove" :command="command(scope.row, cmdEnum.Move)">移动</el-dropdown-item>
                  <el-dropdown-item v-if="scope.row.canGrant" :command="command(scope.row, cmdEnum.Grant)">授权</el-dropdown-item>
                  <el-dropdown-item v-if="scope.row.canDelete" :command="command(scope.row, cmdEnum.Delete)">删除</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </el-space>
        </template>
      </ProTable>
    </div>

    <input ref="fileInputRef" type="file" hidden multiple @change="handleFileInputChange" />
    <input ref="folderInputRef" type="file" hidden multiple webkitdirectory directory @change="handleFolderInputChange" />

    <CreateFolder ref="createFolderRef" />
    <RenameDialog ref="renameDialogRef" />
    <MoveDialog ref="moveDialogRef" />
    <PreviewDialog ref="previewDialogRef" />
    <GrantDialog ref="grantDialogRef" />
  </div>
</template>

<script setup lang="ts" name="bizDocument">
import { BizDocument, documentApi } from "@/api";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import TreeFilter from "@/components/TreeFilter/index.vue";
import { FormOptEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { useDictStore } from "@/stores/modules";
import { ElMessage } from "element-plus";
import { Document as DocumentIcon, ArrowDown, Sort, CloseBold, InfoFilled } from "@element-plus/icons-vue";
import CreateFolder from "./components/createFolder.vue";
import RenameDialog from "./components/renameDialog.vue";
import MoveDialog from "./components/moveDialog.vue";
import PreviewDialog from "./components/previewDialog.vue";
import GrantDialog from "./components/grantDialog.vue";

interface TreeNodeMapValue extends BizDocument.TreeNode {
  parent?: TreeNodeMapValue;
}

interface MoreCommand {
  row: BizDocument.DocumentInfo;
  cmd: string;
}

const dictStore = useDictStore();

const fileTypeOptions: Array<{ label: BizDocument.FileType; value: BizDocument.FileType }> = [
  { label: "文件夹", value: "文件夹" },
  { label: "文档", value: "文档" },
  { label: "图片", value: "图片" },
  { label: "压缩包", value: "压缩包" },
  { label: "应用程序", value: "应用程序" },
  { label: "文件", value: "文件" }
];

const docLabelOptions = computed(() => dictStore.getDictList("doc_label"));

const cmdEnum = {
  Preview: "预览",
  Download: "下载",
  FolderDownload: "文件夹下载",
  Move: "移动",
  Grant: "授权",
  Delete: "删除"
} as const;

const proTable = ref<ProTableInstance>();
const fileInputRef = ref<HTMLInputElement>();
const folderInputRef = ref<HTMLInputElement>();
const createFolderRef = ref<InstanceType<typeof CreateFolder> | null>(null);
const renameDialogRef = ref<InstanceType<typeof RenameDialog> | null>(null);
const moveDialogRef = ref<InstanceType<typeof MoveDialog> | null>(null);
const previewDialogRef = ref<InstanceType<typeof PreviewDialog> | null>(null);
const grantDialogRef = ref<InstanceType<typeof GrantDialog> | null>(null);
const treeData = ref<BizDocument.TreeNode[]>([]);
const nodeMap = shallowRef(new Map<number, TreeNodeMapValue>());
const currentParentId = ref(0);
const initParam = reactive<BizDocument.Page>({ parentId: 0, pageNum: 1, pageSize: 10 });

const columns: ColumnProps<BizDocument.DocumentInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "name", label: "名称", minWidth: 120, search: { el: "input" } },
  { prop: "fileTypeLabel", label: "类型", minWidth: 100 },
  { prop: "fileType", label: "文件类型", enum: fileTypeOptions, search: { el: "select" }, isShow: false, isSetting: false },
  { prop: "createTime", label: "创建时间", minWidth: 180 },
  { prop: "createUserName", label: "创建人", minWidth: 120 },
  { prop: "updateTime", label: "修改时间", minWidth: 180 },
  { prop: "updateUserName", label: "修改人", minWidth: 120 },
  { prop: "sizeInfo", label: "大小", minWidth: 120 },
  { prop: "label", label: "标签", enum: docLabelOptions, search: { el: "select" }, minWidth: 140 },
  { prop: "remark", label: "备注", minWidth: 180 },
  { prop: "operation", label: "操作", minWidth: 150, fixed: "right" }
];

const currentFolderLabel = computed(() => {
  if (currentParentId.value === 0) return "全部文件";
  return nodeMap.value.get(currentParentId.value)?.name || "当前目录";
});

const breadcrumbList = computed(() => {
  const result: TreeNodeMapValue[] = [];
  let current = nodeMap.value.get(currentParentId.value);
  while (current) {
    result.unshift(current);
    current = current.parent;
  }
  return result;
});

onMounted(() => {
  loadTree();
});

async function loadTree() {
  const { data } = await documentApi.tree();
  treeData.value = data;
  const map = new Map<number, TreeNodeMapValue>();

  // 树组件只关心 children，但页面还需要“向上回溯父链”来渲染面包屑，所以额外构建一份 parent 索引。
  const buildMap = (nodes: BizDocument.TreeNode[], parent?: TreeNodeMapValue) => {
    nodes.forEach(item => {
      const current: TreeNodeMapValue = { ...item, parent };
      map.set(item.id, current);
      if (item.children?.length) buildMap(item.children, current);
    });
  };
  buildMap(data);
  nodeMap.value = map;
  if (currentParentId.value !== 0 && !map.has(currentParentId.value)) {
    navigateToFolder(0, false);
  }
}

function navigateToFolder(folderId: number, refreshTable = true) {
  currentParentId.value = folderId;
  initParam.parentId = folderId;

  // 切目录时回到第一页，否则用户可能停留在旧目录的高页码上，看起来像“新目录没有数据”。
  if (proTable.value) proTable.value.pageable.pageNum = 1;
  if (refreshTable) proTable.value?.refresh();
}

function handleTreeChange(value: string | number) {
  navigateToFolder(value === "" ? 0 : Number(value));
}

function handleNameClick(row: BizDocument.DocumentInfo) {
  if (row.nodeType === 1) {
    navigateToFolder(row.id);
    return;
  }
  openPreview(row);
}

function handleRowDblClick(row: BizDocument.DocumentInfo) {
  if (row.nodeType === 1) navigateToFolder(row.id);
}

function goBack() {
  const parentId = nodeMap.value.get(currentParentId.value)?.parentId ?? 0;
  navigateToFolder(parentId);
}

function openCreateFolder() {
  createFolderRef.value?.onOpen({
    parentId: currentParentId.value,
    parentName: currentFolderLabel.value,
    successful: refreshAll
  });
}

function openRename(row: BizDocument.DocumentInfo) {
  renameDialogRef.value?.onOpen({ record: row, successful: refreshAll });
}

function command(row: BizDocument.DocumentInfo, cmd: string): MoreCommand {
  return { row, cmd };
}

function hasMoreActions(row: BizDocument.DocumentInfo) {
  return row.nodeType === 2 || row.canMove || row.canGrant || row.canDelete || row.nodeType === 1;
}

function handleCommand(payload: MoreCommand) {
  const { row, cmd } = payload;
  switch (cmd) {
    case cmdEnum.Preview:
      openPreview(row);
      break;
    case cmdEnum.Download:
      downloadFile(row);
      break;
    case cmdEnum.FolderDownload:
      ElMessage.info("文件夹打包下载待接入，当前请进入目录后分别下载文件。");
      break;
    case cmdEnum.Move:
      onBatchMove([row]);
      break;
    case cmdEnum.Grant:
      openGrant(row);
      break;
    case cmdEnum.Delete:
      onDelete(row);
      break;
  }
}

function handleUploadCommand(command: "file" | "folder") {
  if (command === "folder") {
    triggerFolderUpload();
    return;
  }
  triggerFileUpload();
}

function onBatchMove(rows: Array<BizDocument.DocumentInfo | Record<string, any>>) {
  if (!rows.length) {
    ElMessage.warning("请选择要移动的文件");
    return;
  }
  const typedRows = rows as BizDocument.DocumentInfo[];
  moveDialogRef.value?.onOpen({
    ids: typedRows.map(item => Number(item.id)),
    names: typedRows.map(item => item.name),
    successful: refreshAll
  });
}

async function onDelete(row: BizDocument.DocumentInfo) {
  await useHandleData(documentApi.delete, { id: row.id }, `删除文件 ${row.name}`);
  refreshAll();
}

async function onBatchDelete(ids: Array<string | number>) {
  const validIds = ids.map(id => Number(id));
  if (!validIds.length) {
    ElMessage.warning("请选择要删除的文件");
    return;
  }
  await useHandleData(documentApi.batchDelete, { ids: validIds }, "批量删除选中文件");
  refreshAll();
}

function openGrant(row: BizDocument.DocumentInfo) {
  grantDialogRef.value?.onOpen({ id: row.id });
}

function openPreview(row: BizDocument.DocumentInfo) {
  previewDialogRef.value?.onOpen(row.id, row.name);
}

async function downloadFile(row: BizDocument.DocumentInfo) {
  const response = (await documentApi.download({ id: row.id })) as any;
  const blob = new Blob([response.data], { type: "application/octet-stream;charset=UTF-8" });
  const contentDisposition = response.headers["content-disposition"];
  const link = document.createElement("a");
  link.href = URL.createObjectURL(blob);
  const match = /filename=([^;]+\.[^\.;]+);*/.exec(contentDisposition || "");
  link.download = match ? decodeURIComponent(match[1]) : row.name;
  link.click();
  document.body.appendChild(link);
  document.body.removeChild(link);
  window.URL.revokeObjectURL(link.href);
}

function triggerFileUpload() {
  fileInputRef.value?.click();
}

function triggerFolderUpload() {
  folderInputRef.value?.click();
}

async function handleFileInputChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const files = Array.from(input.files || []);
  if (!files.length) return;
  const formData = new FormData();
  formData.append("ParentId", String(currentParentId.value));
  formData.append("Engine", "LOCAL");
  files.forEach(file => formData.append("Files", file));
  await documentApi.uploadFiles(formData);
  input.value = "";
  refreshAll();
}

async function handleFolderInputChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const files = Array.from(input.files || []);
  if (!files.length) return;
  const formData = new FormData();
  formData.append("ParentId", String(currentParentId.value));
  formData.append("Engine", "LOCAL");
  files.forEach(file => {
    formData.append("Files", file);
    // 浏览器会把目录结构放在 webkitRelativePath 里，后端据此重建整棵文件夹树。
    formData.append("RelativePaths", file.webkitRelativePath || file.name);
  });
  await documentApi.uploadFolder(formData);
  input.value = "";
  refreshAll();
}

async function refreshAll() {
  // 树和表格都依赖同一批文档数据，删除 / 移动 / 上传后要一起刷新，避免目录状态不同步。
  await loadTree();
  proTable.value?.refresh();
}

function resolveLabel(value?: string | number) {
  if (value == null || value === "") return "-";
  const text = dictStore.dictTranslation("doc_label", String(value));
  return text === "无此字典" ? String(value) : text;
}

function fileTypeTagType(fileType?: string) {
  switch (fileType) {
    case "文件夹":
      return "warning";
    case "文档":
      return "info";
    case "图片":
      return "danger";
    case "压缩包":
      return "success";
    case "应用程序":
      return undefined;
    default:
      return undefined;
  }
}
</script>

<style scoped lang="scss">
.document-page {
  gap: 12px;
}

.document-tree {
  width: 240px;
  flex-shrink: 0;
}

.toolbar-row {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 12px;
}

.left-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.breadcrumb-row {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.current-tip {
  color: var(--el-text-color-secondary);
  font-size: 13px;
}

.table-header-wrap {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}

.table-header-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.selection-meta {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  min-height: 32px;
  padding: 0 12px;
  color: var(--el-color-primary);
  background: var(--el-color-primary-light-9);
  border: 1px solid var(--el-color-primary-light-7);
  border-radius: 6px;
}

.name-cell {
  display: flex;
  align-items: center;
  min-width: 0;
}

.remark-text {
  display: inline-block;
  max-width: 180px;
  color: var(--el-text-color-regular);
  word-break: break-word;
}
</style>
