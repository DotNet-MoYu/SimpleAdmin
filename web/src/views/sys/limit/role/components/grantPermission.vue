<!-- 
 * @Description: 授权权限表单
 * @Author: huguodong 
 * @Date: 2023-12-15 15:43:40
!-->
<template>
  <form-container v-model="visible" :title="`授权权限->${grantProps.name}`" form-size="1200px" @close="onClose">
    <el-alert
      title="注：此功能界面需要与代码查询条件配合使用，并非所有接口都需设置数据范围，多用于业务模块！"
      type="warning"
      show-icon
      style="margin-bottom: 10px"
    />
    <ProTable ref="proTable" :columns="columns" :tool-button="false" :data="grantProps.tableData" row-key="api">
      <!-- 自定义api表头 -->
      <template #apiHeader>
        <el-row class="row-bg" justify="space-between">
          <el-col :span="12"><el-checkbox @change="onCheckAllChange"> 接口 </el-checkbox></el-col>
          <el-col :span="12"> <el-input v-model="searchValue" @input="searchApi" placeholder="请输入接口名称" /></el-col>
        </el-row>
      </template>
      <!-- 接口 -->
      <template #api="scope">
        <el-checkbox v-model="scope.row.check" @change="val => changeApi(scope.row, val)">
          {{ scope.row.api }}
        </el-checkbox>
      </template>
      <!-- 数据范围 -->
      <template #dataScope="scope">
        <DataScopeSelector v-if="scope.row.check" v-model="scope.row.dataScope"></DataScopeSelector>
      </template>
    </ProTable>
    <template #footer>
      <el-button @click="onClose"> 取消 </el-button>
      <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
    </template>
  </form-container>
</template>

<script setup lang="ts">
import { SysRole, sysRoleApi, sysUserApi, SysUser } from "@/api";
import { ProTableInstance, ColumnProps } from "@/components/ProTable/interface";
import DataScopeSelector from "./dataScopeSelector.vue";

const visible = ref(false); //授权权限表单是否显示
const searchValue = ref(""); // 搜索的值
// 数据范围列表
const scopeOptions = SysRole.dataScopeOptions;

/** 授权权限表单参数 */
interface GrantPermissionFormProps {
  /** id */
  id: number | string;
  /** 名称 */
  name: string;
  /** 表格数据 */
  tableData: SysRole.RoleGrantPermission[];
  /** 权限数据 */
  permissionData: SysRole.RoleGrantPermission[];
}

//表单参数
const grantProps = reactive<GrantPermissionFormProps>({
  id: 0,
  name: "",
  tableData: [],
  permissionData: []
});

/** props */
const props = defineProps<{
  /** 是否是用户 */
  user?: boolean;
}>();

// 获取 ProTable DOM
const proTable = ref<ProTableInstance>();
// 表格配置项
const columns: ColumnProps<SysRole.ResTreeSelector>[] = [
  // { type: "selection", width: 60 },
  { prop: "api", label: "接口", width: 480, align: "left" },
  { prop: "dataScope", label: "数据范围", align: "left" }
];

/**
 * 打开表单
 * @param props 表单参数
 */
async function onOpen(props: FormProps.Base<SysRole.SysRoleInfo | SysUser.SysUserInfo>) {
  grantProps.id = props.record.id || 0;
  grantProps.name = props.record.name || "";
  await loadData();
  visible.value = true;
}

/** 加载数据 */
async function loadData() {
  const params = { id: grantProps.id };
  // 获取权限树
  const tree = props.user ? await sysUserApi.permissionTreeSelector(params) : await sysRoleApi.permissionTreeSelector(params);
  // 获取当前角色拥有的权限
  const ownPer = props.user ? await sysUserApi.ownPermission(params) : await sysRoleApi.ownPermission(params);
  // 调用echoModuleData方法，将权限树和当前角色拥有的权限进行合并
  echoModuleData(tree.data, ownPer.data);
}

