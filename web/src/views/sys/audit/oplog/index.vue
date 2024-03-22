<!-- 
 * @Description: 操作日志
 * @Author: huguodong 
 * @Date: 2023-12-15 15:44:27
!-->
<template>
  <div>
    <el-row :gutter="10" class="mb-2">
      <el-col :span="16">
        <el-card header="周统计" shadow="never">
          <column-chat />
        </el-card>
      </el-col>
      <el-col :span="8">
        <el-card header="总比例" shadow="never">
          <pie-chat />
        </el-card>
      </el-col>
    </el-row>
    <el-row>
      <div class="table-box min-h-400px">
        <ProTable
          ref="proTable"
          title="系统字典"
          class="table"
          :tool-button="false"
          :columns="columns"
          :request-api="opLogApi.page"
          :init-param="initParam"
        >
          <!-- 表格 header 按钮 -->
          <template #tableHeader>
            <el-space>
              <el-radio-group v-model="initParam.category" class="mb-15px">
                <el-radio-button v-for="(item, index) in visLogTypeOptions" :key="index" :label="item.value">{{ item.label }}</el-radio-button>
              </el-radio-group>
              <el-input v-model="initParam.searchKey" placeholder="请输入字典名称" class="mb-15px">
                <template #append>
                  <el-button :icon="Search" class="el-input-button" />
                </template>
              </el-input>
              <s-button type="danger" plain :opt="FormOptEnum.DELETE" @click="onDelete('清空当前日志')">清空</s-button>
            </el-space>
          </template>

          <!-- 操作 -->
          <template #operation="scope">
            <s-button link :opt="FormOptEnum.VIEW" @click="onOpen(scope.row)"> 详情 </s-button>
          </template>
        </ProTable>
        <!-- 日志详情 -->
        <Detail ref="detailRef" />
      </div>
    </el-row>
  </div>
</template>

<script setup lang="ts" name="sysOpLog">
import ColumnChat from "./components/columnChat.vue";
import PieChat from "./components/pieChat.vue";
import Detail from "./components/detail.vue";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { Search } from "@element-plus/icons-vue";
import { OpLog, opLogApi } from "@/api";
import { FormOptEnum } from "@/enums";
import { useHandleData } from "@/hooks/useHandleData";

// 左侧表格初始化条件
interface InitParam {
  category?: string; //字典分类
  searchKey?: string; //关键字
}
// 访问日志类型
let visLogTypeOptions = [
  {
    label: "操作日志",
    value: "OPERATE"
  },
  {
    label: "异常日志",
    value: "EXCEPTION"
  }
];
// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<InitParam>({ category: visLogTypeOptions[0].value }); //主表格初始化参数

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<OpLog.OpLogInfo>[] = [
  { prop: "name", label: "名称" },
  { prop: "opIp", label: "IP地址" },
  { prop: "opAddress", label: "地址" },
  { prop: "className", label: "类名称" },
  { prop: "methodName", label: "方法名称" },
  { prop: "opUser", label: "用户" },
  { prop: "opAccount", label: "账号" },
  { prop: "opTime", label: "操作时间" },
  { prop: "operation", label: "操作", fixed: "right", width: 100 }
];

const detailRef = ref<InstanceType<typeof Detail> | null>(null); //详情引用

/**
 * 打开表单
 * @param record  记录
 */
function onOpen(record: OpLog.OpLogInfo) {
  detailRef.value?.onOpen(record);
}

/**
 * 清空日志
 */
async function onDelete(msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(opLogApi.delete, initParam.category, msg);
  proTable.value?.refresh(); //刷新主表格
}
</script>

<style lang="scss" scoped>
:deep(.el-card__header) {
  padding: 9px;
}
</style>
