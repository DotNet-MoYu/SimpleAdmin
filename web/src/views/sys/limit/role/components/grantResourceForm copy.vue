<!-- 授权资源表单 -->
<template>
  <form-container v-model="visible" :title="`授权资源->${grantProps.name}`" form-size="1000px" @close="onClose">
    <el-alert title="非超管角色不可被授权系统模块菜单资源" type="warning" show-icon style="margin-bottom: 10px" />
    <ProTable
      ref="proTable"
      highlight-current-row
      :columns="columns"
      :tool-button="false"
      :data="grantProps.tableData"
      :span-method="objectSpanMethod"
      :pagination="false"
    >
      <!-- 表格 header 按钮 -->
      <template #tableHeader>
        <s-radio-group
          v-model="grantProps.moduleId"
          :options="grantProps.echoDatalist"
          value="id"
          label="title"
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
    </ProTable>
  </form-container>
</template>

<script setup lang="ts">
import { SysResource, SysRole, sysRoleApi } from "@/api";
import { ProTableInstance, ColumnProps } from "@/components/ProTable/interface";
import type { TableColumnCtx } from "element-plus";
// import { FormOptEnum } from "@/enums";

const visible = ref(false); //授权资源表单是否显示

// 授权资源表单参数
interface GrantResourceFormProps {
  /** id */
  id: number | string;
  /** 名称 */
  name: string;
  /** 模块id */
  moduleId: number | string;
  /** 资源树 */
  resTree: SysResource.ResTreeSelector[];
  /** 已拥有的资源 */
  ownResource: SysRole.RoleOwnResource | null;
  /** 一级菜单索引 */
  firstShowMap: Record<string, number[]>;
  /** 回显数据列表 */
  echoDatalist: SysResource.ResTreeSelector[];
  /** 表格数据 */
  tableData: SysResource.RoleGrantResourceMenu[];
}

const grantProps = reactive<GrantResourceFormProps>({
  id: 0,
  name: "",
  moduleId: 0,
  resTree: [],
  ownResource: null,
  firstShowMap: {},
  echoDatalist: [],
  tableData: []
});
// 授权资源表单接口
// interface GrantResourceFormProps {}
// defineProps<GrantResourceFormProps>();
// 获取 ProTable DOM
const proTable = ref<ProTableInstance>();
// 表格配置项
const columns: ColumnProps<SysResource.ResTreeSelector>[] = [
  // { type: "selection", width: 80 },
  { prop: "parentName", label: "一级目录", width: 100 },
  { prop: "title", label: "菜单", width: 100 },
  { prop: "button", label: "按钮授权" }
];

// 列合并
interface SpanMethodProps {
  row: SysResource.RoleGrantResourceMenu;
  column: TableColumnCtx<SysResource.RoleGrantResourceMenu>;
  rowIndex: number;
  columnIndex: number;
}
const objectSpanMethod = ({ row, rowIndex, columnIndex }: SpanMethodProps) => {
  // console.log("[ row ] >", row, rowIndex);
  console.log("[  rowIndex ] >", rowIndex);
  console.log("[  columnIndex ] >", columnIndex);
  // const parentName = row.parentName;
  // console.log("[ parentName ] >", parentName);
  // const indexArr = grantProps.firstShowMap[parentName];
  // console.log("[ indexArr ] >", indexArr);
  // if (rowIndex === indexArr[0]) {
  //   return { rowSpan: 3, colspan: 1 };
  // }
  // return { rowspan: 3, colspan: 1 };
  // return { rowSpan: 2, colSpan: 0 };

  const parentName = row.parentName;
  const indexArr = grantProps.firstShowMap[parentName];
  console.log("[ indexArr ] >", indexArr);
  if (rowIndex === indexArr[0]) {
    console.log("[columnIndex === indexArr[0]  ] >", columnIndex);
    return { rowspan: indexArr.length, colspan: 1 };
  }
  return { rowspan: 1, colspan: 1 };
};

/**
 * 打开表单
 * @param props 表单参数
 */
function onOpen(props: FormProps.Base<SysRole.SysRoleInfo>) {
  visible.value = true;
  grantProps.name = props.record.name || "";
}

// 生命周期钩子
onMounted(async () => {
  await loadData();
});

/** 加载数据 */
async function loadData() {
  if (grantProps.echoDatalist.length > 0) {
    let data = grantProps.echoDatalist.find(f => f.id === grantProps.moduleId)!.menu;
    grantProps.tableData = data;
  } else {
    // 获取角色资源树
    await sysRoleApi.roleResourceTreeSelector().then(res => {
      grantProps.resTree = res.data;
      grantProps.moduleId = res.data[0].id;
    });
    // 获取角色拥有资源
    await sysRoleApi.ownResource({ id: grantProps.id }).then(res => {
      grantProps.ownResource = res.data;
    });
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

/**
 * 回显模块数据
 * @param resTree 资源树
 * @param ownResource 已拥有的资源
 */
function echoModuleData(resTree: SysResource.ResTreeSelector[], ownResource: SysRole.RoleOwnResource) {
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
      module.menu.forEach((item: SysResource.RoleGrantResourceMenu, index) => {
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

/** 关闭表单 */
function onClose() {
  visible.value = false;
}

// 暴露给父组件的方法
defineExpose({
  onOpen
});
</script>

<style lang="scss" scoped></style>
