<!-- 表单 -->
<template>
  <div>
    <form-container v-model="visible" :title="`${sysUserProps.opt}用户`" form-size="800px" @close="onClose">
      <el-form
        ref="sysUserFormRef"
        :rules="rules"
        :disabled="sysUserProps.disabled"
        :model="sysUserProps.record"
        :hide-required-asterisk="sysUserProps.disabled"
        label-width="auto"
        label-suffix=" :"
        class="-mt-25px"
      >
        <el-tabs v-model="activeName">
          <Basic v-model="sysUserProps.record"></Basic>
          <More v-model="sysUserProps.record"></More>
        </el-tabs>
      </el-form>
      <template #footer>
        <el-button @click="onClose"> 取消 </el-button>
        <el-button v-show="!sysUserProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
      </template>
    </form-container>
  </div>
</template>

<script setup lang="ts" name="sysUserForm">
import { SysUser, sysUserApi } from "@/api";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
import Basic from "./form_basic.vue";
import More from "./form_more.vue";

const dictStore = useDictStore(); //字典仓库
const visible = ref(false); //是否显示表单
const activeName = ref("basic");
// 通用状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);

// 表单参数
const sysUserProps = reactive<FormProps.Base<SysUser.SysUserInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  account: [required("请输入账号")],
  name: [required("请输入姓名")],
  gender: [required("请选择性别")],
  positionId: [required("请选择部门和职位")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysUser.SysUserInfo>) {
  Object.assign(sysUserProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    sysUserProps.record.sortCode = 99;
    sysUserProps.record.status = statusOptions[0].value;
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    sysUserApi.sysUserDetail({ id: props.record.id }).then(res => {
      sysUserProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const sysUserFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  sysUserFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysUserApi
      .sysUserSubmitForm(sysUserProps.record, sysUserProps.record.id != undefined)
      .then(() => {
        sysUserProps.successful!(); //调用父组件的successful方法
      })
      .finally(() => {
        onClose();
      });
  });
}

/** 关闭表单*/
function onClose() {
  visible.value = false;
  activeName.value = "basic";
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
