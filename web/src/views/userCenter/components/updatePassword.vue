<!-- 
 * @Description: 修改密码
 * @Author: huguodong 
 * @Date: 2024-03-06 15:30:11
!-->
<template>
  <form-container v-model="visible" title="修改密码" form-size="700px">
    <el-form ref="updatePasswordFormRef" :rules="rules" :model="updatePasswordProps" label-width="auto" label-suffix=" :">
      <s-form-item label="旧密码" prop="oldPassword">
        <s-input v-model="updatePasswordProps.oldPassword"></s-input>
      </s-form-item>
      <s-form-item label="新密码" prop="newPassword">
        <s-input v-model="updatePasswordProps.newPassword" type="password"></s-input>
      </s-form-item>

      <s-form-item label="新密码" prop="checkNewPassword">
        <s-input v-model="updatePasswordProps.checkNewPassword" type="password"></s-input>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleCancel"> 取消 </el-button>
      <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { FormInstance } from "element-plus";
import { UserCenter, userCenterApi } from "@/api";
import smCrypto from "@/utils/smCrypto";
import { required } from "@/utils/formRules";
const visible = ref(false); //是否显示表单

const updatePasswordFormRef = ref<FormInstance>(); //表单实例

// 修改密码参数
const updatePasswordProps = reactive({
  oldPassword: "",
  newPassword: "",
  checkNewPassword: ""
});

// 修改密码验证规则
const rules = {
  oldPassword: [required("请输入旧密码")],
  newPassword: [required("请输入新密码")],
  checkNewPassword: [
    required("请再次输入新密码"),
    {
      validator: (rule: any, value: string, callback: (arg0: Error | undefined) => void) => {
        if (value !== updatePasswordProps.newPassword) {
          callback(new Error("两次输入密码不一致"));
        } else {
          callback(undefined);
        }
      },
      trigger: "blur"
    }
  ]
};

/** 提交表单 */
async function handleSubmit() {
  updatePasswordFormRef.value?.validate(async valid => {
    console.log("[ 123 ] >", 123);
    if (!valid) {
      return; // 表单验证失败
    }
    // 加密
    let fromData: UserCenter.ReqUpdatePassword = {
      password: smCrypto.doSm2Encrypt(updatePasswordProps.oldPassword),
      newPassword: smCrypto.doSm2Encrypt(updatePasswordProps.newPassword)
    };
    // 提交数据
    userCenterApi.updatePassword(fromData).then(() => {
      handleCancel();
    });
  });
}

/** 打开表单 */
function onOpen() {
  visible.value = true;
}

function handleCancel() {
  visible.value = false;
  updatePasswordFormRef.value?.resetFields();
}

defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
