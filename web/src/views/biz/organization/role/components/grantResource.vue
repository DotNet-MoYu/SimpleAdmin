<!-- 
 * @Description: 授权资源表单
 * @Author: huguodong 
 * @Date: 2023-12-15 15:43:49
!-->
<template>
  <div>
    <form-container v-model="visible" :title="`授权资源->${grantProps.name}`" form-size="1200px" @close="onClose">
      <el-alert title="非超管角色不可被授权系统模块菜单资源" type="warning" show-icon style="margin-bottom: 10px" />
      <ProTable
        ref="proTable"
        :columns="columns"
        :tool-button="false"
        :cell-style="{ padding: '0px' }"
        :row-style="{ height: '0' }"
        :data="grantProps.tableData"
        :pagination="false"
        :span-method="objectSpanMethod"
      >
        <!-- 表格 header 按钮 -->
        <template #tableHeader>
          <s-radio-group
            v-model="grantProps.moduleId"
            :options="grantProps.echoDatalist"
            value-key="id"
            label-key="title"
            button
            class="mb-2"
            @change="moduleClick"
          >
            <template #radio="{ item }">
              <el-space>
                <svg-icon :icon="item.icon" class="h-4 w-4"></svg-icon>
                {{ item.title }}
              </el-space>
            </template>
          </s-radio-group>
        </template>
        <!-- 一级目录 -->
        <template #parentName="scope">
          <el-checkbox v-model="scope.row.parentCheck" @change="val => changeParent(scope.row, val)">
            {{ scope.row.parentName }}
          </el-checkbox>
        </template>
        <!-- 菜单 -->
        <template #title="scope">
          <el-checkbox v-model="scope.row.nameCheck" @change="val => changeSub(scope.row, val)">
            {{ scope.row.title }}
          </el-checkbox>
        </template>
        <!-- 按钮授权 -->
        <template #button="scope">
          <template v-if="scope.row.button.length > 0">
            <template v-for="(item, index) in scope.row.button" :key="item.id">
              <el-checkbox v-model="item.check" style="margin-right: 5px" @change="val => changeChildCheckBox(scope.row, val)">
                {{ item.title }}
              </el-checkbox>
              <br v-if="(index + 1) % 5 === 0" />
            </template>
          </template>
        </template>
      </ProTable>
      <template #footer>
        <el-button @click="onClose"> 取消 </el-button>
        <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
      </template>
    </form-container>
    <el-dialog v-model="dialogVisible" title="请选择数据范围" width="30%" destroy-on-close>
      <DataScopeSelector v-model="grantProps.defaultDataScope"></DataScopeSelector>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleUserSubmit"> 确定 </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { SysRole, SysUser, bizRoleApi, bizUserApi } from "@/api";
import { ProTableInstance, ColumnProps } from "@/components/ProTable/interface";
import type { TableColumnCtx } from "element-plus";
import DataScopeSelector from "./dataScopeSelector.vue";
const visible = ref(false); //授权资源表单是否显示
const dialogVisible = ref(false); // 数据范围弹窗显示

/** 授权资源表单参数接口 */
interface GrantResourceFormProps {
  /** id */
  id: number | string;
  /** 名称 */
  name: string;
  /** 模块id */
  moduleId: number | string;
  /** 资源树 */
  resTree: SysRole.ResTreeSelector[];
  /** 已拥有的资源 */
  ownResource: SysRole.RoleOwnResource | null;
  /** 一级菜单索引 */
  firstShowMap: Record<string, number[]>;
  /** 回显数据列表 */
  echoDatalist: SysRole.ResTreeSelector[];
  /** 表格数据 */
  tableData: SysRole.RoleGrantResourceMenu[];
  //默认数据范围,用户才有
  defaultDataScope: SysRole.DefaultDataScope;
}

