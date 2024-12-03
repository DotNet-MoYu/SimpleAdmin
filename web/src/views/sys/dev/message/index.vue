<!-- 
 * @Description:站内消息
 * @Author: huguodong 
 * @Date: 2024-11-08 16:16:52
!-->
<template>
  <div class="table-box">
    <ProTable ref="proTable" title="消息列表" :columns="columns" :request-api="messageApi.page">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button suffix="站内信" @click="onOpen(FormOptEnum.ADD)" />
        <s-button
          type="danger"
          plain
          suffix="站内信"
          :opt="FormOptEnum.DELETE"
          :disabled="!scope.isSelected"
          @click="onDelete(scope.selectedListIds, '删除所选信息')"
        />
      </template>
      <!-- 表格 状态 -->
      <template #status="scope">
        <el-space wrap>
          <el-tag v-if="scope.row.status === MessageStatusDictEnum.ALREADY" type="success">{{
            dictStore.dictTranslation(SysDictEnum.MESSAGE_STATUS, MessageStatusDictEnum.ALREADY)
          }}</el-tag>
          <el-tag v-else-if="scope.row.status === MessageStatusDictEnum.READY" type="info">{{
            dictStore.dictTranslation(SysDictEnum.MESSAGE_STATUS, MessageStatusDictEnum.READY)
          }}</el-tag>
        </el-space>
      </template>
      <!-- 表格 类型 -->
      <template #category="scope">
        <el-space wrap>
          <el-tag v-if="scope.row.category === MessageTypeDictEnum.INFORM" type="danger">{{
            dictStore.dictTranslation(SysDictEnum.MESSAGE_CATEGORY, MessageTypeDictEnum.INFORM)
          }}</el-tag>
          <el-tag v-else-if="scope.row.category === MessageTypeDictEnum.MESSAGE" type="primary">{{
            dictStore.dictTranslation(SysDictEnum.MESSAGE_CATEGORY, MessageTypeDictEnum.MESSAGE)
          }}</el-tag>
          <el-tag v-else-if="scope.row.category === MessageTypeDictEnum.NOTICE" type="warning">{{
            dictStore.dictTranslation(SysDictEnum.MESSAGE_CATEGORY, MessageTypeDictEnum.NOTICE)
          }}</el-tag>
        </el-space>
      </template>
      <!-- 操作 -->
      <template #operation="scope">
        <s-button v-if="scope.row.status === MessageStatusDictEnum.READY" link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)" />
        <s-button v-if="scope.row.status === MessageStatusDictEnum.ALREADY" link :opt="FormOptEnum.VIEW" @click="onOpenDetail(scope.row)" />
        <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.title}】信息`)" />
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
    <!-- 详情表单 -->
    <Detail ref="detailRef" />
  </div>
</template>

<script setup lang="tsx" name="sysSpa">
import { messageApi } from "@/api";
import { SysMessage } from "@/api/interface";
import { useHandleData } from "@/hooks/useHandleData";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { useDictStore } from "@/stores/modules";
import { FormOptEnum, SysDictEnum, MessageStatusDictEnum, MessageTypeDictEnum } from "@/enums";
import Form from "./components/form.vue";
import Detail from "./components/detail.vue";

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();

// 单页类型选项
const messageTypeOptions = dictStore.getDictList(SysDictEnum.MESSAGE_CATEGORY);

// 状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.MESSAGE_STATUS);
// 发送方式选项
const sendWayOptions = dictStore.getDictList(SysDictEnum.MESSAGE_WAY);

// 表格配置项
const columns: ColumnProps<SysMessage.SysMessageInfo>[] = [
  { type: "selection", fixed: "left", width: 80 },
  { prop: "searchKey", label: "主题关键字", search: { el: "input" }, isShow: false },
  { prop: "subject", label: "主题" },
  {
    prop: "category",
    label: "消息类型",
    enum: messageTypeOptions,
    search: { el: "select" }
  },
  { prop: "content", label: "正文" },
  { prop: "sendWay", label: "发送方式", enum: sendWayOptions },
  { prop: "createTime", label: "创建时间" },
  { prop: "sendTime", label: "发送时间" },
  { prop: "status", label: "状态", enum: statusOptions, search: { el: "select" } },
  { prop: "operation", label: "操作", width: 200, fixed: "right" }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | SysMessage.SysMessageInfo = {}) {
  formRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(messageApi.delete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh();
}

// 表单引用
const detailRef = ref<InstanceType<typeof Detail> | null>(null);

/**
 * 打开详情
 * @param opt  操作类型
 * @param record  记录
 */
function onOpenDetail(record: SysMessage.SysMessageInfo) {
  detailRef.value?.onOpen({ opt: FormOptEnum.VIEW, record: record, successful: RefreshTable });
}
</script>

<style lang="scss" scoped></style>
