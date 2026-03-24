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
          <el-badge :value="activeUploadCount || uploadTasks.length" :hidden="!uploadTasks.length" class="upload-entry-badge">
            <el-button :type="uploadTasks.length ? 'primary' : 'default'" plain @click="openUploadPanel">上传任务</el-button>
          </el-badge>
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

    <el-dialog v-model="uploadPanelVisible" title="上传任务" width="720px" top="8vh">
      <div v-if="uploadTasks.length" class="upload-panel">
        <div class="upload-panel-header">
          <div class="upload-panel-title-wrap">
            <div class="upload-panel-title">上传任务</div>
            <div class="upload-panel-meta">进行中：{{ activeUploadCount }}，总进度：{{ uploadSummary.progress }}%</div>
          </div>
          <el-space wrap size="small">
            <el-button v-if="uploadSummary.error > 0" link type="danger" @click="retryFailedUploadTasks">重试失败任务</el-button>
            <el-button v-if="uploadSummary.finished > 0" link type="primary" @click="clearFinishedUploadTasks">清理已完成</el-button>
            <el-button link type="info" @click="clearCancelledUploadTasks">清理已取消</el-button>
          </el-space>
        </div>
        <el-progress :percentage="uploadSummary.progress" />
        <div class="upload-summary-grid">
          <div class="upload-summary-item">
            <span class="label">总数</span>
            <span class="value">{{ uploadSummary.total }}</span>
          </div>
          <div class="upload-summary-item">
            <span class="label">成功</span>
            <span class="value success">{{ uploadSummary.success }}</span>
          </div>
          <div class="upload-summary-item">
            <span class="label">失败</span>
            <span class="value danger">{{ uploadSummary.error }}</span>
          </div>
          <div class="upload-summary-item">
            <span class="label">暂停</span>
            <span class="value warning">{{ uploadSummary.paused }}</span>
          </div>
          <div class="upload-summary-item">
            <span class="label">已取消</span>
            <span class="value">{{ uploadSummary.cancelled }}</span>
          </div>
        </div>
        <div class="upload-task-list">
          <div v-for="task in uploadTasks" :key="task.taskId" class="upload-task">
            <div class="upload-task-top">
              <div class="upload-task-name" :title="task.displayName">{{ task.displayName }}</div>
              <el-tag :type="uploadStatusTagType(task.status)" effect="plain">{{ resolveUploadStatusText(task.status) }}</el-tag>
            </div>
            <el-progress
              :percentage="task.progress"
              :status="task.status === 'success' ? 'success' : task.status === 'error' ? 'exception' : undefined"
            />
            <div class="upload-task-bottom">
              <span class="upload-task-size">{{ formatBytes(task.uploadedBytes) }} / {{ formatBytes(task.file.size) }}</span>
              <el-space wrap size="small">
                <el-button v-if="task.status === 'uploading' || task.status === 'hashing'" link type="warning" @click="pauseUploadTask(task)">
                  暂停
                </el-button>
                <el-button v-if="task.status === 'paused'" link type="primary" @click="resumeUploadTask(task)">继续</el-button>
                <el-button v-if="task.status === 'error'" link type="danger" @click="retryUploadTask(task)">重试</el-button>
                <el-button v-if="task.status !== 'success' && task.status !== 'cancelled'" link type="danger" @click="cancelUploadTask(task)">
                  取消
                </el-button>
              </el-space>
            </div>
            <div v-if="task.errorMessage" class="upload-task-error">{{ task.errorMessage }}</div>
          </div>
        </div>
      </div>
      <el-empty v-else description="暂无上传任务" />
    </el-dialog>
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

interface UploadTask {
  taskId: string;
  displayName: string;
  relativePath: string;
  parentId: number;
  file: File;
  status: BizDocument.ChunkTaskStatus;
  progress: number;
  uploadedBytes: number;
  chunkSize: number;
  chunkCount: number;
  uploadId?: number;
  uploadedChunks: number[];
  errorMessage: string;
  activeControllers: AbortController[];
}

const dictStore = useDictStore();
const LARGE_FILE_THRESHOLD = 20 * 1024 * 1024;
const DEFAULT_CHUNK_SIZE = 5 * 1024 * 1024;
const DEFAULT_CHUNK_CONCURRENCY = 3;

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
const uploadTasks = ref<UploadTask[]>([]);
const uploadPanelVisible = ref(false);

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

