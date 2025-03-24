<!-- 
 * @Description: 代码生成步骤
 * @Author: huguodong 
 * @Date: 2025-01-08 15:00:51
!-->
<template>
  <div>
    <el-card class="steps-card" :bordered="false">
      <el-row class="xn-row">
        <el-col :span="6"></el-col>
        <el-col :span="12">
          <el-steps align-center finish-status="success" :active="active">
            <el-step v-for="item in steps" :key="item.title" :title="item.title" />
          </el-steps>
        </el-col>
        <el-col :span="6">
          <div style="float: right">
            <el-button :disabled="active === 0" style="margin-right: 8px" @click="prev"> 上一步 </el-button>
            <el-button :disabled="active === 2" type="primary" style="margin-left: 8px" @click="next"> 继续 </el-button>
            <el-button type="primary" danger ghost style="margin-left: 8px" @click="$emit('closed')"> 关闭 </el-button>
          </div>
        </el-col>
      </el-row>
    </el-card>
    <div v-if="active === 0">
      <Basic ref="basicRef" />
    </div>
    <div v-if="active === 1">
      <Config ref="configRef" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { GenCode } from "@/api";
import { FormOptEnum } from "@/enums";
import Basic from "./basic.vue";
import Config from "./config.vue";

const active = ref(0);
const emit = defineEmits(["closed"]);
const recordData = ref({}); //记录数据
// 分布步骤数据
const steps = [
  {
    title: "基础信息",
    content: "基础信息"
  },
  {
    title: "详细配置",
    content: "详细配置"
  }
];

// 表单参数
reactive<FormProps.Base<GenCode.GenBasic>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 获取组件实例
const basicRef = ref<InstanceType<typeof Basic> | null>(null);
const configRef = ref<InstanceType<typeof Config> | null>(null);

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<GenCode.GenBasic>) {
  // console.log("record", props.record);
  // Object.assign(genBasicProps, props); //合并参数
  basicRef.value?.onOpen(props.record as GenCode.GenBasic);
}

/**下一步**/
function next() {
  // active.value++;
  console.log("active.value", active.value);
  // 判断是哪一步
  if (active.value === 0) {
    basicRef.value?.onSubmit().then(data => {
      recordData.value = data;
      active.value++;
      nextTick(() => {
        configRef.value?.onOpen(data.id);
      });
    });
    // active.value--;
  }
  if (active.value === 1) {
    configRef.value?.onSubmit().then(() => {
      active.value++;
      nextTick(() => {
        // emit("closed");
        //关闭页面
        emit("closed");
      });
    });
  }
}

/**上一步**/
function prev() {
  active.value--;
  if (active.value === 0) {
    nextTick(() => {
      basicRef.value!.onOpen(recordData.value as GenCode.GenBasic);
    });
  }
  if (active.value === 1) {
    nextTick(() => {
      // configRef.value.onOpen(recordData.value);
    });
  }
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped>
.steps-card {
  padding-top: -10px;
  margin: -12px -12px 10px;
}
.gen-row {
  margin-top: -10px;
  margin-bottom: -10px;
}
</style>
