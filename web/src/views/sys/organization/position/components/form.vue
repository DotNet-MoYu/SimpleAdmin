<!-- 
 * @Description: 表单
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:37
!-->
<template>
  <div>
    <form-container v-model="visible" :title="`${positionProps.opt}职位`" form-size="600px">
      <el-form
        ref="positionFormRef"
        :rules="rules"
        :disabled="positionProps.disabled"
        :model="positionProps.record"
        :hide-required-asterisk="positionProps.disabled"
        label-width="auto"
        label-suffix=" :"
      >
        <s-form-item label="所属组织" prop="orgId">
          <org-selector v-model:org-value="positionProps.record.orgId!" :show-all="false" />
        </s-form-item>
        <s-form-item label="职位名称" prop="name">
          <s-input v-model="positionProps.record.name"></s-input>
        </s-form-item>
        <s-form-item label="职位编码" prop="code">
          <s-input v-model="positionProps.record.code" placeholder="请填写职位编码,不填则为随机值"></s-input>
        </s-form-item>
        <s-form-item label="职位分类" prop="category">
          <s-radio-group v-model="positionProps.record.category" :options="posCategoryOptions" button />
        </s-form-item>
        <s-form-item label="状态" prop="status">
          <s-radio-group v-model="positionProps.record.status" :options="statusOptions" button />
        </s-form-item>
        <s-form-item label="排序" prop="sortCode">
          <el-slider v-model="positionProps.record.sortCode" show-input :min="1" />
        </s-form-item>
      </el-form>
      <template #footer>
        <el-button @click="onClose"> 取消 </el-button>
        <el-button v-show="!positionProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
      </template>
    </form-container>
  </div>
</template>

<script setup lang="ts">
import { SysPosition, sysPositionApi } from "@/api";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库
// 职位类型选项
const posCategoryOptions = dictStore.getDictList(SysDictEnum.POSITION_CATEGORY);
// 通用状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);

// 表单参数
const positionProps = reactive<FormProps.Base<SysPosition.SysPositionInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  parentId: [required("请选择所属组织")],
  name: [required("请输入职位名称")],
  orgId: [required("请选择所属职位")],
  category: [required("请选择职位类型")],
  status: [required("请选择状态")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysPosition.SysPositionInfo>) {
  Object.assign(positionProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    positionProps.record.sortCode = 99;
    positionProps.record.category = posCategoryOptions[0].value;
    positionProps.record.status = statusOptions[0].value;
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    sysPositionApi.detail({ id: props.record.id }).then(res => {
      positionProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const positionFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  positionFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysPositionApi
      .submitForm(positionProps.record, positionProps.record.id != undefined)
      .then(() => {
        positionProps.successful!(); //调用父组件的successful方法
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

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
