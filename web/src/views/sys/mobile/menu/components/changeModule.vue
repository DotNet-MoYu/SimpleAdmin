<!-- 
 * @Description: 更改菜单模块
 * @Author: huguodong 
 * @Date: 2025-07-02 13:52:09
!-->
<template>
  <form-container v-model="visible" :title="'更改模块->' + changeModuleProps.title" form-size="450px">
    <el-form ref="changeModuleFormRef" :rules="rules" :model="changeModuleProps" class="mt-20px">
      <s-form-item label="所属模块" prop="module">
        <s-select v-model="changeModuleProps.module" :options="changeModuleProps.moduleOptions"></s-select>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { mobileMenuApi } from "@/api";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus/es/components/form";

const visible = ref(false); //是否显示表单
/** 表单参数 */
interface ChangeModuleProps {
  /** 菜单id */
  id: number | string;
  /** 模块id */
  module: number | string;
  /** 菜单名称 */
  title: string;
  /** 模块列表 */
  moduleOptions: { label: string; value: number | string }[];
  /** 表单实例 */
  successful?: () => void;
}
/** 表单参数 */
const changeModuleProps = reactive<ChangeModuleProps>({
  id: 0,
  module: 0,
  title: "",
  moduleOptions: []
});

// 表单验证规则
const rules = reactive({
  module: [required("请选择所属模块")]
});

// 提交数据（新增/编辑）
const changeModuleFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  changeModuleFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await mobileMenuApi
      .changeModule({
        id: changeModuleProps.id,
        module: changeModuleProps.module
      })
      .then(() => {
        //提交成功
        changeModuleProps.successful?.();
      })
      .finally(() => {
        onClose();
      });
  });
}

/** 打开表单
 * @param title 菜单名称
 * @param module 模块id
 */
function onOpen(moduleProps: ChangeModuleProps) {
  Object.assign(changeModuleProps, moduleProps); //合并参数
  visible.value = true; //显示表单
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