/** 授权资源表单参数 */
const grantProps = reactive<GrantResourceFormProps>({
  id: 0,
  name: "",
  moduleId: 0,
  resTree: [],
  ownResource: null,
  firstShowMap: {},
  echoDatalist: [],
  tableData: [],
  defaultDataScope: SysRole.dataScopeOptions[0]
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
  // { type: "selection", width: 80 },
  { prop: "parentName", label: "一级目录", width: 150, align: "left" },
  { prop: "title", label: "菜单", width: 180, align: "left" },
  { prop: "button", label: "按钮授权", align: "left" }
];

/** 列合并 */
interface SpanMethodProps {
  // 行
  row: SysRole.RoleGrantResourceMenu;
  // 列
  column: TableColumnCtx<SysRole.RoleGrantResourceMenu>;
  // 行索引
  rowIndex: number;
  // 列索引
  columnIndex: number;
}

/** 列合并*/
const objectSpanMethod = ({ row, rowIndex, columnIndex }: SpanMethodProps) => {
  // 如果列索引为0
  if (columnIndex == 0) {
    // 获取父名称
    const parentName = row.parentName;
    // 获取父名称的索引数组
    const indexArr = grantProps.firstShowMap[parentName];
    // 如果行索引等于索引数组的第一个
    if (rowIndex === indexArr[0]) {
      return { rowspan: indexArr.length, colspan: 1 };
    } else {
      return { rowspan: 0, colspan: 0 };
    }
  }
};

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
  if (grantProps.echoDatalist.length > 0) {
    let data = grantProps.echoDatalist.find(f => f.id === grantProps.moduleId)!.menu;
    grantProps.tableData = data;
  } else {
    // 获取角色资源树
    await bizRoleApi.resourceTreeSelector().then(res => {
      grantProps.resTree = res.data;
      grantProps.moduleId = res.data[0].id;
    });
    await getResources(); // 获取资源
    // 如果有已拥有的资源
    if (grantProps.ownResource) {
      // 回显模块数据
      // 获取echoDatalist
      grantProps.echoDatalist = echoModuleData(grantProps.resTree, grantProps.ownResource);
      // 获取模块id
      grantProps.moduleId = grantProps.resTree[0].id;
      // 获取表格数据
      grantProps.tableData = grantProps.echoDatalist[0].menu;
    }
  }
}

/** 获取资源 */
async function getResources() {
  if (props.user) {
    // 获取用户拥有资源
    await bizUserApi.ownResource({ id: grantProps.id }).then(res => {
      grantProps.ownResource = res.data;
    });
  } else {
    // 获取角色拥有资源
    await bizRoleApi.ownResource({ id: grantProps.id }).then(res => {
      grantProps.ownResource = res.data;
    });
  }
}

/**
 * 回显模块数据
 * @param resTree 资源树
 * @param ownResource 已拥有的资源
 */
function echoModuleData(resTree: SysRole.ResTreeSelector[], ownResource: SysRole.RoleOwnResource) {
  // 通过应用循环
  resTree.forEach(module => {
    //如果有菜单
    if (module.menu) {
      module.menu.forEach(menu => {
        const menuCheck = ref(0);
        // 如果已授权资源信息列表有数据
        if (ownResource.grantInfoList.length > 0) {
          // 通过已授权资源信息列表循环
          ownResource.grantInfoList.forEach(grantInfo => {
            // 如果已授权资源信息列表中的菜单id等于当前菜单id
            if (menu.id == grantInfo.menuId) {
              menuCheck.value++;
              // 处理按钮
              if (grantInfo.buttonInfo.length > 0) {
                // 通过已授权资源信息列表中的按钮循环
                grantInfo.buttonInfo.forEach(button => {
                  menu.button.forEach(itemButton => {
                    // 如果已授权资源信息列表中的按钮id等于当前按钮id
                    if (button === itemButton.id) {
                      itemButton.check = true;
                    }
                  });
                });
              }
            }
          });
        }
        // 回显前面的2个
        if (menuCheck.value > 0) {
          menu.parentCheck = true;
          menu.nameCheck = true;
        }
      });
      // 缓存加入索引
      module.menu.forEach((item: SysRole.RoleGrantResourceMenu, index) => {
        // 下面就是用来知道不同的一级菜单里面有几个二级菜单，以及他们所在的索引
        if (grantProps.firstShowMap[item.parentName]) {
          grantProps.firstShowMap[item.parentName].push(index);
        } else {
          grantProps.firstShowMap[item.parentName] = [index];
        }
      });
    }
  });
  return resTree;
}

