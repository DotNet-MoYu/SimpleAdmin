<!-- 
 * @Description: 测试管理表单页面
 * @Author: superAdmin  
 * @Date: 2025-03-20 14:12:55
!-->

<template>
  <form-container v-model="visible" :title="`${testPros.opt}测试`" form-size="600px">
    <el-form
      ref="testFormRef"
      :rules="rules"
      :disabled="testPros.disabled"
      :model="testPros.record"
      :hide-required-asterisk="testPros.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="姓名" prop="name">
        <s-input v-model="testPros.record.name"></s-input>
      </s-form-item>
      <s-form-item label="性别" prop="sex">
        <s-radio-group v-model="testPros.record.sex" :options="sexOptions" button />
      </s-form-item>
      <s-form-item label="民族" prop="nation">
        <s-select v-model="testPros.record.nation" :options="nationOptions"></s-select>
      </s-form-item>
      <s-form-item label="年龄" prop="age">
        <el-input-number v-model="testPros.record.age" :min="1" :max="100" />
      </s-form-item>
      <s-form-item label="生日" prop="bir">
        <date-picker v-model="testPros.record.bir"></date-picker>
      </s-form-item>
      <s-form-item label="存款" prop="money">
        <s-input v-model="testPros.record.money"></s-input>
      </s-form-item>
      <s-form-item label="排序码" prop="sortCode">
        <s-input v-model="testPros.record.sortCode"></s-input>
      </s-form-item>
      <s-form-item label="状态" prop="status">
        <s-select v-model="testPros.record.status" :options="statusOptions"></s-select>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!testPros.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { genTestApi, GenTest } from "@/api";
import { FormOptEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore();
const sexOptions = dictStore.getDictList("GENDER"); //性别选项
const nationOptions = dictStore.getDictList("NATION"); //民族选项
const statusOptions = dictStore.getDictList("COMMON_STATUS"); //状态选项

// 表单参数
const testPros = reactive<FormProps.Base<GenTest.GenTestInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

const rules = reactive({
  name: [required("请输入姓名")],
  sex: [required("请选择性别")],
  nation: [required("请选择民族")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<GenTest.GenTestInfo>) {
  Object.assign(testPros, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    testPros.record.sex = sexOptions[0].value;
    testPros.record.nation = nationOptions[0].value;
    testPros.record.age = 0;
    //如果是新增,设置默认值
    testPros.record.sortCode = 99;
    testPros.record.status = statusOptions[0].value;
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    genTestApi.detail({ id: props.record.id }).then(res => {
      testPros.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const testFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  testFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await genTestApi
      .submitForm(testPros.record, testPros.record.id != undefined)
      .then(() => {
        testPros.successful!(); //调用父组件的successful方法
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
