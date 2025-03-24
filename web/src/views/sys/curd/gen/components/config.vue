<!-- 
 * @Description:  配置步骤
 * @Author: huguodong 
 * @Date: 2025-01-08 16:05:19
!-->
<template>
  <el-card :bordered="true">
    <ProTable
      ref="proTable"
      :request-auto="false"
      :columns="columns"
      :tool-button="false"
      :pagination="false"
      :request-api="genConfigApi.list"
      :init-param="initParam"
      :data-callback="dataCallback"
    >
      <template #fieldRemark="scope">
        <s-input v-model="scope.row.fieldRemark" />
      </template>
      <template #width="scope">
        <s-input v-model="scope.row.width" />
      </template>
      <template #fieldNetType="scope">
        <s-select
          v-model="scope.row.fieldNetType"
          :disabled="toCommonFieldEstimate(scope.row)"
          :options="genConst.fieldNetTypeOptions"
          filterable
          @change="fieldNetTypeChange(scope.row)"
        />
      </template>
      <template #effectType="scope">
        <s-select
          v-model="scope.row.effectType"
          :disabled="toCommonFieldEstimate(scope.row) || toFieldSelectEstimate(scope.row)"
          :options="genConst.effectTypeOptions"
          filterable
          @change="effectTypeChange(scope.row)"
          :class="{ effectType: scope.row.effectType === 'fk' }"
        />
        <el-button
          @click="fkFormRef?.onOpen(scope.row)"
          type="primary"
          link
          v-if="scope.row.effectType === 'fk'"
          class="pl-2 ml-2"
          :icon="View"
        ></el-button>
      </template>
      <template #dictTypeCode="scope">
        <s-select
          v-if="scope.row.effectType === 'radio' || scope.row.effectType === 'select' || scope.row.effectType === 'checkbox'"
          v-model="scope.row.dictTypeCode"
          :options="dictTypeCodeOptions"
          filterable
        />
      </template>
      <template #whetherTable="scope">
        <el-checkbox v-model="scope.row.whetherTable" :value="scope.row.whetherTable" />
      </template>
      <template #whetherRetract="scope">
        <el-checkbox v-model="scope.row.whetherRetract" :value="scope.row.whetherRetract" />
      </template>
      <template #whetherResizable="scope">
        <el-checkbox v-model="scope.row.whetherResizable" :value="scope.row.whetherResizable" />
      </template>
      <template #whetherAddUpdate="scope">
        <el-checkbox v-model="scope.row.whetherAddUpdate" :disabled="toFieldEstimate(scope.row)" :value="scope.row.whetherAddUpdate" />
      </template>
      <template #whetherImportExport="scope">
        <el-checkbox v-model="scope.row.whetherImportExport" :disabled="toFieldEstimate(scope.row)" :value="scope.row.whetherAddUpdate" />
      </template>
      <template #whetherRequired="scope">
        <el-checkbox
          v-model="scope.row.whetherRequired"
          :disabled="toFieldEstimate(scope.row) || !scope.row.whetherAddUpdate"
          :value="scope.row.whetherRequired"
        />
      </template>
      <template #queryWhether="scope">
        <el-switch v-model="scope.row.queryWhether" />
      </template>
      <template #queryType="scope">
        <s-select
          v-if="scope.row.queryWhether === true && scope.row.effectType !== 'datepicker'"
          v-model="scope.row.queryType"
          :options="genConst.queryTypeOptions"
          filterable
        />
        <span v-else-if="scope.row.effectType === 'datepicker' && scope.row.queryWhether === true">时间段</span>
        <span v-else>无</span>
      </template>
    </ProTable>
  </el-card>
  <FkForm ref="fkFormRef" @successful="proTable?.refresh()" />
</template>

<script setup lang="ts">
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { GenCode, genConfigApi } from "@/api";
import * as genConst from "../genConst";
import { useDictStore } from "@/stores/modules";
import FkForm from "./fkForm.vue";
import { View } from "@element-plus/icons-vue";
const dictStore = useDictStore();
// 初始化参数
const initParam = reactive<{ basicId: string | number }>({ basicId: 0 });
const dictTypeCodeOptions = dictStore.getDictAll();
const fkFormRef = ref<InstanceType<typeof FkForm> | null>(null);

