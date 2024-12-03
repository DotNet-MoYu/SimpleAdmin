<!-- 
 * @Description: 站内信详情
 * @Author: huguodong 
 * @Date: 2024-11-11 09:22:49
!-->
<template>
  <form-container v-model="visible" title="站内信详情" form-size="700px">
    <h4 class="title">基本信息</h4>
    <el-descriptions :column="2" border>
      <el-descriptions-item label="主题" label-align="left">
        {{ messageProps.record.subject }}
      </el-descriptions-item>
      <el-descriptions-item label="消息类型" label-align="left">
        {{ dictStore.dictTranslation(SysDictEnum.MESSAGE_CATEGORY, messageProps.record.category!) }}
      </el-descriptions-item>
      <el-descriptions-item label="发送方式" label-align="left">
        {{ dictStore.dictTranslation(SysDictEnum.MESSAGE_WAY, messageProps.record.sendWay!) }}
      </el-descriptions-item>
      <el-descriptions-item label="创建时间" label-align="left">
        {{ messageProps.record.createTime }}
      </el-descriptions-item>
      <el-descriptions-item label="发送时间" label-align="left">
        {{ messageProps.record.sendTime }}
      </el-descriptions-item>
      <el-descriptions-item label="状态" label-align="left">
        {{ dictStore.dictTranslation(SysDictEnum.MESSAGE_STATUS, messageProps.record.status!) }}
      </el-descriptions-item>
      <!-- <el-descriptions-item label="正文" class-name="my-class" label-align="left" label-class-name="my-label">
        {{ messageProps.record.content }}
      </el-descriptions-item> -->
    </el-descriptions>
    <h4 class="title">正文:</h4>
    <div v-html="messageProps.record.content"></div>
    <h4 class="title">接收详情:</h4>
    <ProTable
      v-if="getReadData"
      ref="proTable"
      title="接收列表"
      class="table"
      :columns="columns"
      :pagination="true"
      :data="messageProps.record.receiverDetail"
      :tool-button="false"
    >
      <!-- 表格 类型 -->
      <template #read="scope">
        <el-space wrap>
          <el-tag v-if="scope.row.read === true" type="info">是</el-tag>
          <el-tag v-else-if="scope.row.read === false" type="danger">否</el-tag>
        </el-space>
      </template>
    </ProTable>
    <template #footer>
      <el-button type="primary" @click="onClose"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { SysMessage, messageApi } from "@/api";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { useDictStore } from "@/stores/modules";
import { ColumnProps, ProTableInstance } from "@/components/ProTable/interface";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore();
// 获取 ProTable 元素，调用其获取刷新数据方法（还能获取到当前查询参数，方便导出携带参数）
const proTable = ref<ProTableInstance>();
// 是否
const yesNoOptions = dictStore.getDictList(SysDictEnum.YES_NO, true);

const getReadData = ref<boolean>(false); //是否获取了接收详情数据

// 表格配置项
const columns: ColumnProps<any>[] = [
  { prop: "name", label: "姓名" },
  { prop: "read", label: "是否已读", enum: yesNoOptions }
];

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
  console.log("[ messageProps.record.id ] >", messageProps.record.id);
  Object.assign(messageProps, props); //合并参数
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    messageApi.detail({ id: props.record.id, showReceiveInfo: true }).then(res => {
      messageProps.record = res.data;
      getReadData.value = true;
    });
  }
}

/**
 * 获取按钮分页数据
 * 如果你想在请求之前对当前请求参数做一些操作，可以自定义如下函数：params 为当前所有的请求参数（包括分页），最后返回请求列表接口
 * 默认不做操作就直接在 ProTable 组件上绑定	:requestApi="getUserList"
 */
// function getPage(params: any) {
//   let newParams = JSON.parse(JSON.stringify(params)); //转换成json字符串再转换成json对象
//   newParams.id = messageProps.record.id; //按钮父Id
//   console.log("[ initParam ] >", initParam.value);
//   return messageApi.readDetail(newParams);
// }

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
