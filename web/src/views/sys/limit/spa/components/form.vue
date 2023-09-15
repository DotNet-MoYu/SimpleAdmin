<!-- 单页管理表单页面 -->
<template>
  <form-container v-model="visible" :title="`${spaProps.opt}单页`" form-size="600px">
    <el-form
      ref="spaFormRef"
      :rules="rules"
      :disabled="spaProps.disabled"
      :model="spaProps.record"
      :hide-required-asterisk="spaProps.disabled"
      label-width="auto"
      class="-mt-25px"
    >
      <!-- 基本设置 -->
      <el-divider content-position="left">基本设置</el-divider>
      <s-form-item label="单页名称" prop="title">
        <el-input v-model="spaProps.record.title" placeholder="请填写单页名称" clearable></el-input>
      </s-form-item>
      <s-form-item label="单页类型" prop="menuType">
        <el-radio-group v-model="spaProps.record.menuType">
          <el-radio-button v-for="(item, index) in spaTypeOptions" :key="index" :label="item.value">{{
            item.label
          }}</el-radio-button>
        </el-radio-group>
      </s-form-item>
      <s-form-item label="图标" prop="icon">
        <SelectIconPlus v-model:icon-value="spaProps.record.icon" />
      </s-form-item>

      <div v-if="spaProps.record.menuType === MenuTypeDictEnum.MENU">
        <s-form-item label="路由地址" prop="path">
          <el-input v-model="spaProps.record.path" placeholder="请填写路由地址,例:/home/index" clearable></el-input>
        </s-form-item>
        <s-form-item label="组件名称" prop="name">
          <el-input v-model="spaProps.record.name" placeholder="请填写组件名称" clearable>
            <template #prepend>setup name=</template>
          </el-input>
        </s-form-item>
        <s-form-item label="组件地址" prop="component">
          <el-input v-model="spaProps.record.component" placeholder="请填写组件地址" clearable>
            <template #prepend>src/views/</template>
          </el-input>
        </s-form-item>
      </div>
      <div v-else>
        <s-form-item label="链接地址:" prop="path">
          <el-input v-model="spaProps.record.path" placeholder="请填写链接地址,例:http://www.baidu.com" clearable></el-input>
        </s-form-item>
      </div>
      <s-form-item label="排序" prop="sortCode">
        <el-slider v-model="spaProps.record.sortCode" show-input :min="1" />
      </s-form-item>
      <s-form-item label="说明" prop="description">
        <el-input v-model="spaProps.record.description" placeholder="请填写单页说明" clearable></el-input>
      </s-form-item>
      <!-- 功能设置 -->
      <el-divider content-position="left">功能设置</el-divider>
      <el-row :gutter="24">
        <el-col :span="12">
          <s-form-item label="设置主页" prop="isHome">
            <el-radio-group v-model="spaProps.record.isHome">
              <el-radio-button v-for="(item, index) in yesOptions" :key="index" :label="item.value">{{
                item.label
              }}</el-radio-button>
            </el-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="隐藏页面" prop="isHide">
            <el-radio-group v-model="spaProps.record.isHide">
              <el-radio-button v-for="(item, index) in yesOptions" :key="index" :label="item.value">{{
                item.label
              }}</el-radio-button>
            </el-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="页面全屏" prop="isFull">
            <el-radio-group v-model="spaProps.record.isFull">
              <el-radio-button v-for="(item, index) in yesOptions" :key="index" :label="item.value">{{
                item.label
              }}</el-radio-button>
            </el-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="固定标签页" prop="isAffix">
            <el-radio-group v-model="spaProps.record.isAffix">
              <el-radio-button v-for="(item, index) in yesOptions" :key="index" :label="item.value">{{
                item.label
              }}</el-radio-button>
            </el-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="12">
          <s-form-item label="路由缓存" prop="isKeepAlive">
            <el-radio-group v-model="spaProps.record.isKeepAlive">
              <el-radio-button v-for="(item, index) in yesOptions" :key="index" :label="item.value">{{
                item.label
              }}</el-radio-button>
            </el-radio-group>
          </s-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button v-show="!spaProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { Spa, spaDetailApi, spaSubmitFormApi } from "@/api";
import { required } from "@/utils/formRules";
import { FormOptEnum, SysDictEnum, MenuTypeDictEnum } from "@/enums";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库

// 单页类型选项
const spaTypeOptions = dictStore
  .getDictList(SysDictEnum.MENU_TYPE)
  .filter(item => item.value == MenuTypeDictEnum.MENU || item.value == MenuTypeDictEnum.LINK);
// 是否选项
const yesOptions = dictStore.getDictList(SysDictEnum.YES_NO);

// 表单参数
const spaProps = reactive<FormProps.Base<Spa.SpaInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  title: [required("请输入单页名称")],
  menuType: [required("请选择单页类型")],
  path: [required("请输入路由地址")],
  name: [required("请输入组件名称")],
  component: [required("请输入组件地址")],
  sortCode: [required("请输入排序")],
  icon: [required("请选择图标")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<Spa.SpaInfo>) {
  Object.assign(spaProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    spaProps.record.menuType = MenuTypeDictEnum.MENU;
    spaProps.record.sortCode = 99;
    spaProps.record.isHome = false;
    spaProps.record.isHide = false;
    spaProps.record.isFull = false;
    spaProps.record.isAffix = false;
    spaProps.record.isKeepAlive = true;
  }

  visible.value = true; //显示表单
  if (props.id) {
    //如果传了id，就去请求api获取record
    spaDetailApi({ id: props.id }).then(res => {
      spaProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const spaFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  spaFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await spaSubmitFormApi(spaProps.record, spaProps.record.id != undefined)
      .then(() => {
        spaProps.successful!(); //调用父组件的successful方法
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
<style lang="scss" scoped>
:deep(.el-input-group__prepend) {
  padding: 0 10px !important;
}
</style>
