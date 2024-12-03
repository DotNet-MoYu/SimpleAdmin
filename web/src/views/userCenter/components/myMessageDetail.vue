<!-- 
 * @Description: 站内信详情
 * @Author: huguodong 
 * @Date: 2024-11-11 09:22:49
!-->
<template>
  <form-container v-model="visible" title="消息详情" form-size="600px">
    <h4 class="title">基本信息</h4>
    <el-descriptions :column="1" border>
      <el-descriptions-item label="主题" label-align="left">
        {{ messageProps.record.subject }}
      </el-descriptions-item>
      <el-descriptions-item label="发送时间" label-align="left">
        {{ messageProps.record.sendTime }}
      </el-descriptions-item>
    </el-descriptions>
    <h4 class="title">正文:</h4>
    <div v-html="messageProps.record.content"></div>

    <template #footer>
      <el-button type="primary" @click="onClose"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { SysMessage, userCenterApi } from "@/api";
import { FormOptEnum } from "@/enums";
import { useMessageStore } from "@/stores/modules";

const messageStore = useMessageStore();
const visible = ref(false); //是否显示表单

// 表单参数
const messageProps = reactive<FormProps.Base<SysMessage.SysMessageInfo>>({
  opt: FormOptEnum.VIEW,
  record: {},
  disabled: false
});

/** 关闭表单*/
function onClose() {
  visible.value = false;
}

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysMessage.SysMessageInfo>) {
  Object.assign(messageProps, props); //合并参数
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    userCenterApi.myMessageDetail({ id: props.record.id }).then(res => {
      messageProps.record = res.data;
      messageStore.unReadCountSubtract(1); //未读数量减1
    });
  }
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
