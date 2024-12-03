<!-- 
 * @Description: 单页管理表单页面
 * @Author: huguodong 
 * @Date: 2023-12-15 15:43:59
!-->
<template>
  <form-container v-model="visible" :title="`${messageProps.opt}站内信`" form-size="700px">
    <el-form
      ref="messageFormRef"
      :rules="rules"
      :disabled="messageProps.disabled"
      :model="messageProps.record"
      :hide-required-asterisk="messageProps.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="消息主题" prop="subject">
        <s-input v-model="messageProps.record.subject"></s-input>
      </s-form-item>
      <s-form-item label="消息类型" prop="category">
        <s-select v-model="messageProps.record.category" :options="messageTypeOptions" />
      </s-form-item>
      <s-form-item label="正文内容" prop="content">
        <s-input v-model="messageProps.record.content" type="textarea" :autosize="{ minRows: 4, maxRows: 8 }" />
      </s-form-item>
      <s-form-item label="发送方式" prop="sendWay">
        <s-radio-group v-model="messageProps.record.sendWay" :options="sendWayOptions" button @change="handleSendWayChange" />
      </s-form-item>
      <s-form-item v-if="messageProps.record.sendWay === MessageSendWayDictEnum.DELAY" label="延迟时间" prop="delayTime">
        <el-input v-model="messageProps.record.delayTime">
          <template #append>秒</template>
        </el-input>
      </s-form-item>
      <s-form-item v-if="messageProps.record.sendWay === MessageSendWayDictEnum.SCHEDULE" label="指定时间" prop="sendTime">
        <date-picker v-model="messageProps.record.sendTime" :show-shortcuts="false" :disabled-date-before="true" type="datetime" />
      </s-form-item>
      <s-form-item label="接收人" prop="receiverType">
        <!-- 全部和指定 -->
        <s-radio-group v-model="messageProps.record.receiverType" :options="receiverTypeOptions" button @change="handleReceiverTypeChange" />
      </s-form-item>
      <s-form-item v-if="messageProps.record.receiverType !== MessageReceiverTypeDictEnum.ALL" :label="receiverType" prop="receiverInfo">
        <el-button link type="primary" @click="showSelector">选择</el-button>
        <el-tag v-for="item in messageProps.record.receiverInfo" :key="item.id" class="ml-3px" closable @close="removeTag(item)">{{
          item.name
        }}</el-tag>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!messageProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
  <!-- 用户选择器 -->
  <user-selector
    ref="userSelectorRef"
    multiple
    :org-tree-api="sysOrgApi.tree"
    :position-tree-api="sysPositionApi.tree"
    :role-tree-api="sysRoleApi.tree"
    :user-selector-api="sysUserApi.selector"
    @successful="handleChooseUser"
  />
  <!-- 角色选择器 -->
  <role-selector
    ref="roleSelectorRef"
    multiple
    :org-tree-api="sysOrgApi.tree"
    :role-selector-api="sysRoleApi.roleSelector"
    @successful="handleChooseRole"
  ></role-selector>
</template>

<script setup lang="ts">
import { SysMessage, messageApi, sysOrgApi, sysPositionApi, sysRoleApi, sysUserApi, SysUser, SysRole } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum, MessageTypeDictEnum, SysDictEnum, MessageReceiverTypeDictEnum, MessageSendWayDictEnum } from "@/enums";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
import { UserSelectorInstance } from "@/components/Selectors/UserSelector/interface";
import { RoleSelectorInstance } from "@/components/Selectors/RoleSelector/interface";

const dictStore = useDictStore();
const visible = ref(false); //是否显示表单
// 消息类型选项
const messageTypeOptions = dictStore.getDictList(SysDictEnum.MESSAGE_CATEGORY);

// 消息接受者类型选项
const receiverTypeOptions = dictStore.getDictList(SysDictEnum.RECEIVER_TYPE);

// 发送方式选项
const sendWayOptions = dictStore.getDictList(SysDictEnum.MESSAGE_WAY);

//根据接收人类型,计算出显示角色还是用户
const receiverType = computed(() => {
  return messageProps.record.receiverType == MessageReceiverTypeDictEnum.ROLE ? "角色" : "用户";
});

