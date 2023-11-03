<!-- 字典表单 -->
<template>
  <form-container v-model="visible" :title="`${dictProps.opt}字典`" form-size="600px">
    <el-form
      ref="spaFormRef"
      :rules="rules"
      :disabled="dictProps.disabled"
      :model="dictProps.record"
      :hide-required-asterisk="dictProps.disabled"
      label-width="auto"
    >
      <s-form-item label="字典名称" prop="dictLabel">
        <el-input v-model="dictProps.record.dictLabel" placeholder="请填写单页名称" clearable></el-input>
      </s-form-item>
      <s-form-item label="字典值" prop="dictValue">
        <el-input v-model="dictProps.record.dictValue" placeholder="请填写单页名称" clearable></el-input>
      </s-form-item>
      <s-form-item label="字典状态" prop="status">
        <el-radio-group v-model="dictProps.record.status">
          <el-radio-button v-for="(item, index) in statusOptions" :key="index" :label="item.value">{{ item.label }}</el-radio-button>
        </el-radio-group>
      </s-form-item>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="dictProps.record.sortCode" show-input :min="1" />
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!dictProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { Dict, dictDetailApi, dictSubmitFormApi } from "@/api";
import { required } from "@/utils/formRules";
import { useDictStore } from "@/stores/modules";
import { FormOptEnum, CommonStatusEnum, SysDictEnum, DictCategoryEnum } from "@/enums";
import { FormInstance } from "element-plus";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库

// 字典类型选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);

// 表单参数接口
interface DictProps extends FormProps.Base<Dict.DictInfo> {
  category: DictCategoryEnum; //字典分类
  parentId?: number | string; //父Id
}

// 表单参数
const dictProps = reactive<DictProps>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false,
  category: DictCategoryEnum.FRM
});

// 表单验证规则
const rules = reactive({
  dictLabel: [required("请输入字典名称")],
  dictValue: [required("请输入字典值")],
  status: [required("请选择状态")],
  sortCode: [required("请输入排序")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: DictProps) {
  Object.assign(dictProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    dictProps.record.status = CommonStatusEnum.ENABLE;
    dictProps.record.sortCode = 99;
    dictProps.record.category = props.category;
    dictProps.record.parentId = props.parentId;
  }
  console.log(dictProps.record);

  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    dictDetailApi({ id: props.record.id }).then(res => {
      dictProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const spaFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  spaFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await dictSubmitFormApi(dictProps.record, dictProps.record.id != undefined)
      .then(() => {
        dictProps.successful!(); //调用父组件的successful方法
      })
      .finally(() => {
        onClose();
      });
  });
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