// 获取 ProTable 元素
const proTable = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<GenCode.GenConfig>[] = [
  { type: "sort", label: "排序", width: 60 },
  { prop: "fieldName", label: "字段" },
  { prop: "fieldRemark", label: "注释" },
  { prop: "fieldType", label: "类型", width: 100 },
  { prop: "fieldNetType", label: "实体类型", width: 150 },
  { prop: "effectType", label: "作用类型" },
  { prop: "dictTypeCode", label: "字典", width: 140 },
  { prop: "width", label: "列宽", width: 80 },
  { prop: "whetherTable", label: "列表", width: 60 },
  { prop: "whetherRetract", label: "列省略", width: 60 },
  { prop: "whetherResizable", label: "可伸缩列", width: 60 },
  { prop: "whetherAddUpdate", label: "增改", width: 60 },
  { prop: "whetherImportExport", label: "导入导出", width: 60 },
  { prop: "whetherRequired", label: "必填", width: 60 },
  { prop: "queryWhether", label: "查询", width: 80 },
  { prop: "queryType", label: "查询方式", width: 150, fixed: "right" }
];

/**
 * 打开配置步骤
 * @param id  基础信息id
 */
function onOpen(id: number | string) {
  initParam.basicId = id;
}

// dataCallback 是对于返回的表格数据做处理
function dataCallback(data: any) {
  data.forEach(item => {
    for (const key in item) {
      if (item[key] === "Y") {
        item[key] = true;
      }
      if (item[key] === "N") {
        item[key] = false;
      }
    }
    // 去掉删除标识
    // 如果是主键，我们不提供主键的配置，也用不到
    if (
      item.isPrimaryKey ||
      item.fieldName.toLowerCase().includes("isdelete") ||
      item.fieldName.toLowerCase().includes("createuserid") ||
      item.fieldName.toLowerCase().includes("updateuserid")
    ) {
      data = data.filter(table => {
        return table.fieldName != item.fieldName;
      });
    }
    // 让默认的变成设置的
    fieldNetTypeChange(item);
  });
  return data;
}

/**
 *通用字段是否可选
 * @param record
 */
function toCommonFieldEstimate(record: GenCode.GenConfig) {
  if (record.fieldName.toLowerCase().includes("createuser") || record.fieldName.toLowerCase().includes("updateuser")) {
    return true;
  }
  return false;
}

/**
 *
 * @param record 通用字段是否可选
 */
function toFieldSelectEstimate(record) {
  if (record.fieldNetType === "DateTime" && record.effectType === "datepicker") {
    return true;
  }
  return false;
}

/**
 *
 * @param record 改变作用类型
 * @param record
 */
function effectTypeChange(record: GenCode.GenConfig) {
  console.log("value", record);
  if (record.effectType === "fk") {
    console.log("value", record);

    fkFormRef.value?.onOpen(record);
  } else {
    record.fkEntityName = "";
    record.fkColumnName = "";
    record.fkColumnId = "";
  }
  console.log(record);
}

/**
 * 实体类型选择触发
 */
function fieldNetTypeChange(record: GenCode.GenConfig) {
  if (record.fieldNetType === "DateTime") {
    record.effectType = "datepicker";
  }
}

/**
 * 通用字段是否可选
 * @param data
 */
function toFieldEstimate(data: GenCode.GenConfig) {
  if (
    data.fieldName.toLowerCase().includes("createuser") ||
    data.fieldName.toLowerCase().includes("createtime") ||
    data.fieldName.toLowerCase().includes("updateuser") ||
    data.fieldName.toLowerCase().includes("updatetime") ||
    data.fieldName.toLowerCase().includes("isdelete") ||
    data.fieldName.toLowerCase().includes("createuserid") ||
    data.fieldName.toLowerCase().includes("updateuserid") ||
    data.isPrimaryKey === "true"
  ) {
    return true;
  }
  return false;
}

/**
 * 验证并提交数据
 */
function onSubmit(): Promise<any> {
  let data = proTable.value?.tableData as GenCode.GenConfig[];
  let error = false;
  data.forEach(item => {
    // 必填那一项转换
    for (const key in item) {
      //不转isdeleste
      if (key != "isDelete") {
        if (item[key] === true) {
          item[key] = "Y";
        }
        if (item[key] === false) {
          item[key] = "N";
        }
      }
    }
    if (item.queryWhether === "Y" && !item.queryType) {
      // 排除掉时间选择
      if (item.fieldNetType !== "DateTime" && item.effectType !== "checkbox" && item.fieldNetType !== "Date") {
        error = true;
      }
    }
    if ((item.effectType === "select" || item.effectType === "radio" || item.effectType === "checkbox") && !item.dictTypeCode) {
      error = true;
    }
  });
  return new Promise((resolve, reject) => {
    if (error) {
      reject("校验失败，请选择对应的下拉框选项");
      return;
    }
    genConfigApi
      .editBatch(data)
      .then(res => {
        resolve(res.data);
      })
      .catch(err => {
        reject(err);
      });
  });
}

// 暴露方法
defineExpose({ onOpen, onSubmit });
</script>

<style lang="scss" scoped>
.effectType {
  width: 75%;
}
</style>
