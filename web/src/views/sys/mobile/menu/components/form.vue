<!-- 
 * @Description: 菜单管理表单页面
 * @Author: superAdmin  
 * @Date: 2025-06-26 16:41:59
!-->

<template>
  <form-container v-model="visible" :title="`${menuProps.opt}菜单`" form-size="600px">
    <el-form
      ref="menuFormRef"
      :rules="rules"
      :disabled="menuProps.disabled"
      :model="menuProps.record"
      :hide-required-asterisk="menuProps.disabled"
      label-width="auto"
      label-suffix=" :"
    >
      <s-form-item label="菜单名称" prop="title">
        <s-input v-model="menuProps.record.title"></s-input>
      </s-form-item>

      <s-form-item label="菜单类型" prop="menuType">
        <s-radio-group v-model="menuProps.record.menuType" :options="menuTypeOptions" button />
      </s-form-item>
      <s-form-item label="上级菜单" prop="parentId">
        <MenuSelector
          v-model:menu-value="menuProps.record.parentId"
          show-all
          @change="changeMenu"
          :menu-tree-api="() => mobileMenuApi.menuTreeSelector({ module: module })"
        />
      </s-form-item>
      <s-form-item label="路径" prop="path" v-if="isMenu">
        <s-input v-model="menuProps.record.path"></s-input>
      </s-form-item>
      <s-form-item label="图标" prop="icon">
        <SelectIconPlus v-model:icon-value="menuProps.record.icon!" />
      </s-form-item>
      <s-form-item label="颜色" prop="color">
        <color-picker v-model:value="menuProps.record.color" />
      </s-form-item>
      <s-form-item label="排序码" prop="sortCode">
        <el-slider v-model="menuProps.record.sortCode" show-input :min="1" />
      </s-form-item>
      <s-form-item label="正规则" prop="regType">
        <s-radio-group v-model="menuProps.record.regType" :options="regTypeOptions" button />
      </s-form-item>
      <s-form-item label="状态" prop="status">
        <s-radio-group v-model="menuProps.record.status" :options="statusOptions" button />
      </s-form-item>
      <s-form-item label="描述" prop="description">
        <s-input v-model="menuProps.record.description"></s-input>
      </s-form-item>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!menuProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { mobileMenuApi, MobileModule } from "@/api";
import { FormOptEnum, MenuTypeDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore();
const menuTypeOptions = dictStore
  .getDictList("MENU_TYPE")
  .filter((item: { value: MenuTypeDictEnum }) => item.value == MenuTypeDictEnum.MENU || item.value == MenuTypeDictEnum.CATALOG); //菜单类型选项
const regTypeOptions = dictStore.getDictList("YES_NO"); //正规则选项
const statusOptions = dictStore.getDictList("COMMON_STATUS"); //状态选项
const isMenu = computed(() => menuProps.record.menuType === MenuTypeDictEnum.MENU); //是否是菜单
// 模块id
const module = ref<number | string>(0);

// 表单参数
const menuProps = reactive<FormProps.Base<MobileModule.MobileModuleInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入菜单名称")],
  parentId: [required("请选择上级菜单")],
  menuType: [required("请选择菜单类型")],
  path: [required("请输入路径")],
  regType: [required("请选择正规则")],
  color: [required("请选择颜色")],
  sortCode: [required("请输入排序")],
  icon: [required("请选择图标")],
  status: [required("请选择状态")]
});
/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<MobileModule.MobileModuleInfo>, moduleId: number | string) {
  Object.assign(menuProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    menuProps.record.menuType = menuTypeOptions[0].value;
    //如果是新增,设置默认值
    //如果是新增,设置默认值
    menuProps.record.sortCode = 99;
    menuProps.record.status = statusOptions[0].value;
    menuProps.record.menuType = MenuTypeDictEnum.MENU;
    menuProps.record.sortCode = 99;
    menuProps.record.module = moduleId;
    menuProps.record.regType = regTypeOptions[0].value;
    menuProps.record.color = "#1890ff"; //默认颜色
  }
  module.value = moduleId; //设置模块id
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    mobileMenuApi.detail({ id: props.record.id }).then(res => {
      menuProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const menuFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  menuFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await mobileMenuApi
      .submitForm(menuProps.record, menuProps.record.id != undefined)
      .then(() => {
        menuProps.successful!(); //调用父组件的successful方法
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

/** 菜单选择事件
 * @param path 路由地址
 */
function changeMenu(path: string) {
  menuProps.record.path = path;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>
