<!-- 
 * @Description: 表单
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:28
!-->
<template>
  <div>
    <form-container v-model="visible" :title="`${orgProps.opt}组织`" form-size="600px">
      <el-form
        ref="orgFormRef"
        :rules="rules"
        :disabled="orgProps.disabled"
        :model="orgProps.record"
        :hide-required-asterisk="orgProps.disabled"
        label-width="auto"
        label-suffix=" :"
      >
        <s-form-item label="上级组织" prop="parentId">
          <org-selector v-model:org-value="orgProps.record.parentId" />
        </s-form-item>
        <s-form-item label="组织名称" prop="name">
          <s-input v-model="orgProps.record.name"></s-input>
        </s-form-item>
        <s-form-item label="组织编码" prop="code">
          <s-input v-model="orgProps.record.code" placeholder="请填写组织编码,不填则为随机值" clearable></s-input>
        </s-form-item>
        <s-form-item label="组织分类" prop="category">
          <s-radio-group v-model="orgProps.record.category" :options="orgCategoryOptions" button />
        </s-form-item>
        <s-form-item label="状态" prop="status">
          <s-radio-group v-model="orgProps.record.status" :options="statusOptions" button />
        </s-form-item>
        <s-form-item label="指定主管" prop="sortCode">
          <el-button link type="primary" @click="showSelector">选择</el-button>
          <el-tag v-if="orgProps.record.directorId" class="ml-3px" type="warning" closable @close="removeDirector">{{
            orgProps.record.directorInfo?.name
          }}</el-tag>
        </s-form-item>
        <s-form-item label="排序" prop="sortCode">
          <el-slider v-model="orgProps.record.sortCode" show-input :min="1" />
        </s-form-item>
      </el-form>
      <template #footer>
        <el-button @click="onClose"> 取消 </el-button>
        <el-button v-show="!orgProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
      </template>
    </form-container>
    <user-selector
      ref="userSelectorRef"
      :org-tree-api="sysOrgApi.tree"
      :position-tree-api="sysPositionApi.tree"
      :role-tree-api="sysRoleApi.tree"
      :user-selector-api="sysUserApi.selector"
      @successful="handleChooseUser"
    />
  </div>
</template>

<script setup lang="ts">
import { SysOrg, SysUser, sysOrgApi, sysPositionApi, sysRoleApi, sysUserApi } from "@/api";
import { FormOptEnum, SysDictEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";
import { UserSelectorInstance } from "@/components/Selectors/UserSelector/interface";

const visible = ref(false); //是否显示表单
const dictStore = useDictStore(); //字典仓库
// 组织类型选项
const orgCategoryOptions = dictStore.getDictList(SysDictEnum.ORG_CATEGORY);
// 通用状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);

// 表单参数
const orgProps = reactive<FormProps.Base<SysOrg.SysOrgInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  name: [required("请输入组织名称")],
  parentId: [required("请选择上级组织")],
  category: [required("请选择组织类型")],
  status: [required("请选择状态")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysOrg.SysOrgInfo>) {
  Object.assign(orgProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    orgProps.record.sortCode = 99;
    orgProps.record.category = orgCategoryOptions[0].value;
    orgProps.record.status = statusOptions[0].value;
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    sysOrgApi.detail({ id: props.record.id }).then(res => {
      orgProps.record = res.data;
    });
  }
}

// 提交数据（新增/编辑）
const orgFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  orgFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysOrgApi
      .submitForm(orgProps.record, orgProps.record.id != undefined)
      .then(() => {
        orgProps.successful!(); //调用父组件的successful方法
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

const userSelectorRef = ref<UserSelectorInstance>(); //用户选择器引用
/** 显示用户选择器 */
function showSelector() {
  //将orgProps.record.directorInfo转为 SysUser.SysUserInfo[]类型
  const directorInfo = orgProps.record.directorInfo ? [orgProps.record.directorInfo] : [];
  userSelectorRef.value?.showSelector(directorInfo);
}

/** 选择用户 */
function handleChooseUser(data: SysUser.SysUserInfo[]) {
  // 选择用户后，将用户id赋值给orgProps.record.directorId
  if (data.length > 0) {
    orgProps.record.directorId = data[0].id;
    orgProps.record.directorInfo = data[0];
  }
}

/** 移除主管 */
function removeDirector() {
  orgProps.record.directorId = null;
  orgProps.record.directorInfo = null;
}
// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
