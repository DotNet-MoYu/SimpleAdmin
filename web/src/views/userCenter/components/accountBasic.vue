<!-- 
 * @Description: 基本信息
 * @Author: huguodong 
 * @Date: 2024-03-06 10:20:46
!-->
<template>
  <el-form
    ref="formRef"
    :model="formData"
    :rules="formRules"
    label-position="right"
    label-width="170px"
    class="pt-4 w-800px"
    label-suffix=" :"
    size="large"
  >
    <s-form-item label="账号" prop="account">
      {{ formData.account }}
    </s-form-item>
    <s-form-item label="姓名" prop="name">
      <s-input v-model="formData.name"></s-input>
    </s-form-item>
    <s-form-item label="手机" prop="phone">
      <s-input v-model="formData.phone"></s-input>
    </s-form-item>
    <s-form-item label="昵称" prop="nickname">
      <s-input v-model="formData.nickname"></s-input>
    </s-form-item>
    <s-form-item label="性别" prop="gender">
      <s-radio-group v-model="formData.gender" :options="genderOptions" />
    </s-form-item>
    <s-form-item label="出生日期" prop="birthday">
      <date-picker v-model="formData.birthday"></date-picker>
    </s-form-item>
    <s-form-item label="邮箱" prop="email">
      <s-input v-model="formData.email"></s-input>
    </s-form-item>
    <el-form-item>
      <el-button type="primary" :loading="loading" @click="handleSubmit"> 保存基本信息 </el-button>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { Login, userCenterApi } from "@/api";
import { SysDictEnum } from "@/enums";
import { useUserStore, useDictStore } from "@/stores/modules";
import { FormInstance } from "element-plus";
const userStore = useUserStore(); // 用户仓库
const dictStore = useDictStore(); // 字典仓库
const userInfo = userStore.userInfoGet; // 用户信息
const loading = ref(false); // 加载状态
const genderOptions = dictStore.getDictList(SysDictEnum.GENDER); // 性别选项

// 表单数据
const formData = ref<Login.LoginUserInfo>({
  ...(userInfo as Login.LoginUserInfo)
});

const formRules = {
  name: [{ required: true, message: "请输入姓名", trigger: "blur" }] // 表单验证规则
};

const formRef = ref<FormInstance>();

/** 更新基本信息 */
function handleSubmit() {
  formRef.value?.validate(async valid => {
    if (!valid) return; // 表单验证失败
    loading.value = true; // 加载中
    // 提交表单
    userCenterApi
      .updateUserInfo(formData.value)
      .then(() => {
        // 更新前端缓存
        userStore.setUserInfo(formData.value);
      })
      .finally(() => {
        loading.value = false; // 加载完成
      });
  });
}
</script>

<style lang="scss" scoped>
/* 自定义CSS样式以控制标签和包装器的布局 */
.label-col {
  flex-basis: 30%; /* 或者你想要的宽度 */
  text-align: right;
}
.wrapper-col {
  flex-grow: 1;
}
</style>
