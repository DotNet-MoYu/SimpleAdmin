<!-- 
 * @Description: 代码生成器
 * @Author: huguodong 
 * @Date: 2025-01-07 16:24:17
!-->
<template>
  <div class="table-box">
    <ProTable v-if="indexShow" ref="proTable" title="代码生成器列表" :columns="columns" :request-api="genBasicApi.page">
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <s-button @click="onOpen(FormOptEnum.ADD)" />
        <s-button
          type="danger"
          plain
          :opt="FormOptEnum.DELETE"
          :disabled="!scope.isSelected"
          @click="onDelete(scope.selectedListIds, '删除所选代码生成')"
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
        <s-button link :opt="FormOptEnum.VIEW" @click="previewRef?.onOpen(scope.row)">预览</s-button>
        <s-button link :opt="FormOptEnum.EDIT" @click="onOpen(FormOptEnum.EDIT, scope.row)">配置</s-button>
        <s-button link :icon="VideoPlay" @click="choseExecGen(scope.row)">生成</s-button>
        <s-button link :opt="FormOptEnum.DELETE" @click="onDelete([scope.row.id], `删除【${scope.row.dbTable}】信息`)" />
      </template>
    </ProTable>
    <Steps v-else ref="stepsRef" @closed="closeStep" />
    <Preview ref="previewRef" />
    <el-dialog v-model="dialogVisible" title="生成选项" width="400">
      <s-radio-group v-model="execType" :options="genConst.execTypeOptions" size="large"></s-radio-group>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="execGen">确定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="tsx" name="genCode">
import { VideoPlay } from "@element-plus/icons-vue";
import { genBasicApi, GenCode } from "@/api";
import { useHandleData } from "@/hooks/useHandleData";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { useDictStore } from "@/stores/modules";
import { FormOptEnum, SysDictEnum, MessageStatusDictEnum, MessageTypeDictEnum } from "@/enums";
import * as genConst from "./genConst";
import Steps from "./components/steps.vue";
import Preview from "./components/preview.vue";
// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
const dictStore = useDictStore();
const indexShow = ref(true);
const previewRef = ref<InstanceType<typeof Preview> | null>(null); // 预览表单引用
const dialogVisible = ref(false);
const execType = ref(genConst.execTypeOptions[0].value);
const execRecord = ref(); //要生成的id
// 表格配置项
const columns: ColumnProps<GenCode.GenBasic>[] = [
  { type: "selection", fixed: "left", width: 80 },
  { prop: "configId", label: "库名" },
  { prop: "dbTable", label: "表名" },
  { prop: "busName", label: "业务名" },
  { prop: "functionName", label: "功能名" },
  { prop: "className", label: "类名" },
  { prop: "generateType", label: "生成方式", enum: genConst.generateTypeOptions },
  { prop: "moduleType", label: "生成模版", enum: genConst.moduleTypeOptions },
  { prop: "authorName", label: "作者" },
  { prop: "operation", label: "操作", width: 280, fixed: "right" }
];

// 表单引用
const stepsRef = ref<InstanceType<typeof Steps> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, record: {} | GenCode.GenBasic = {}) {
  indexShow.value = false;
  nextTick(() => {
    stepsRef.value?.onOpen({ opt: opt, record: record, successful: RefreshTable });
  });
}

/**
 * 关闭步骤
 */
function closeStep() {
  indexShow.value = true;
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(genBasicApi.delete, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh();
}

//生成方式
// function generateTypeFilter(text: string) {
//   const array = genConst.generateTypeOptions;
//   const foundItem = array.find(f => f.value === text);
//   if (foundItem) {
//     return foundItem.label;
//   } else {
//     // 返回一个默认值或者抛出一个错误
//     return "未知";
//   }
// }
//选择生成前端还是后端
function choseExecGen(record: GenCode.GenBasic) {
  dialogVisible.value = true;
  execRecord.value = record;
}

/**执行代码生成 */
function execGen() {
  console.log("execType", execType);
  const param = {
    id: execRecord.value.id,
    execType: execType.value
  };
  if (execRecord.value.generateType === "PRO") {
    genBasicApi.execGenPro(param).then(() => {
      RefreshTable();
    });
  } else {
    // 下载压缩包
    genBasicApi.execGenZip(param).then(res => {
      console.log("[ res ] >", res);
      let data = res as any;
      const blob = new Blob([data.data], { type: "application/octet-stream;charset=UTF-8" });
      const contentDisposition = data.headers["content-disposition"];
      const patt = new RegExp("filename=([^;]+\\.[^\\.;]+);*");
      const $link = document.createElement("a");
      $link.href = URL.createObjectURL(blob);
      const match = patt.exec(contentDisposition);
      if (match) {
        $link.download = decodeURIComponent(match[1]);
      }
      $link.click();
      document.body.appendChild($link);
      document.body.removeChild($link); // 下载完成移除元素
      window.URL.revokeObjectURL($link.href); // 释放掉blob对象
    });
  }
  execCancel();
}

//取消生成
function execCancel() {
  dialogVisible.value = false;
  execType.value = genConst.execTypeOptions[0].value;
}
</script>

<style lang="scss" scoped></style>