// 表单参数
const messageProps = reactive<FormProps.Base<SysMessage.SysMessageInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  subject: [required("请输入消息主题")],
  category: [required("请输入消息类型")],
  content: [required("请输入正文内容")],
  receiverType: [required("请选择接收人")],
  sendWay: [required("请选择发送方式")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysMessage.SysMessageInfo>) {
  Object.assign(messageProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    messageProps.record.category = MessageTypeDictEnum.INFORM;
    messageProps.record.receiverType = MessageReceiverTypeDictEnum.ALL;
    messageProps.record.receiverInfo = [] as SysUser.SysUserInfo[] | SysRole.SysRoleInfo[];
    messageProps.record.sendWay = MessageSendWayDictEnum.NOW;
    messageProps.record.read = false;
  }

  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    messageApi.detail({ id: props.record.id }).then(res => {
      messageProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const messageFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  messageFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //如果接收人类型是指定，但是没有选择接收人，提示用户选择接收人
    if (messageProps.record.receiverType != MessageReceiverTypeDictEnum.ALL && messageProps.record.receiverInfo!.length == 0) {
      ElMessage("请选择接收人");
      return;
    }
    //如果发送方式是延迟发送，但是没有填写延迟时间，提示用户填写延迟时间
    if (messageProps.record.sendWay == MessageSendWayDictEnum.DELAY && messageProps.record.delayTime == null) {
      ElMessage("请填写延迟时间");
      return;
    }
    //如果发送方式是定时发送，但是没有填写发送时间，提示用户填写发送时间
    if (messageProps.record.sendWay == MessageSendWayDictEnum.SCHEDULE && messageProps.record.sendTime == null) {
      ElMessage("请填写发送时间");
      return;
    }

    //提交表单
    await messageApi
      .submitForm(messageProps.record, messageProps.record.id != undefined)
      .then(() => {
        messageProps.successful!(); //调用父组件的successful方法
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

const userSelectorRef = ref<UserSelectorInstance>(); //用户选择器引用
const roleSelectorRef = ref<RoleSelectorInstance>(); //角色选择器引用

/** 显示用户选择器 */
function showSelector() {
  //根据是指定角色还是用户，显示不同的选择器
  if (messageProps.record.receiverType == MessageReceiverTypeDictEnum.ROLE) {
    roleSelectorRef.value?.showSelector();
  } else if (messageProps.record.receiverType == MessageReceiverTypeDictEnum.APPOINT) {
    userSelectorRef.value?.showSelector();
  }
}

/** 选择用户 */
function handleChooseUser(data: SysUser.SysUserInfo[]) {
  // 选择用户后，将用户id赋值给orgProps.record.directorId
  if (data.length > 0) {
    console.log("[ data ] >", data);
    //赋值messageProps.record.receiverInfo
    messageProps.record.receiverInfo = data;
  }
}

/** 选择角色 */
function handleChooseRole(data: SysRole.SysRoleInfo[]) {
  // 选择角色后，将角色id赋值给orgProps.record.directorId
  if (data.length > 0) {
    messageProps.record.receiverInfo = data;
  }
}

/** 移除选择的角色或者用户 */
function removeTag(data: SysUser.SysUserInfo | SysRole.SysRoleInfo) {
  //移除messageProps.record.receiverInfo中对应Id的数据
  if (messageProps.record.receiverInfo) {
    const newData = messageProps.record.receiverInfo!.filter(item => item.id != data.id) as SysUser.SysUserInfo[] | SysRole.SysRoleInfo[];
    messageProps.record.receiverInfo = newData;
  }
}

/** 改变接收人类型 */
function handleReceiverTypeChange() {
  //接收人类型改变时，清空接收人信息
  messageProps.record.receiverInfo = [];
}

/** 改变发送方式 */
function handleSendWayChange(value: any) {
  //发送方式改变时，清空发送方式信息
  // messageProps.record.sendType = [];
  messageProps.record.sendTime = undefined;
  messageProps.record.delayTime = undefined;
  messageProps.record.sendWay = value;
  return;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>
<style lang="scss" scoped>
:deep(.s-input-group__prepend) {
  padding: 0 10px !important;
}
</style>
