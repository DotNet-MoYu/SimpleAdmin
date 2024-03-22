<template>
  <el-dropdown trigger="click">
    <div class="avatar">
      <img :src="userInfo?.avatar" alt="avatar" />
    </div>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item @click="openDialog('infoRef')">
          <el-icon><User /></el-icon>个人中心
        </el-dropdown-item>
        <el-dropdown-item @click="openDialog('changeModuleRef')">
          <el-icon><Switch /></el-icon>切换应用
        </el-dropdown-item>
        <el-dropdown-item divided @click="logout">
          <el-icon><SwitchButton /></el-icon>退出登录
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
  <!-- infoDialog -->
  <InfoDialog ref="infoRef" />
  <!-- passwordDialog -->
  <PasswordDialog ref="passwordRef" />
  <ChooseModule ref="changeModuleRef" />
</template>

<script setup lang="ts">
import { ref } from "vue";
import { LOGIN_URL, USER_CENTER_URL } from "@/config";
import { useRouter } from "vue-router";
import { loginApi } from "@/api/modules";
import { useUserStore } from "@/stores/modules/user";
import { ElMessageBox, ElMessage } from "element-plus";
import InfoDialog from "./InfoDialog.vue";
import PasswordDialog from "./PasswordDialog.vue";
import ChooseModule from "@/components/ChooseModule/index.vue";

const router = useRouter();
const userStore = useUserStore();
const { userInfo } = userStore;
// 退出登录
const logout = () => {
  ElMessageBox.confirm("您是否确认退出登录?", "温馨提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning"
  }).then(async () => {
    const { accessToken } = userStore;
    // 1.执行退出登录接口
    await loginApi.logout({ token: accessToken });

    // 2.清除 Token
    userStore.clearToken();

    // 3.重定向到登陆页
    router.replace(LOGIN_URL);
    ElMessage.success("退出登录成功！");
  });
};

// 打开修改密码和个人信息弹窗
const infoRef = ref<InstanceType<typeof InfoDialog> | null>(null);
const passwordRef = ref<InstanceType<typeof PasswordDialog> | null>(null);
const changeModuleRef = ref<InstanceType<typeof ChooseModule> | null>(null);
const openDialog = (ref: string) => {
  if (ref == "infoRef") router.replace(USER_CENTER_URL);
  if (ref == "changeModuleRef") changeModuleRef.value?.openDialog();
};
</script>

<style scoped lang="scss">
.avatar {
  width: 40px;
  height: 40px;
  overflow: hidden;
  cursor: pointer;
  border-radius: 50%;
  img {
    width: 100%;
    height: 100%;
  }
}
</style>
