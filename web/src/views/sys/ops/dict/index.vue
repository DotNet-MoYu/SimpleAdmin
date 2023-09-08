<!-- 系统字典 -->
<template>
  <div class="main-box gap-10px">
    <div class="w-7/12">
      <ProTable
        ref="proTable"
        title="系统字典"
        :columns="columns"
        :tool-button="false"
        :request-api="getDictPage"
        :init-param="initParam"
        @current-change="handleCurrentChange"
      >
        <!-- 表格 header 按钮 -->
        <template #tableHeader="scope">
          <el-space>
            <el-radio-group v-model="initParam.category" class="mb-15px">
              <el-radio-button v-for="(item, index) in dctTypeOptions" :key="index" :label="item.value">{{
                item.label
              }}</el-radio-button>
            </el-radio-group>
            <el-input v-model="initParam.searchKey" placeholder="请输入字典名称" class="mb-15px">
              <template #append>
                <el-button :icon="Search" class="el-input-button" @click="RefreshTable" />
              </template>
            </el-input>
            <el-button type="primary" :icon="CirclePlus" @click="onOpen(FormOptEnum.ADD, initParam.category as DictCategoryEnum)">
              新增字典
            </el-button>
            <el-button
              type="danger"
              :icon="Delete"
              plain
              :disabled="!scope.isSelected"
              @click="onDelete(scope.selectedListIds, '删除所选字典')"
            >
              删除字典
            </el-button>
          </el-space>
        </template>
        <!-- 状态 -->
        <template #status="scope">
          <el-tag v-if="scope.row.status === CommonStatusEnum.ENABLE" type="success">{{
            dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status)
          }}</el-tag>
          <el-tag v-else type="danger">{{ dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status) }}</el-tag>
        </template>

        <!-- 菜单操作 -->
        <template #operation="scope">
          <el-button
            type="primary"
            link
            :icon="EditPen"
            @click="onOpen(FormOptEnum.EDIT, initParam.category as DictCategoryEnum, scope.row.parentId, scope.row)"
          >
            编辑
          </el-button>
          <el-button type="primary" link :icon="Delete" @click="onDelete([scope.row.id], `删除【${scope.row.dictLabel}】字典`)">
            删除
          </el-button>
        </template>
      </ProTable>
    </div>
    <ProTable
      ref="proTableChild"
      title="系统字典"
      :columns="columns"
      :tool-button="false"
      :request-api="dictPageApi"
      :request-auto="false"
      :init-param="childInitParam"
    >
      <!-- 表格 header 按钮 -->
      <template #tableHeader="scope">
        <el-space>
          <el-input v-model="childInitParam.searchKey" placeholder="请输入字典名称" class="mb-15px">
            <template #append>
              <el-button :icon="Search" class="el-input-button" />
            </template>
          </el-input>
          <el-button
            type="primary"
            :icon="CirclePlus"
            @click="onOpen(FormOptEnum.ADD, childInitParam.category as DictCategoryEnum, childInitParam.parentId)"
          >
            新增字典
          </el-button>
          <el-button
            type="danger"
            :icon="Delete"
            plain
            :disabled="!scope.isSelected"
            @click="onDelete(scope.selectedListIds, '删除所选字典')"
          >
            删除字典
          </el-button>
        </el-space>
      </template>
      <!-- 状态 -->
      <template #status="scope">
        <el-tag v-if="scope.row.status === CommonStatusEnum.ENABLE" type="success">{{
          dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status)
        }}</el-tag>
        <el-tag v-else type="danger">{{ dictStore.dictTranslation(SysDictEnum.COMMON_STATUS, scope.row.status) }}</el-tag>
      </template>
      <!-- 菜单操作 -->
      <template #operation="scope">
        <el-button
          type="primary"
          link
          :icon="EditPen"
          @click="onOpen(FormOptEnum.EDIT, childInitParam.category as DictCategoryEnum, scope.row.parentId, scope.row)"
        >
          编辑
        </el-button>
        <el-button type="primary" link :icon="Delete" @click="onDelete([scope.row.id], `删除【${scope.row.dictLabel}字典`)">
          删除
        </el-button>
      </template>
    </ProTable>
    <!-- 新增/编辑表单 -->
    <Form ref="formRef" />
  </div>