/**
 * 回显模块数据
 * @param apiList 权限列表
 * @param ownPermission 已拥有的权限
 */
function echoModuleData(apiList: string[], ownPermission: SysRole.RoleOwnPermission) {
  //遍历api列表
  apiList.forEach(item => {
    let data: SysRole.RoleGrantPermission = {
      api: item,
      dataScope: SysRole.dataScopeOptions[0],
      check: false
    };
    let api = subStrApi(item); // 截取api串中的中文及括号
    //找到对应的Api权限
    let grantInfo = ownPermission.grantInfoList.find(grantInfo => grantInfo.apiUrl === api);
    if (grantInfo) {
      data.check = true;
      let dataScope = scopeOptions.find(scope => scope.scopeCategory === grantInfo?.scopeCategory);
      // 找到对应的数据范围
      data.dataScope = { ...dataScope! }; //合并参数
      //  如果scopeCategory为自定义数据范围则赋值数据范围
      if (grantInfo.scopeCategory == SysRole.DataScopeEnum.SCOPE_ORG_DEFINE) {
        data.dataScope.scopeDefineOrgIdList = grantInfo.scopeDefineOrgIdList;
      }
    }
    grantProps.tableData.push(data);
  });
  grantProps.permissionData = grantProps.tableData; //保存权限数据
}

/** 搜索api
 * @param value 搜索值
 */
function searchApi(value: string) {
  console.log("[ value ] >", value);
  //如果搜索值为空，就显示全部
  if (!value) {
    grantProps.tableData = grantProps.permissionData;
    return;
  }
  //过滤api
  grantProps.tableData = grantProps.permissionData.filter(item => {
    return item.api.indexOf(value) > -1;
  });
}

/** 提交之前转换数据 */
function convertData() {
  //将tableData的数据合并到permissionData的中
  grantProps.permissionData.forEach(item => {
    let data = grantProps.tableData.find(data => data.api === item.api);
    if (data) {
      item.check = data.check;
      item.dataScope = data.dataScope;
    }
  });
  //定义参数
  const params: SysRole.GrantPermissionReq = {
    id: grantProps.id,
    grantInfoList: []
  };
  // 遍历表格数据赋值给参数列表
  grantProps.permissionData.forEach(item => {
    if (item.check) {
      let grantInfo: SysRole.RelationRolePermission = {
        apiUrl: subStrApi(item.api),
        scopeCategory: item.dataScope.scopeCategory,
        scopeDefineOrgIdList: item.dataScope.scopeDefineOrgIdList
      };
      params.grantInfoList.push(grantInfo);
    }
  });
  return params;
}

/** 截取api串中的中文及括号 */
function subStrApi(api: string) {
  return api.substring(0, api.indexOf("[")); // 截取api串中的中文及括号
}

/** 选中api */
function changeApi(row: SysRole.RoleGrantPermission, val: any) {
  row.check = val; //设置选中状态
  //如果选中了
  if (val) {
    row.dataScope.check = true;
  } else {
    //去掉已选中的
    row.dataScope.check = false;
    if (row.dataScope.scopeCategory == SysRole.DataScopeEnum.SCOPE_ORG_DEFINE) {
      row.dataScope.scopeDefineOrgIdList = [];
    }
  }
}

/** 全选/取消 */
function onCheckAllChange(val: any) {
  grantProps.tableData.forEach(item => {
    changeApi(item, val);
  });
}

/** 提交 */
function handleSubmit() {
  let params = convertData();
  //判断是否是用户
  if (props.user) {
    sysUserApi.grantPermission(params).then(() => {
      onClose();
    });
  } else {
    sysRoleApi.grantPermission(params).then(() => {
      onClose();
    });
  }
}

/** 关闭表单 */
function onClose() {
  visible.value = false;
  grantProps.tableData = [];
  grantProps.permissionData = [];
  searchValue.value = "";
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
