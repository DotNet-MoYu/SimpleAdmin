<!-- 
 * @Description: 我的消息
 * @Author: huguodong 
 * @Date: 2024-11-22 13:27:25
!-->
<template>
  <div>
    <el-row :gutter="10" class="min-h-450px">
      <el-col :span="2">
        <el-menu :default-active="activeName">
          <el-menu-item v-for="(item, index) in messageTypeOptions" :key="index" :index="item.value" @click="menuClick">
            <span>{{ item.label }}</span>
          </el-menu-item>
        </el-menu>
      </el-col>
      <el-col :span="20">
        <ProTable
          ref="proTable"
          :tool-button="false"
          title="消息列表"
          :init-param="initParam"
          :columns="columns"
          :request-api="userCenterApi.myMessagePage"
        >
          <template #tableHeader="scope">
            <el-space>
              <!-- 表格 header 按钮 -->
              <el-input v-model="initParam.searchKey" placeholder="请输入关键字">
                <template #append>
                  <el-button :icon="Search" class="el-input-button" @click="RefreshTable" />
                </template>
              </el-input>
              <s-button :opt="FormOptEnum.VIEW" @click="allRead()">全部已读</s-button>
              <s-button :opt="FormOptEnum.VIEW" @click="read(scope.selectedListIds)" :disabled="!scope.isSelected">标记已读</s-button>
              <s-button
                type="danger"
                plain
                suffix="消息"
                :opt="FormOptEnum.DELETE"
                :disabled="!scope.isSelected"
                @click="setDelete(scope.selectedListIds, '删除所选消息')"
              />
              <s-button type="danger" :opt="FormOptEnum.DELETE" @click="allDelete">全部删除</s-button>
            </el-space>
          </template>
          <!-- 表格 已读 -->
          <template #read="scope">
            <el-space wrap>
              <span v-if="scope.row.read" style="color: #d9d9d9">已读</span>
              <span v-else style="color: #ff4d4f">未读</span>
            </el-space>
          </template>
          <!-- 操作 -->
          <template #operation="scope">
            <s-button link :opt="FormOptEnum.VIEW" @click="onOpenDetail(scope.row)" />
            <s-button link :opt="FormOptEnum.DELETE" @click="onDelete(scope.row, `删除【${scope.row.subject}】信息`)" />
          </template>
        </ProTable>
      </el-col>
    </el-row>
    <!-- 详情表单 -->
    <MyMessageDetail ref="detailRef" />
  </div>
</template>

<script setup lang="ts">
import { Search } from "@element-plus/icons-vue";
import { useDictStore, useMessageStore } from "@/stores/modules";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";
import { MenuItemRegistered } from "element-plus";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { SysMessage, userCenterApi, UserCenter } from "@/api";
import MyMessageDetail from "./myMessageDetail.vue";

const dictStore = useDictStore();
const messageStore = useMessageStore();
const proTable = ref<ProTableInstance>();

// 消息类型选项
const messageTypeOptions = dictStore.getDictList(SysDictEnum.MESSAGE_CATEGORY);

const activeName = ref(messageTypeOptions[0].value); // 默认选中第一个
// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<UserCenter.ReqMyMessagePage>({ category: messageTypeOptions[0].value }); //主表格初始化参数

// 表格配置项
const columns: ColumnProps<SysMessage.SysMessageInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "subject", label: "主题" },
  { prop: "content", label: "正文" },
  { prop: "sendTime", label: "发送时间" },
  { prop: "read", label: "是否已读" },
  { prop: "operation", label: "操作", width: 160, fixed: "right" }
];

/** 点击菜单 */
function menuClick(item: MenuItemRegistered) {
  initParam.category = item.index;
  activeName.value = item.index;
}

async function onDelete(record: SysMessage.SysMessageInfo, msg: string) {
  let ids = [record.id];
  // 二次确认 => 请求api => 刷新表格
  const res = await useHandleData(userCenterApi.setDelete, { ids }, msg);
  if (res) {
    // 更新未读消息数量
    if (record.read == false) {
      messageStore.unReadCountSubtract(1);
    }
    RefreshTable();
  }
}

/** 删除消息 */
async function setDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(userCenterApi.setDelete, { ids }, msg);
  messageStore.getUnReadInfo(); //更新未读消息数量
  RefreshTable();
}

// 表单引用
const detailRef = ref<InstanceType<typeof MyMessageDetail> | null>(null);

/**
 * 打开详情
 * @param opt  操作类型
 * @param record  记录
 */
function onOpenDetail(record: SysMessage.SysMessageInfo) {
  record.read = true;
  // 打开详情
  detailRef.value?.onOpen({ opt: FormOptEnum.VIEW, record: record, successful: RefreshTable });
}

/** 标记已读 */
function read(ids: string[] | number[]) {
  //参数转reqId[]
  // const params = record.map(item => ({ id: item }));
  userCenterApi.setRead({ ids }).then(() => {
    // 更新未读消息数量
    messageStore.unReadCountSubtract(ids.length);
    // 标记已读成功
    RefreshTable();
  });
}

/** 全部已读 */
function allRead() {
  userCenterApi
    .allRead({
      category: activeName.value
    })
    .then(res => {
      // 更新未读消息数量,res.data为未读消息数量,转为number
      let count = Number(res.data);
      if (count > 0) {
        messageStore.unReadCountSubtract(count);
      }
      // 标记已读成功
      RefreshTable();
      ElMessage.success("全部已读成功");
    });
}

/** 全部删除 */
async function allDelete() {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(
    userCenterApi.setDelete,
    {
      category: activeName.value
    },
    "删除全部消息"
  );
  messageStore.getUnReadInfo(); //更新未读消息数量
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh();
}
</script>

<style lang="scss" scoped></style>