const activeUploadCount = computed(() => uploadTasks.value.filter(task => ["hashing", "uploading", "merging"].includes(task.status)).length);
const uploadSummary = computed(() => {
  const total = uploadTasks.value.length;
  const uploadedBytes = uploadTasks.value.reduce((sum, task) => sum + task.uploadedBytes, 0);
  const totalBytes = uploadTasks.value.reduce((sum, task) => sum + task.file.size, 0);
  const progress = totalBytes > 0 ? Math.min(100, Math.round((uploadedBytes / totalBytes) * 100)) : 0;
  const success = uploadTasks.value.filter(task => task.status === "success").length;
  const error = uploadTasks.value.filter(task => task.status === "error").length;
  const paused = uploadTasks.value.filter(task => task.status === "paused").length;
  const cancelled = uploadTasks.value.filter(task => task.status === "cancelled").length;
  return {
    total,
    progress,
    success,
    error,
    paused,
    cancelled,
    finished: success + cancelled
  };
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

function openUploadPanel() {
  uploadPanelVisible.value = true;
}

async function handleFileInputChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const files = Array.from(input.files || []);
  if (!files.length) return;
  const directFiles = files.filter(file => file.size < LARGE_FILE_THRESHOLD);
  const chunkTasks = files.filter(file => file.size >= LARGE_FILE_THRESHOLD).map(file => createUploadTask(file, ""));
  if (directFiles.length) await uploadFilesDirect(directFiles);
  if (chunkTasks.length) await enqueueChunkTasks(chunkTasks);
  input.value = "";
}

async function handleFolderInputChange(event: Event) {
  const input = event.target as HTMLInputElement;
  const files = Array.from(input.files || []);
  if (!files.length) return;
  const tasks = files.map(file => createUploadTask(file, file.webkitRelativePath || file.name));
  await enqueueChunkTasks(tasks);
  input.value = "";
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

async function uploadFilesDirect(files: File[]) {
  const formData = new FormData();
  formData.append("ParentId", String(currentParentId.value));
  formData.append("Engine", "LOCAL");
  files.forEach(file => formData.append("Files", file));
  await documentApi.uploadFiles(formData);
  await refreshAll();
}

function createUploadTask(file: File, relativePath: string): UploadTask {
  return {
    taskId: `${file.name}-${file.size}-${file.lastModified}-${Date.now()}-${Math.random().toString(16).slice(2)}`,
    displayName: relativePath || file.name,
    relativePath,
    parentId: currentParentId.value,
    file,
    status: "waiting",
    progress: 0,
    uploadedBytes: 0,
    chunkSize: DEFAULT_CHUNK_SIZE,
    chunkCount: 0,
    uploadId: undefined,
    uploadedChunks: [],
    errorMessage: "",
    activeControllers: []
  };
}

async function enqueueChunkTasks(tasks: UploadTask[]) {
  uploadTasks.value.unshift(...tasks);
  ElMessage.info(`已加入 ${tasks.length} 个上传任务，可点击“上传任务”查看进度`);
  let successCount = 0;
  let failCount = 0;
  for (const task of tasks) {
    const success = await startUploadTask(task);
    if (success) successCount++;
    else if (task.status === "error") failCount++;
  }
  if (successCount > 0) await refreshAll();
  ElMessage[failCount > 0 ? "warning" : "success"](`本批次上传结束：成功 ${successCount} 个，失败 ${failCount} 个`);
}

async function startUploadTask(task: UploadTask) {
  if (task.status === "uploading" || task.status === "merging" || task.status === "success" || task.status === "cancelled") return false;

  try {
    task.errorMessage = "";
    task.status = "hashing";
    if (!task.uploadId) {
      const fileHash = await buildFileFingerprint(task.file, task.relativePath);
      const { data } = await documentApi.uploadInit({
        parentId: task.parentId,
        engine: "LOCAL",
        fileName: task.file.name,
        fileSize: task.file.size,
        chunkSize: DEFAULT_CHUNK_SIZE,
        fileHash,
        relativePath: task.relativePath || undefined
      });
      task.uploadId = data.uploadId;
      task.chunkSize = data.chunkSize;
      task.chunkCount = data.chunkCount;
      task.uploadedChunks = [...data.uploadedChunks].sort((a, b) => a - b);
      syncUploadTaskProgress(task);
      if (data.skipUpload) {
        task.status = "success";
        task.progress = 100;
        task.uploadedBytes = task.file.size;
        return true;
      }
    } else {
      const { data } = await documentApi.uploadStatus({ uploadId: task.uploadId });
      syncUploadTaskStatus(task, data);
      if (data.isCompleted) {
        task.status = "success";
        task.progress = 100;
        task.uploadedBytes = task.file.size;
        return true;
      }
    }

    const remainingChunks = Array.from({ length: task.chunkCount }, (_, index) => index).filter(index => !task.uploadedChunks.includes(index));
    task.status = "uploading";
    if (remainingChunks.length) await uploadTaskChunks(task, remainingChunks);
    if (task.status !== "uploading") return false;

    task.status = "merging";
    await documentApi.uploadComplete({ uploadId: task.uploadId! });
    task.status = "success";
    task.progress = 100;
    task.uploadedBytes = task.file.size;
    return true;
  } catch (error: any) {
    if (isAbortError(error)) {
      return false;
    }
    task.status = "error";
    task.errorMessage = error?.msg || error?.message || "上传失败";
    return false;
  }
}

async function uploadTaskChunks(task: UploadTask, chunkIndexes: number[]) {
  const uploadedSet = new Set(task.uploadedChunks);
  const queue = [...chunkIndexes];
  const workers = Array.from({ length: Math.min(DEFAULT_CHUNK_CONCURRENCY, queue.length) }, async () => {
    while (queue.length && task.status === "uploading") {
      const chunkIndex = queue.shift();
      if (chunkIndex == null) return;
      const controller = new AbortController();
      task.activeControllers.push(controller);
      try {
        const chunkBlob = getFileChunk(task.file, task.chunkSize, chunkIndex);
        const chunkHash = await computeBlobHash(chunkBlob);
        const formData = new FormData();
        formData.append("UploadId", String(task.uploadId));
        formData.append("ChunkIndex", String(chunkIndex));
        formData.append("ChunkHash", chunkHash);
        formData.append("Chunk", new File([chunkBlob], task.file.name, { type: task.file.type || "application/octet-stream" }));
        await documentApi.uploadChunk(formData, { signal: controller.signal });
        if (!uploadedSet.has(chunkIndex)) {
          uploadedSet.add(chunkIndex);
          task.uploadedChunks = Array.from(uploadedSet).sort((a, b) => a - b);
          syncUploadTaskProgress(task);
        }
      } finally {
        removeTaskController(task, controller);
      }
    }
  });

  const results = await Promise.allSettled(workers);
  const rejected = results.find(result => result.status === "rejected") as PromiseRejectedResult | undefined;
  if (rejected) throw rejected.reason;
}

async function pauseUploadTask(task: UploadTask) {
  abortTaskControllers(task);
  task.status = "paused";
  if (task.uploadId) {
    const { data } = await documentApi.uploadStatus({ uploadId: task.uploadId });
    syncUploadTaskStatus(task, data);
    task.status = "paused";
  }
}

async function resumeUploadTask(task: UploadTask) {
  const success = await startUploadTask(task);
  if (success) await refreshAll();
}

async function retryUploadTask(task: UploadTask) {
  task.errorMessage = "";
  const success = await startUploadTask(task);
  if (success) await refreshAll();
}

async function retryFailedUploadTasks() {
  const failedTasks = uploadTasks.value.filter(task => task.status === "error");
  if (!failedTasks.length) return;
  let successCount = 0;
  for (const task of failedTasks) {
    task.errorMessage = "";
    const success = await startUploadTask(task);
    if (success) successCount++;
  }
  if (successCount > 0) await refreshAll();
}

async function cancelUploadTask(task: UploadTask) {
  abortTaskControllers(task);
  if (task.uploadId) await documentApi.uploadCancel({ uploadId: task.uploadId });
  task.status = "cancelled";
}

function clearFinishedUploadTasks() {
  uploadTasks.value = uploadTasks.value.filter(task => task.status !== "success");
}

function clearCancelledUploadTasks() {
  uploadTasks.value = uploadTasks.value.filter(task => task.status !== "cancelled");
}

function syncUploadTaskStatus(task: UploadTask, status: BizDocument.ChunkUploadStatusRes) {
  task.chunkCount = status.chunkCount;
  task.uploadedChunks = [...status.uploadedChunks].sort((a, b) => a - b);
  syncUploadTaskProgress(task);
}

function syncUploadTaskProgress(task: UploadTask) {
  task.uploadedBytes = calculateUploadedBytes(task);
  task.progress = calculateTaskProgress(task);
}

function calculateUploadedBytes(task: UploadTask) {
  return task.uploadedChunks.reduce((total, chunkIndex) => total + getFileChunk(task.file, task.chunkSize, chunkIndex).size, 0);
}

function calculateTaskProgress(task: UploadTask) {
  if (task.file.size <= 0) return 0;
  if (task.status === "success") return 100;
  if (task.uploadedBytes >= task.file.size) return 99;
  return Math.min(99, Math.round((task.uploadedBytes / task.file.size) * 100));
}

function getFileChunk(file: File, chunkSize: number, chunkIndex: number) {
  const start = chunkIndex * chunkSize;
  const end = Math.min(file.size, start + chunkSize);
  return file.slice(start, end);
}

function abortTaskControllers(task: UploadTask) {
  task.activeControllers.splice(0).forEach(controller => controller.abort());
}

function removeTaskController(task: UploadTask, controller: AbortController) {
  const index = task.activeControllers.indexOf(controller);
  if (index >= 0) task.activeControllers.splice(index, 1);
}

function isAbortError(error: any) {
  return error?.name === "CanceledError" || error?.code === "ERR_CANCELED";
}

async function buildFileFingerprint(file: File, relativePath: string) {
  return await digestText(`${file.name}|${file.size}|${file.lastModified}|${relativePath}`);
}

async function computeBlobHash(blob: Blob) {
  const buffer = await blob.arrayBuffer();
  const digest = await crypto.subtle.digest("SHA-256", buffer);
  return bufferToHex(digest);
}

async function digestText(value: string) {
  const data = new TextEncoder().encode(value);
  const digest = await crypto.subtle.digest("SHA-256", data);
  return bufferToHex(digest);
}

function bufferToHex(buffer: ArrayBuffer) {
  return Array.from(new Uint8Array(buffer))
    .map(item => item.toString(16).padStart(2, "0"))
    .join("");
}

function resolveUploadStatusText(status: BizDocument.ChunkTaskStatus) {
  switch (status) {
    case "waiting":
      return "等待中";
    case "hashing":
      return "准备中";
    case "uploading":
      return "上传中";
    case "paused":
      return "已暂停";
    case "merging":
      return "合并中";
    case "success":
      return "已完成";
    case "error":
      return "失败";
    case "cancelled":
      return "已取消";
    default:
      return status;
  }
}

function uploadStatusTagType(status: BizDocument.ChunkTaskStatus) {
  switch (status) {
    case "success":
      return "success";
    case "error":
      return "danger";
    case "paused":
      return "warning";
    case "cancelled":
      return "info";
    default:
      return "primary";
  }
}

function formatBytes(size: number) {
  if (size >= 1024 * 1024 * 1024) return `${(size / 1024 / 1024 / 1024).toFixed(2)} GB`;
  if (size >= 1024 * 1024) return `${(size / 1024 / 1024).toFixed(2)} MB`;
  if (size >= 1024) return `${(size / 1024).toFixed(2)} KB`;
  return `${size} B`;
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
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
  flex-wrap: wrap;
}

.left-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.upload-entry-badge {
  display: inline-flex;
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

.upload-panel {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.upload-task-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  max-height: 52vh;
  overflow-y: auto;
  padding-right: 4px;
}

.upload-panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.upload-panel-title-wrap {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.upload-panel-title {
  font-size: 15px;
  font-weight: 600;
}

.upload-panel-meta {
  color: var(--el-text-color-secondary);
  font-size: 13px;
}

.upload-summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(110px, 1fr));
  gap: 10px;
}

.upload-summary-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
  padding: 10px 12px;
  border-radius: 8px;
  background: var(--el-fill-color-light);
}

.upload-summary-item .label {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}

.upload-summary-item .value {
  font-size: 18px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}

.upload-summary-item .value.success {
  color: var(--el-color-success);
}

.upload-summary-item .value.danger {
  color: var(--el-color-danger);
}

.upload-summary-item .value.warning {
  color: var(--el-color-warning);
}

.upload-task {
  padding: 12px;
  border: 1px solid var(--el-border-color-light);
  border-radius: 8px;
  background: var(--el-fill-color-blank);
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.upload-task-top,
.upload-task-bottom {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.upload-task-name {
  min-width: 0;
  font-weight: 500;
  word-break: break-all;
}

.upload-task-size,
.upload-task-error {
  font-size: 13px;
}

.upload-task-error {
  color: var(--el-color-danger);
  word-break: break-word;
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
