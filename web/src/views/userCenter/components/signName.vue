<!-- 
 * @Description: 电子签名
 * @Author: huguodong 
 * @Date: 2024-03-01 13:22:09
!-->
<template>
  <form-container v-model="visible" title="电子签名" form-size="1500px">
    <el-row :gutter="5">
      <el-col :span="15">
        <div style="border: 1px solid rgb(236 236 236)">
          <e-sign
            ref="esignRef"
            :width="800"
            :height="300"
            :image="props.image"
            :is-crop="options.isCrop"
            :line-width="options.lineWidth"
            :line-color="options.lineColor"
            v-model:bgColor="options.bgColor"
          />
        </div>
      </el-col>
      <el-col :span="9">
        <div style="width: auto; height: 90px">
          <img :src="resultImg" style="width: 100%; height: 90px; border: 1px solid rgb(236 236 236)" />
        </div>
      </el-col>
    </el-row>
    <div style="margin-top: 10px">
      <el-space>
        <el-form>
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="画笔粗细：">
                <el-input-number v-model="options.lineWidth" :min="1" :max="20" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-button type="primary" @click="handleGenerate">预览</el-button>
              <el-button @click="handleReset">清屏</el-button>
            </el-col>
          </el-row>
        </el-form>
      </el-space>
    </div>
    <template #footer>
      <el-button @click="visible = false"> 取消 </el-button>
      <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { ESignInstance } from "@/components/ESign/interface";
const visible = ref(false); //是否显示表单
const options = reactive({
  isCrop: false, //是否开启裁剪
  lineWidth: 6, //线条宽度
  lineColor: "#000000", //线条颜色
  bgColor: "" //背景颜色
});

const props = defineProps({
  image: {
    type: String,
    default: ""
  }
});
const emit = defineEmits(["successful"]);

const esignRef = ref<ESignInstance>(); //签名容器
const resultImg = ref(props.image); //签名结果

function open() {
  visible.value = true;
}

function handleGenerate() {
  if (esignRef.value) {
    //生成签名
    esignRef.value
      .generate(null)
      .then(res => {
        console.log("[ res  ] >", res);
        resultImg.value = res as string;
      })
      .catch(() => {
        ElMessage.error("无任何签名");
      });
  }
}

function handleReset() {
  //清空签名
  resultImg.value = "";
  esignRef.value?.reset();
}
function handleSubmit() {
  if (esignRef.value) {
    //生成签名
    esignRef.value
      .generate(null)
      .then(res => {
        console.log("[ res ] >", res);
        emit("successful", res);
        visible.value = false;
      })
      .catch(() => {
        ElMessage.error("无任何签名");
      });
  }
}

defineExpose({
  open
});
</script>

<style lang="scss" scoped></style>
