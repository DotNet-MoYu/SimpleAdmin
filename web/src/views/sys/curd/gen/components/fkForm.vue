<!-- 
 * @Description: 外键关系配置
 * @Author: huguodong 
 * @Date: 2025-02-05 11:02:14
!-->
<template>
  <form-container v-model="visible" title="外键关系配置" form-size="600px">
    <el-form ref="fkFormRef" :model="fkProps.fkInfo" :rules="rules" label-width="auto" label-suffix=" :">
      <s-form-item label="选择外键表" prop="fkEntityName">
        <s-select v-model="fkProps.fkInfo.fkEntityName" :options="fkProps.tableOptions" filterable @change="formFieldAssign" />
      </s-form-item>
      <s-form-item label="外键ID" prop="fkColumnId">
        <s-select v-model="fkProps.fkInfo.fkColumnId" :options="fkProps.columnOptions" filterable />
      </s-form-item>
      <s-form-item label="显示字段" prop="fkColumnName">
        <s-select v-model="fkProps.fkInfo.fkColumnName" :options="fkProps.columnOptions" filterable />
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button type="primary" @click="onSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { GenCode, genBasicApi } from "@/api";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
const fkFormRef = ref<FormInstance>();

const visible = ref(false); //是否显示表单
// 选项接口
interface FkProps {
  /*数据表列表 */
  tableOptions: GenCode.TableList[];
  /*字段选项 */
  columnOptions: { label: string; value: string }[];
  /*当前行数据 */
  fkInfo: GenCode.GenConfig;
}

// 表单参数
const fkProps: FkProps = reactive({
  tableOptions: [],
  columnOptions: [],
  fkInfo: {} as GenCode.GenConfig
});

// 表单验证规则
const rules = reactive({
  fkEntityName: [required("请选择外键表")],
  fkColumnId: [required("请选择外键Id")],
  fkColumnName: [required("请选择外键字段")]
});

/**
 * 打开表单
 * @param genConfig 表单参数
 */
function onOpen(genConfig: GenCode.GenConfig) {
  visible.value = true; //显示抽屉
  console.log("genConfig", genConfig);
  fkProps.fkInfo = genConfig;
  // 获取数据库中的所有表
  genBasicApi.tables({ isAll: true }).then(res => {
    const data = res.data;
    fkProps.tableOptions = data.map(item => {
      return {
        value: item.tableName,
        tableName: item.tableName,
        configId: item.configId,
        label: `${item.tableDescription}-${item.configId}-${item.entityName}`,
        entityName: item.entityName,
        tableDescription: item.tableDescription || item.entityName,
        tableColumns: item.columns
      };
    });
    const option = fkProps.tableOptions.find(item => item.value === genConfig.fkEntityName);
    if (option) {
      fkProps.columnOptions = option.tableColumns.map(item => {
        return {
          value: item.columnName,
          label: `${item.columnName}:${item.columnDescription}`
        };
      });
    }
  });
}

/**
 * 表单内设置默认的值
 * @param value
 * @param options
 */
function formFieldAssign(value: string) {
  fkProps.fkInfo.fkColumnName = "";
  fkProps.fkInfo.fkColumnId = "";
  const option = fkProps.tableOptions.find(item => item.value === value);
  if (option) {
    fkProps.columnOptions = option.tableColumns.map(item => {
      return {
        value: item.columnName,
        label: `${item.columnName}:${item.columnDescription}`
      };
    });
  }
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
}
/** 提交表单*/
function onSubmit() {
  fkFormRef.value?.validate(valid => {
    if (!valid) return; //表单验证失败
    onClose(); //关闭表单
  });
}
defineExpose({ onOpen });
</script>

<style lang="scss" scoped></style>
