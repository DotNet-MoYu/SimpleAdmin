<!-- 
 * @Description: 账号相关
 * @Author: huguodong 
 * @Date: 2024-03-06 14:22:43
!-->
<template>
  <div>
    <el-row class="list-container">
      <el-col :span="24" class="list-item" v-for="(item, index) in data" :key="index">
        <!-- 可选的图标或头像部分 -->
        <div v-if="item.avatar">
          <svg-icon :icon="item.avatar" class="bind-icon" :style="{ color: item.color }"></svg-icon>
        </div>
        <!-- 标题和描述部分 -->
        <div class="item-content">
          <h3>{{ item.title }}</h3>
          <p></p>
          <p>
            {{ item.description }}<span v-if="item.value"> : {{ item.value }}</span>
          </p>
        </div>
        <!-- 操作部分 -->
        <div class="item-action">
          <el-button link @click="bindAction(item.bindType)">{{ item.value ? "修改" : "去绑定" }}</el-button>
        </div>
      </el-col>
    </el-row>
    <update-password ref="updatePasswordRef" />
  </div>
</template>

<script setup lang="ts">
//导入修改密码组件
import UpdatePassword from "./updatePassword.vue";

const updatePasswordRef = ref<InstanceType<typeof UpdatePassword> | null>(null);

// 绑定数据接口
interface BindData {
  title: string; // 标题
  description: string; // 描述
  value: string; // 值
  bindType: BindType; // 类型
  bindStatus: number; // 绑定状态
  avatar?: string; // 头像
  color?: string; // 颜色
}

// 绑定类型
enum BindType {
  Password = "password",
  QQ = "qq",
  WeChat = "weChat",
  AliPay = "alipay",
  Gitee = "gitee"
}
// 获取绑定的情况
const data: BindData[] = [
  { title: "密码强度", description: "当前密码强度", value: "弱", bindType: BindType.Password, bindStatus: 0 },
  /*{ title: '密保手机', description: '已绑定手机', value: '138****8293', type: 'phone', bindStatus: 1 },
		{ title: '密保邮箱', description: '未绑定邮箱', value: '', type: 'email', bindStatus: 0 },
		{ title: '实名状态', description: '未实名', value: '', type: 'userReal', bindStatus: 0 },*/
  { avatar: "ant-design:qq-outlined", title: "绑定QQ", description: "未绑定", value: "", bindType: BindType.QQ, bindStatus: 0, color: "1890FF" },
  {
    avatar: "ant-design:wechat-outlined",
    title: "绑定微信",
    description: "未绑定",
    value: "",
    bindType: BindType.WeChat,
    bindStatus: 0,
    color: "1AAD19"
  },
  {
    avatar: "ant-design:alipay-circle-outlined",
    title: "绑定支付宝",
    description: "未绑定",
    value: "",
    bindType: BindType.AliPay,
    bindStatus: 0,
    color: "178bf5"
  },
  { avatar: "simple-icons:gitee", title: "绑定Gitee", description: "未绑定", value: "", bindType: BindType.Gitee, bindStatus: 0, color: "d84040" }
];

/** 
 * 绑定操作
 * @param bindType 绑定类型

 */
function bindAction(bindType: BindType) {
  // 这里填充处理绑定操作的函数
  if (bindType === BindType.Password) {
    updatePasswordRef.value?.onOpen();
  } else {
    ElMessage.info("开发中");
  }
}
</script>

<style lang="scss" scoped>
.list-container {
  padding: 0;
  margin: 0;
}
.list-item {
  display: flex;
  align-items: center;
  padding: 10px;
  border-bottom: 1px solid #f0f0f0;
}
.item-content {
  flex-grow: 1;
  color: #585252; /* 调整字体颜色为深灰色 */
}
.item-content h3 {
  margin: 0 0 10px; /* 添加一些间距 */
  font-size: 16px;
  font-weight: normal; /* 调整标题字体为正常粗细 */
  color: #242425; /* 标题字体颜色稍深一点 */
}
.item-content p {
  margin: 0; /* 移除段落默认的上下外边距 */
  font-size: 14px;
  color: #8f8f94; /* 标题字体颜色稍深一点 */
}
.item-action {
  margin-left: auto;
}
.item-action .el-button {
  font-weight: normal;
  color: #409eff; /* 为按钮文字设置主题颜色 */
}
.bind-icon {
  padding-right: 10px;
  padding-left: 10px;
  font-size: 40px;
}
</style>
