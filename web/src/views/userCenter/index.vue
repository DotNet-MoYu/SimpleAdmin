<!-- 
 * @Description: 个人中心
 * @Author: huguodong 
 * @Date: 2024-03-01 09:56:39
!-->
<template>
  <div>
    <el-row :gutter="24">
      <el-col :lg="7" :md="24" style="padding-right: 0">
        <el-card>
          <div class="account-center-avatarHolder">
            <div class="avatar">
              <img :src="userInfo?.avatar" />
              <a style="cursor: pointer" @click="uploadAvatar">
                <div :class="userInfo?.avatar ? 'mask' : 'mask-notImg'">
                  <el-icon><Upload /></el-icon>
                </div>
              </a>
            </div>
            <div class="username">{{ userInfo?.name }}</div>
            <div class="bio">{{ userInfo?.nickname }}</div>
          </div>
          <div class="account-center-detail">
            <p><i class="title"></i>{{ userInfo?.positionName }}</p>
            <p><i class="group"></i>{{ userInfo?.orgNames }}</p>
            <p>
              <i class="address"></i>
              <span>{{ userInfo?.homeAddress ? userInfo?.homeAddress : "暂无地址" }}</span>
            </p>
          </div>
          <el-divider />
          <div class="account-center-team">
            <div v-if="userInfo?.signature" class="mb-2" style="width: 100%">
              <el-image :src="userInfo?.signature" style="height: 120px; border: 1px solid rgb(236 236 236)" width="100%" />
            </div>
            <el-button @click="openSignName">打开签名板</el-button>
          </div>
        </el-card>
      </el-col>

      <el-col :lg="17" :md="24">
        <div class="card">
          <el-tabs v-model="activeName" class="-mt-4">
            <el-tab-pane label="基本信息" name="accountBasic"><AccountBasic /></el-tab-pane>
            <el-tab-pane label="账号相关" name="file"><AccountBind /></el-tab-pane>
            <el-tab-pane label="快捷方式" name="other"><ShortcutSetting /></el-tab-pane>
          </el-tabs>
        </div>
      </el-col>
    </el-row>
    <sign-name ref="signNameRef" :image="userInfo?.signature" @successful="signSuccess"></sign-name>
    <crop-upload ref="cropUploadRef" :img-src="userInfo?.avatar" @successful="uploadSuccess"></crop-upload>
  </div>
</template>

<script setup lang="ts" name="userCenter">
import { userCenterApi } from "@/api";
import SignName from "./components/signName.vue";
import AccountBasic from "./components/accountBasic.vue";
import AccountBind from "./components/accountBind.vue";
import ShortcutSetting from "./components/shortcutSetting.vue";
import { useUserStore } from "@/stores/modules";
import { CropUploadInstance } from "@/components/CropUpload/interface";
const userStore = useUserStore();
const userInfo = userStore.userInfoGet;
const activeName = ref("accountBasic");
const cropUploadRef = ref<CropUploadInstance>();

/** 上传头像 */
function uploadAvatar() {
  cropUploadRef.value?.show();
}

const signNameRef = ref();

/** 打开签名板 */
function openSignName() {
  signNameRef.value.open();
}

/** 签名成功回调 */
function signSuccess(url: string) {
  console.log("[ url ] >", url);
  const param = {
    signature: url
  };
  userCenterApi.updateSignature(param).then(() => {
    userStore.setUserInfoItem("signature", url);
  });
}

/** 上传成功回调 */
function uploadSuccess(data: { fileName: string; blobData: any }) {
  let result = null;
  // 如果没有文件名，就转换为file类型，否则就是file类型
  if (data.fileName === "") {
    result = blobToFile(data.blobData, "avatar.jpg");
  } else {
    // 转换为file类型
    result = new File([data.blobData], data.fileName, {
      type: "image/jpeg",
      lastModified: Date.now()
    });
  }
  let formData = new FormData();
  formData.append("file", result);
  userCenterApi.updateAvatar(formData).then(res => {
    userStore.setUserInfoItem("avatar", res.data);
  });
}

/** blob转file */
function blobToFile(blob: BlobPart, fileName: string) {
  return new File([blob], fileName, {
    type: "image/jpeg",
    lastModified: Date.now()
  });
}
</script>

<style lang="scss" scoped>
.account-center-avatarHolder {
  margin-bottom: 24px;
  text-align: center;
  & > .avatar {
    width: 104px;
    height: 104px;
    margin: 0 auto;
    margin-bottom: 20px;
    overflow: hidden;
    border-radius: 50%;
    img {
      width: 100%;
      height: 100%;
    }
  }
  .mask {
    position: absolute;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 104px;
    height: 104px;
    margin-top: -104px;
    font-size: 25px;
    color: #ffffff;
    background: rgb(101 101 101 / 60%);
    border-radius: 50%;
    opacity: 0;
  }
  .mask-notImg {
    position: absolute;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 104px;
    height: 104px;
    margin-top: -24px;
    font-size: 25px;
    color: #ffffff;
    background: rgb(101 101 101 / 60%);
    border-radius: 50%;
    opacity: 0;
  }
  .avatar a:hover .mask {
    opacity: 1;
  }
  .avatar a:hover .mask-notImg {
    opacity: 1;
  }
  .username {
    margin-bottom: 4px;
    font-size: 20px;
    font-weight: 500;
    line-height: 28px;
  }
}
.account-center-detail {
  p {
    position: relative;
    padding-left: 26px;
    margin-bottom: 8px;
  }
  i {
    position: absolute;
    top: 4px;
    left: 0;
    width: 14px;
    height: 14px;
    background: url("/src/assets/icons/userDetail.svg");
  }
  .title {
    background-position: 0 0;
  }
  .group {
    background-position: 0 -22px;
  }
  .address {
    background-position: 0 -44px;
  }
}
.account-center-team {
  .members {
    a {
      display: block;
      height: 24px;
      margin: 12px 0;
      line-height: 24px;
      .member {
        display: inline-block;
        max-width: 100px;
        margin-left: 12px;
        font-size: 14px;
        line-height: 24px;
        vertical-align: top;
        transition: all 0.3s;
      }
    }
  }
}
:deep(.el-tabs__item) {
  font-size: large;
}
</style>