</template>

<script setup lang="ts" name="opsDict">
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";
import { CirclePlus, Search, EditPen, Delete } from "@element-plus/icons-vue";
import { Dict, dictPageApi, dictDeleteApi } from "@/api";
import { useHandleData } from "@/hooks/useHandleData";
import { SysDictEnum, FormOptEnum, CommonStatusEnum, DictCategoryEnum } from "@/enums";
import { useDictStore } from "@/stores/modules";
import Form from "./components/form.vue";
import { ElMessage } from "element-plus";

const dictStore = useDictStore(); //字典仓库
// 左侧表格初始化条件
interface InitParam {
  category?: string; //字典分类
  searchKey?: string; //关键字
}
//右侧表格列初始化条件
interface ChildInitParam extends InitParam {
  parentId: number | string; //父Id
}
const defaultParentId: string | number = -1; //默认父Id
// 如果表格需要初始化请求参数，直接定义传给 ProTable(之后每次请求都会自动带上该参数，此参数更改之后也会一直带上，改变此参数会自动刷新表格数据)
const initParam = reactive<InitParam>({ category: DictCategoryEnum.FRM }); //主表格初始化参数
const childInitParam = reactive<ChildInitParam>({ parentId: defaultParentId }); //子表格初始化参数

// 如果你想在请求之前对当前请求参数做一些操作，可以自定义如下函数：params 为当前所有的请求参数（包括分页），最后返回请求列表接口
// 默认不做操作就直接在 ProTable 组件上绑定	:requestApi="getUserList"
const getDictPage = (params: any) => {
  childInitParam.parentId = defaultParentId;
  return dictPageApi(params);
};

// 字典类型选项
const dctTypeOptions = dictStore.getDictList(SysDictEnum.DICT_CATEGORY);

// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
const proTableChild = ref<ProTableInstance>();

// 表格配置项
const columns: ColumnProps<Dict.DictInfo>[] = [
  { type: "selection", fixed: "left", width: 50 },
  { prop: "dictLabel", label: "字典名称" },
  { prop: "dictValue", label: "字典值" },
  { prop: "status", label: "状态", width: 100 },
  { prop: "sortCode", label: "排序", width: 80 },
  { prop: "operation", label: "操作", fixed: "right", width: 150 }
];

// 表单引用
const formRef = ref<InstanceType<typeof Form> | null>(null);

/**
 * 打开表单
 * @param opt  操作类型
 * @param record  记录
 */
function onOpen(opt: FormOptEnum, category: DictCategoryEnum, parentId: number | string = 0, record: {} | Dict.DictInfo = {}) {
  //如果parentId是-1代表没有选择任何字典，此时不打开表单
  if (parentId === defaultParentId) {
    ElMessage("请先选择字典");
  } else {
    formRef.value?.onOpen({
      opt: opt,
      record: record,
      category: category,
      parentId: parentId,
      successful: RefreshTable
    });
  }
}

/**
 * 删除
 * @param ids  id数组
 */
async function onDelete(ids: string[], msg: string) {
  // 二次确认 => 请求api => 刷新表格
  await useHandleData(dictDeleteApi, { ids }, msg);
  RefreshTable();
}

/**
 * 刷新表格
 */
function RefreshTable() {
  proTable.value?.refresh(); //刷新主表格
  proTableChild.value?.refresh(); //刷新子表格
}

/**
 * 表格当前行变化
 * @param val 选中的值
 */
function handleCurrentChange(val: Dict.DictInfo | undefined) {
  // 如果val存在，则将parentId和category赋值给childInitParam
  if (val) {
    childInitParam.parentId = val.id;
    childInitParam.category = val.category;
  }
}
</script>

<style lang="scss" scoped></style>
