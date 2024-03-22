<!-- 
 * @Description: 图片裁剪组件
 * @Author: huguodong 
 * @Date: 2024-03-04 15:54:46
!-->
<template>
  <el-dialog ref="cropRef" v-model="visible" :width="700" title="图片裁剪" @cancel="handleClear" @ok="cropOk">
    <el-row :gutter="10">
      <!-- 裁剪区 -->
      <el-col :span="17">
        <VueCropper
          ref="cropper"
          class="cropper"
          :img="options.img"
          :output-size="options.outputSize"
          :output-type="options.outputType"
          :info="true"
          :full="options.full"
          :can-move="options.canMove"
          :can-move-box="options.canMoveBox"
          :fixed-box="options.fixedBox"
          :original="options.original"
          :auto-crop="options.autoCrop"
          :auto-crop-width="options.autoCropWidth"
          :auto-crop-height="options.autoCropHeight"
          :center-box="options.centerBox"
          :high="options.high"
          :info-true="options.infoTrue"
          :max-img-size="options.maxImgSize"
          @real-time="getCropData"
        />
      </el-col>
      <el-col :span="7">
        <div style="width: 165px; height: 165px; border: 1px solid #e9e9e9; border-radius: 2px">
          <el-image :src="cropPros.previewUrl" />
        </div>
        <div style="display: flex; padding-top: 10px">
          <div style="width: 100px; height: 100px; border: 1px solid #e9e9e9; border-radius: 2px">
            <el-image :src="cropPros.previewUrl" />
          </div>
          <div style="width: 60px; height: 60px; margin-left: 5px; border: 1px solid #e9e9e9; border-radius: 2px">
            <el-image :src="cropPros.previewUrl" />
          </div>
        </div>
      </el-col>
    </el-row>
    <div style="padding-top: 10px; text-align: center">
      <el-space>
        <el-button @click="cropper.changeScale(1)">放大</el-button>
        <el-button @click="cropper.changeScale(-1)">缩小</el-button>
        <el-button @click="cropper.rotateLeft()">向左旋转</el-button>
        <el-button @click="cropper.rotateRight()">向右旋转</el-button>
      </el-space>
      <div style="padding-top: 10px">
        <el-upload
          name="file"
          :auto-upload="false"
          :show-upload-list="false"
          :custom-request="() => {}"
          accept="image/png, image/jpeg, image/gif, image/jpg"
          :on-change="uploadChange"
        >
          <el-button type="primary">
            <Upload />
            点击上传图片
          </el-button>
        </el-upload>
      </div>
      <div style="padding-top: 10px">请上传图片文件，建议不超过2M</div>
    </div>
    <template #footer>
      <div>
        <el-button @click="handleClear">取消</el-button>
        <el-button type="primary" @click="cropOk"> 确认 </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts" name="CropUpload">
import "vue-cropper/dist/index.css";
import { VueCropper } from "vue-cropper";

import { VueCropperProps, OutputType, CropUploadOptions } from "./interface";
import { UploadFile, UploadRawFile } from "element-plus";

const cropper = ref(); // 裁剪组件实例

const cropPros = reactive<CropUploadOptions>({
  previewUrl: "",
  fileName: ""
});

const props = defineProps({
  imgSrc: {
    type: String,
    default: () => ""
  },
  circle: {
    type: Boolean,
    default: () => false
  }
});
const emit = defineEmits({ successful: null });
const visible = ref(false); // 是否显示裁剪框

// 初始化参数
const options = reactive<VueCropperProps>({
  img: "",
  outputSize: 1,
  full: false,
  outputType: OutputType.png,
  canMove: true,
  fixedBox: props.circle,
  original: false,
  canMoveBox: true,
  autoCrop: true,
  autoCropWidth: 200,
  autoCropHeight: 200,
  centerBox: false,
  high: false,
  cropData: {},
  enlarge: 1,
  mode: "contain",
  maxImgSize: 3000,
  limitMinSize: [100, 120],
  infoTrue: false
});

function getBase64(img: UploadRawFile, callback: (base64String: string | ArrayBuffer | null) => void) {
  const reader = new FileReader();
  reader.addEventListener("load", () => callback(reader.result));
  reader.readAsDataURL(img);
}

function uploadChange(file: UploadFile) {
  if (file.raw) {
    getBase64(file.raw, (imageUrl: string | ArrayBuffer | null) => {
      cropPros.fileName = file.name;
      options.img = imageUrl as string;
    });
  }
}

function show() {
  if (props.imgSrc) {
    options.img = props.imgSrc;
    // cropPros.fileName = ''
  }
  visible.value = true;
}

function getCropData() {
  cropper.value.getCropData((data: string) => {
    cropPros.previewUrl = data;
  });
}

function cropOk() {
  cropper.value.getCropBlob((blobData: any) => {
    emit("successful", { fileName: cropPros.fileName, blobData: blobData });
    handleClear();
  });
}
function handleClear() {
  visible.value = false;
  options.img = cropPros.previewUrl = cropPros.fileName = "";
}
defineExpose({
  show
});
</script>

<style lang="scss" scoped>
.circle {
  border-radius: 50%;
}
.cropper {
  height: 280px;
}
</style>