/** 选择模块 */
function moduleClick() {
  loadData();
}

/** 一级目录点击 */
function changeParent(row: SysRole.RoleGrantResourceMenu, val: any) {
  row.parentCheck = val;
  // 通过这个应用id，找到应用下的所有菜单
  const moduleMenu = grantProps.echoDatalist.find(f => row.module === f.id);
  const parentName = row.parentName;
  // 获取同一级菜单的所有索引
  const indexArr = grantProps.firstShowMap[parentName];
  indexArr.forEach(indexItem => {
    // 获取同一级菜单的所有行
    const row = moduleMenu?.menu[indexItem];

    if (row)
      // 给这些菜单的索引去勾选
      changeSub(row, val);
  });
}

/** 二级菜单的点击 */
function changeSub(row: SysRole.RoleGrantResourceMenu, val: any) {
  // 选中二级菜单
  row.nameCheck = val;
  row.button.forEach(item => {
    // 选中按钮
    item.check = val;
  });
}

/** 权限按钮点击 */
function changeChildCheckBox(row: SysRole.RoleGrantResourceMenu, val: any) {
  // 如果val为false，并且row的子菜单没有选中，则取消所有按钮的选中
  if (!val && checkAllChildNotChecked(row)) {
    // 这里注释掉勾选去掉所有按钮，联动去掉菜单
    /*record.nameCheck = false
			record.parentCheck = false*/
  } else if (val) {
    // 如果val为true，则将row的nameCheck和parentCheck属性设置为true
    row.nameCheck = val;
    row.parentCheck = val;
  }
}

/** 检查所有子级是否都没有勾选 */
function checkAllChildNotChecked(record: SysRole.RoleGrantResourceMenu) {
  // 获取记录的子节点
  const child = record.button;
  // 判断子节点是否被选中，如果有一个子节点未被选中，则返回false
  return child.every(field => !field.check);
}

/** 提交之前转换数据 */
function convertData() {
  const params: SysRole.GrantResourceReq = {
    id: grantProps.id,
    grantInfoList: []
  };
  // 遍历echoDatalist，获取每一项
  grantProps.echoDatalist.forEach(data => {
    // 如果data有menu属性
    if (data.menu) {
      // 遍历data.menu，获取每一项
      data.menu.forEach(item => {
        // 初始化grantInfo
        const grantInfo: SysRole.RelationRoleResource = {
          menuId: 0,
          buttonInfo: []
        };
        // 如果被选中了
        if (item.nameCheck) {
          // 获取item的id
          grantInfo.menuId = item.id;
          // 遍历item.button，获取每一项
          item.button.forEach(button => {
            // 如果button被选中了
            if (button.check) {
              // 将button的id添加到grantInfo.buttonInfo中
              grantInfo.buttonInfo.push(button.id as never);
            }
          });
          // 将grantInfo添加到params.grantInfoList中
          params.grantInfoList.push(grantInfo);
        }
      });
    }
  });
  // 返回params
  return params;
}

/** 提交 */
function handleSubmit() {
  if (props.user) {
    dialogVisible.value = true;
  } else {
    let params = convertData();
    grantResource(params);
  }
}

/** 用户授权资源提交 */
const handleUserSubmit = () => {
  const param = convertData();
  param.defaultDataScope = grantProps.defaultDataScope;
  grantResource(param);
  dialogVisible.value = false;
};

/** 授权资源 */
function grantResource(params: SysRole.GrantResourceReq) {
  // 根据用户判断走哪个方法
  if (props.user) {
    bizUserApi.grantResource(params).then(() => {
      onClose();
    });
  } else {
    bizRoleApi.grantResource(params).then(() => {
      onClose();
    });
  }
}

/** 关闭表单 */
function onClose() {
  visible.value = false;
  grantProps.echoDatalist = [];
  grantProps.moduleId = 0;
  grantProps.firstShowMap = {};
  grantProps.tableData = [];
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
