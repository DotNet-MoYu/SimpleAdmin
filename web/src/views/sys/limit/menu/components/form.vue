<!-- 菜单管理表菜单面 -->
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
      class="-mt-25px"
    >
      <!-- 基本设置 -->
      <el-divider content-position="left">基本设置</el-divider>
      <s-form-item label="菜单名称" prop="title">
        <s-input v-model="menuProps.record.title"></s-input>
      </s-form-item>
      <s-form-item label="菜单类型" prop="menuType">
        <s-radio-group v-model="menuProps.record.menuType" :options="menuTypeOptions" button />
      </s-form-item>
      <s-form-item label="图标" prop="icon">
        <SelectIconPlus v-model:icon-value="menuProps.record.icon" />
      </s-form-item>
      <s-form-item label="上级菜单" prop="parentId">
        <MenuSelector v-model:menu-value="menuProps.record.parentId" :module="module" @change="changeMenu" />
      </s-form-item>
      <s-form-item label="状态" prop="status">
        <s-radio-group v-model="menuProps.record.status" :options="statusOptions" button />
      </s-form-item>
      <s-form-item v-if="isCatalog || isMenu || isSubSet" label="路由地址" prop="path">
        <s-input v-model="menuProps.record.path" placeholder="请填写路由地址,例:/home/index"></s-input>
      </s-form-item>
      <div v-if="isMenu || isSubSet">
        <s-form-item label="组件名称" prop="name">
          <s-input v-model="menuProps.record.name">
            <template #prepend>setup name=</template>
          </s-input>
        </s-form-item>
        <s-form-item label="组件地址" prop="component">
          <s-input v-model="menuProps.record.component">
            <template #prepend>src/views/</template>
          </s-input>
        </s-form-item>
      </div>
      <div v-if="isLink">
        <s-form-item label="链接地址:" prop="path">
          <s-input v-model="menuProps.record.path" placeholder="请填写链接地址,例:http://www.baidu.com"></s-input>
        </s-form-item>
      </div>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="menuProps.record.sortCode" show-input :min="1" />
      </s-form-item>
      <s-form-item label="说明" prop="description">
        <s-input v-model="menuProps.record.description"></s-input>
      </s-form-item>
      <!-- 功能设置 -->
      <div v-if="isMenu || isLink">
        <el-divider content-position="left">功能设置</el-divider>
        <el-row :gutter="24">
          <el-col :span="12">
            <s-form-item label="设置主页" prop="isHome">
              <s-radio-group v-model="menuProps.record.isHome" :disabled="isLink || isSubSet" :options="yesOptions" button />
            </s-form-item>
          </el-col>
          <el-col :span="12">
            <s-form-item label="隐藏菜单" prop="isHide">
              <s-radio-group v-model="menuProps.record.isHide" :options="yesOptions" button />
            </s-form-item>
          </el-col>
          <el-col :span="12">
            <s-form-item label="页面全屏" prop="isFull">
              <s-radio-group v-model="menuProps.record.isFull" :disabled="isLink" :options="yesOptions" button />
            </s-form-item>
          </el-col>
          <el-col :span="12">
            <s-form-item label="固定标签页" prop="isAffix">
              <s-radio-group v-model="menuProps.record.isAffix" :disabled="isLink" :options="yesOptions" button />
            </s-form-item>
          </el-col>
          <el-col :span="12">
            <s-form-item label="路由缓存" prop="isKeepAlive">
              <s-radio-group v-model="menuProps.record.isKeepAlive" :disabled="isLink" :options="yesOptions" button />
            </s-form-item>
          </el-col>
        </el-row>
      </div>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!menuProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { menuApi, Menu } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum, SysDictEnum, MenuTypeDictEnum } from "@/enums";
import { FormInstance } from "element-plus/es/components/form";
import { useDictStore } from "@/stores/modules";
const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库
// 菜单类型选项
const menuTypeOptions = dictStore.getDictList(SysDictEnum.MENU_TYPE).filter(item => item.value !== MenuTypeDictEnum.BUTTON);
// 是否选项
const yesOptions = dictStore.getDictList(SysDictEnum.YES_NO);

// 通用状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);
// 模块id
const module = ref<number | string>(0);

// 表单参数
const menuProps = reactive<FormProps.Base<Menu.MenuInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入菜单名称")],
  parentId: [required("请选择上级菜单")],
  menuType: [required("请选择菜单类型")],
  path: [required("请输入路由地址")],
  name: [required("请输入组件名称")],
  component: [required("请输入组件地址")],
  sortCode: [required("请输入排序")],
  icon: [required("请选择图标")],
  status: [required("请选择状态")]
});

const isCatalog = computed(() => menuProps.record.menuType === MenuTypeDictEnum.CATALOG); //是否是目录
const isMenu = computed(() => menuProps.record.menuType === MenuTypeDictEnum.MENU); //是否是菜单
const isLink = computed(() => menuProps.record.menuType === MenuTypeDictEnum.LINK); //是否
const isSubSet = computed(() => menuProps.record.menuType === MenuTypeDictEnum.SUBSET); //是否是子集

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<Menu.MenuInfo>, moduleId: number | string) {
  Object.assign(menuProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    menuProps.record.sortCode = 99;
    menuProps.record.status = statusOptions[0].value;
    menuProps.record.menuType = MenuTypeDictEnum.MENU;
    menuProps.record.sortCode = 99;
    menuProps.record.isHome = false;
    menuProps.record.isHide = false;
    menuProps.record.isFull = false;
    menuProps.record.isAffix = false;
    menuProps.record.isKeepAlive = true;
    menuProps.record.module = moduleId;
  }
  module.value = moduleId; //设置模块id
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    menuApi.menuDetail({ id: props.record.id }).then(res => {
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
    await menuApi
      .menuSubmitForm(menuProps.record, menuProps.record.id != undefined)
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
  if (isSubSet.value) {
    path = path + "/:id";
    menuProps.record.activeMenu = path;
  }
  menuProps.record.path = path;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
