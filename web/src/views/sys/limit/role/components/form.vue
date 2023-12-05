<!-- 表单 -->
<template>
  <div>
    <form-container v-model="visible" :title="`${sysRoleProps.opt}角色`" form-size="800px" @close="onClose">
      <el-form
        ref="sysRoleFormRef"
        :rules="rules"
        :disabled="sysRoleProps.disabled"
        :model="sysRoleProps.record"
        :hide-required-asterisk="sysRoleProps.disabled"
        label-width="auto"
        label-suffix=" :"
      >
        <s-form-item label="角色名称" prop="name">
          <s-input v-model="sysRoleProps.record.name"></s-input>
        </s-form-item>
        <s-form-item label="角色分类" prop="category">
          <s-radio-group v-model="sysRoleProps.record.category" :options="roleCategoryOptions" button />
        </s-form-item>
        <s-form-item v-if="sysRoleProps.record.category === OrgCategoryEnum.ORG" label="所属组织" prop="orgId">
          <org-selector v-model:org-value="sysRoleProps.record.orgId" />
        </s-form-item>
        <s-form-item label="角色编码" prop="code">
          <s-input v-model="sysRoleProps.record.code" placeholder="请填写角色编码,不填则为随机值"></s-input>
        </s-form-item>
        <s-form-item label="默认数据范围" prop="code">
          <el-space>
            <s-radio-group v-model="defaultLevel" :options="scopeOptions" value="level" label="title" @change="changeDataScope">
              <template #radio="{ item }">
                <el-badge
                  v-if="defaultLevel === defineLevel && item.scopeCategory === SysRole.DataScopeEnum.SCOPE_ORG_DEFINE && item.scopeDefineOrgIdList"
                  :value="defineOrgIdList.length"
                  type="warning"
                >
                  {{ item.title }}
                </el-badge>
                <span v-else>{{ item.title }}</span>
              </template>
            </s-radio-group>
            <div v-if="defaultLevel === defineLevel">
              <el-button link type="primary" class="ml-15px" @click="handleDefineOrg">选择组织</el-button>
            </div>
          </el-space>
        </s-form-item>
        <s-form-item label="状态" prop="status">
          <s-radio-group v-model="sysRoleProps.record.status" :options="statusOptions" button />
        </s-form-item>

        <s-form-item label="排序" prop="sortCode">
          <el-slider v-model="sysRoleProps.record.sortCode" show-input :min="1" />
        </s-form-item>
      </el-form>
      <template #footer>
        <el-button @click="onClose"> 取消 </el-button>
        <el-button v-show="!sysRoleProps.disabled" type="primary" @click="handleSubmit"> 确定 </el-button>
      </template>
    </form-container>
    <el-dialog v-model="dialogVisible" title="选择组织" width="400" destroy-on-close>
      <TreeFilter
        title="组织列表"
        multiple
        check-strictly
        class="treeFilter"
        label="name"
        :request-api="sysOrgApi.sysOrgTree"
        :default-value="defineOrgIdList"
        @change="changeTreeFilter"
      />
      <template #footer>
        <el-button @click="dialogCancel">取消</el-button>
        <el-button type="primary" @click="chooseDefineOrg"> 确定 </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { SysRole, sysRoleApi, sysOrgApi } from "@/api";
import { FormOptEnum, SysDictEnum, OrgCategoryEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { FormInstance } from "element-plus";
import { useDictStore } from "@/stores/modules";

const visible = ref(false); //是否显示表单
const dialogVisible = ref(false); //是否显示弹窗
const dictStore = useDictStore(); //字典仓库
// 角色类型选项
const roleCategoryOptions = dictStore.getDictList(SysDictEnum.ROLE_CATEGORY);
// 通用状态选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);
const scopeOptions = SysRole.dataScopeOptions;
// 默认数据范围选项
const defaultLevel = ref<number>(SysRole.dataScopeOptions[0].level);
// 自定义数据范围级别
const defineLevel = SysRole.dataScopeOptions.find(item => item.scopeCategory === SysRole.DataScopeEnum.SCOPE_ORG_DEFINE)?.level;
// 自定义组织id列表
const defineOrgIdList = ref<number[] | string[]>([]);
// 表单参数
const sysRoleProps = reactive<FormProps.Base<SysRole.SysRoleInfo>>({
  opt: FormOptEnum.ADD,
  record: {},
  disabled: false
});

// 表单验证规则
const rules = reactive({
  name: [required("请输入角色名称")],
  orgId: [required("请选择所属组织")],
  category: [required("请选择角色类型")],
  status: [required("请选择状态")]
});

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysRole.SysRoleInfo>) {
  Object.assign(sysRoleProps, props); //合并参数
  if (props.opt == FormOptEnum.ADD) {
    //如果是新增,设置默认值
    sysRoleProps.record.sortCode = 99;
    sysRoleProps.record.category = roleCategoryOptions[0].value;
    sysRoleProps.record.status = statusOptions[0].value;
    sysRoleProps.record.defaultDataScope = SysRole.dataScopeOptions[0];
    defaultLevel.value = SysRole.dataScopeOptions[0].level;
    defineOrgIdList.value = [];
  }
  visible.value = true; //显示表单
  if (props.record.id) {
    //如果传了id，就去请求api获取record
    sysRoleApi.sysRoleDetail({ id: props.record.id }).then(res => {
      sysRoleProps.record = res.data;
      defaultLevel.value = res.data.defaultDataScope?.level || 5;
      defineOrgIdList.value = res.data.defaultDataScope?.scopeDefineOrgIdList || [];
    });
  }
  console.log("[defineOrgIdList.value  ] >", sysRoleProps.record.defaultDataScope?.scopeDefineOrgIdList);
}

// 提交数据（新增/编辑）
const sysRoleFormRef = ref<FormInstance>();
/** 提交表单 */
async function handleSubmit() {
  sysRoleFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //提交表单
    await sysRoleApi
      .sysRoleSubmitForm(sysRoleProps.record, sysRoleProps.record.id != undefined)
      .then(() => {
        sysRoleProps.successful!(); //调用父组件的successful方法
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

/** 修改数据权限 */
function changeDataScope(value: any) {
  let dataScope = SysRole.dataScopeOptions.find(item => item.level === value);
  sysRoleProps.record.defaultDataScope = dataScope;
}

/** 处理自定义 */
function handleDefineOrg() {
  dialogVisible.value = true;
}

/** 组织树数据变化 */
function changeTreeFilter(val: any[]) {
  defineOrgIdList.value = val;
}

/** 取消选择组织 */
function dialogCancel() {
  // 关闭弹窗
  dialogVisible.value = false;
}

/** 选择机构 */
function chooseDefineOrg() {
  // 设置默认数据权限中的机构ID列表
  sysRoleProps.record.defaultDataScope!.scopeDefineOrgIdList = defineOrgIdList.value;
  // 关闭弹窗
  dialogVisible.value = false;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped>
.treeFilter {
  width: 100% !important;
}
</style>
