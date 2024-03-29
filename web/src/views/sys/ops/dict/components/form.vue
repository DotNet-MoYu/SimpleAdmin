<!-- 
 * @Description: 字典表单
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:05
!-->
<template>
  <form-container v-model="visible" :title="`${dictProps.opt}字典`" form-size="600px">
    <el-form
      ref="dictFormRef"
      :rules="rules"
      :disabled="dictProps.disabled"
      :model="dictProps.record"
      :hide-required-asterisk="dictProps.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="字典名称" prop="dictLabel">
        <s-input v-model="dictProps.record.dictLabel"></s-input>
      </s-form-item>
      <s-form-item label="字典值" prop="dictValue">
        <s-input v-model="dictProps.record.dictValue"></s-input>
      </s-form-item>
      <s-form-item label="字典状态" prop="status">
        <s-radio-group v-model="dictProps.record.status" :options="statusOptions" />
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
import { SysDict, sysDictApi } from "@/api";
import { required } from "@/utils/formRules";
import { useDictStore } from "@/stores/modules";
import { FormOptEnum, CommonStatusEnum, SysDictEnum, DictCategoryEnum } from "@/enums";
import { FormInstance } from "element-plus";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库

// 字典类型选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);

// 表单参数接口
interface DictProps extends FormProps.Base<SysDict.DictInfo> {
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
  visible.value = true; //显示表单
}

// 提交数据（新增/编辑）
const dictFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  dictFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysDictApi
      .submitForm(dictProps.record, dictProps.record.id != undefined)
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
