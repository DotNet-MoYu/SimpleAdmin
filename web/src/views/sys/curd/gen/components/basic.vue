<!-- 
 * @Description: 基础信息
 * @Author: huguodong 
 * @Date: 2025-01-08 16:05:34
!-->
<template>
  <el-form ref="basicFormRef" :rules="rules" :model="basicProps.record" label-width="auto" label-suffix=" :" label-position="top">
    <el-card :bordered="true" header="基本设置">
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="选择主表" prop="dbTable">
            <s-select v-model="basicProps.record.dbTable" :options="basicOptions.tableOptions" filterable @change="formFieldAssign" />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="表前缀移除" prop="tablePrefix">
            <s-radio-group v-model="basicProps.record.tablePrefix" :options="genConst.tablePrefixOptions" @change="tablePrefixChange">
            </s-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="生成方式" prop="generateType">
            <s-radio-group v-model="basicProps.record.generateType" :options="genConst.generateTypeOptions"> </s-radio-group>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="选择生成模板" prop="moduleType" :popover="{ title: '生成模板说明', placement: 'top-start', width: '500' }">
            <template #popover>
              <p>单表:生成普通单表的增删改查</p>
              <p>树表-列表型:生成树型结构的表的增删改查,参考组织管理</p>
              <p>树表-树型:生成树型结构的表的增删改查,参考菜单管理</p>
              <p>主子表:生成主表关联子表的增删改查,删除主表数据会清除相应的子表数据</p>
            </template>
            <s-select v-model="basicProps.record.moduleType" :options="genConst.moduleTypeOptions" />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="数据权限" prop="dataPermission">
            <s-radio-group v-model="basicProps.record.dataPermission" :options="genConst.dataPermissionOptions"></s-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="4">
          <s-form-item label="功能名" prop="functionName">
            <s-input v-model="basicProps.record.functionName" allow-clear />
          </s-form-item>
        </el-col>
        <el-col :span="4">
          <s-form-item label="后缀" prop="functionNameSuffix">
            <s-input v-model="basicProps.record.functionNameSuffix" allow-clear />
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="功能列表" prop="funcList">
            <s-select v-model="basicProps.record.funcList" multiple :options="genConst.funcOptions" />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="作者" prop="authorName">
            <s-input v-model="basicProps.record.authorName" allow-clear />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="排序" prop="sortCode">
            <el-slider v-model="basicProps.record.sortCode" :max="100" style="width: 100%" />
          </s-form-item>
        </el-col>
      </el-row>
      <el-divider v-if="showTree" content-position="left">树表配置</el-divider>
      <el-row v-if="showTree" :gutter="24">
        <el-col :span="8">
          <s-form-item label="树ID字段" prop="treeId">
            <s-select v-model="basicProps.record.treeId" :options="basicOptions.columnOptions" filterable />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="树父ID字段" prop="treePid">
            <s-select v-model="basicProps.record.treeId" :options="basicOptions.columnOptions" filterable />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="树名称字段" prop="treeName">
            <s-select v-model="basicProps.record.treeId" :options="basicOptions.columnOptions" filterable />
          </s-form-item>
        </el-col>
      </el-row>
      <el-divider v-if="showMaster" content-position="left">主子表配置</el-divider>
      <el-row v-if="showMaster" :gutter="24">
        <el-col :span="8">
          <s-form-item label="关联子表的表名" prop="childTable">
            <s-select v-model="basicProps.record.childTable" :options="basicOptions.childTableList" filterable @change="selectChildTable" />
          </s-form-item>
        </el-col>

        <el-col :span="8">
          <s-form-item label="树ID字段" prop="childFk">
            <s-select v-model="basicProps.record.childFk" :options="basicOptions.childColumnOptions" filterable />
          </s-form-item>
        </el-col>
      </el-row>
    </el-card>
    <el-card :bordered="true" header="前端设置" class="mt-2">
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="前端文件夹路径" prop="frontedPath">
            <s-input v-model="basicProps.record.frontedPath" allow-clear />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="所属模块" prop="module">
            <s-select
              v-model="basicProps.record.module"
              :options="basicOptions.moduleOptions"
              filterable
              @change="moduleChange(basicProps.record.module as number, false)"
            />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="上级目录" prop="servicePosition">
            <MenuSelector
              v-model:menu-value="basicProps.record.menuPid"
              :check-strictly="true"
              :menu-tree-data="basicOptions.menuTreeData"
              :show-all="true"
              :only-catalog="true"
              @change="changeMenu"
            />
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="表单布局" prop="formLayout">
            <s-radio-group v-model="basicProps.record.formLayout" :options="genConst.formLayoutOptions"></s-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="使用栅格" prop="gridWhether">
            <s-radio-group v-model="basicProps.record.gridWhether" :options="genConst.gridWhetherOptions"></s-radio-group>
          </s-form-item>
        </el-col>
        <el-col :span="8" v-if="basicProps.record.moduleType === genConst.moduleTypeOptions[0].value">
          <s-form-item label="左侧树" prop="leftTree">
            <s-select v-model="basicProps.record.leftTree" :options="genConst.treeOptions" filterable />
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="路由名(最终路由名是：路由名+业务名)" prop="routeName">
            <s-input v-model="basicProps.record.routeName" allow-clear placeholder="根据上级目录自动生成,顶级需手动填写" />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="图标" prop="icon">
            <SelectIconPlus v-if="showIcon" v-model:icon-value="basicProps.record.icon" />
          </s-form-item>
        </el-col>
      </el-row>
    </el-card>

    <el-card :bordered="true" header="后端设置" class="mt-2">
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="业务名(最好和文件夹路径对应)" prop="busName">
            <s-input v-model="basicProps.record.busName" allow-clear />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="类名" prop="className">
            <s-input v-model="basicProps.record.className" allow-clear />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="服务层代码位置" prop="servicePosition">
            <el-input style="max-width: 600px" v-model="basicProps.record.serviceDictionary" placeholder="请输入文件夹">
              <template #prepend>
                <s-select style="width: 250px" v-model="basicProps.record.servicePosition" :options="basicOptions.servicePositions" filterable />
              </template>
            </el-input>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="24">
        <el-col :span="8">
          <s-form-item label="接口层代码位置" prop="controllerPosition">
            <el-input style="max-width: 600px" v-model="basicProps.record.controllerDictionary" placeholder="请输入文件夹">
              <template #prepend>
                <s-select style="width: 250px" v-model="basicProps.record.controllerPosition" :options="basicOptions.servicePositions" filterable />
              </template>
            </el-input>
          </s-form-item>
        </el-col>
      </el-row>
    </el-card>
  </el-form>
</template>

<script setup lang="ts">
import { GenCode, genBasicApi, userCenterApi } from "@/api";
import { FormOptEnum } from "@/enums";
import { required } from "@/utils/formRules";
import { useUserStore } from "@/stores/modules";
import * as genConst from "../genConst";
import { FormInstance } from "element-plus";
import { MenuCategoryEnum } from "@/enums";
const userStore = useUserStore();
const basicFormRef = ref<FormInstance>();
const showIcon = ref(false);

// 选项接口
interface BasicOptions {
  /*模块选项 */
  moduleOptions: { label: string; value: string | number }[];
  /*服务列表 */
  servicePositions: { label: string; value: string | number }[];
  /*控制器选项 */
  controllerPositions: { label: string; value: string | number }[];
  /*数据表列表 */
  tableOptions: GenCode.TableList[];
  /*字段选项 */
  columnOptions: { label: string; value: string }[];
  /*子表字段 */
  childColumnOptions: { label: string; value: string }[];
  /*子表列表 */
  childTableList: GenCode.TableList[];
  /*菜单选择器数据 */
  menuTreeData: any[];
}

// 表单参数
const basicProps = reactive<FormProps.Base<GenCode.GenBasic>>({
  opt: FormOptEnum.ADD,
  record: {}
});

//显示树表配置
const showTree = computed(() => {
  return basicProps.record.moduleType && basicProps.record.moduleType.includes("tree");
});

//显示主子表配置
const showMaster = computed(() => {
  return basicProps.record.moduleType && basicProps.record.moduleType.includes("master");
});

//下拉用的数据
const basicOptions: BasicOptions = reactive({
  tableOptions: [],
  moduleOptions: [],
  servicePositions: [],
  controllerPositions: [],
  columnOptions: [],
  childColumnOptions: [],
  childTableList: [],
  menuTreeData: []
});

// 表单验证规则
const rules = reactive({
  tablePrefix: [required("请选择是否移除表前缀")],
  dbTable: [required("请选择主表")],
  dbTableKey: [required("请选择主表主键")],
  generateType: [required("请选择生成方式")],
  module: [required("请选择所属模块")],
  menuPid: [required("请选择上级目录")],
  routeName: [required("请输入路由名")],
  icon: [required("请选择图标")],
  functionName: [required("请输入功能名")],
  busName: [required("请输入业务名")],
  className: [required("请输入类名")],
  frontedPath: [required("请输入前端路径")],
  servicePosition: [required("请输入服务层代码项目")],
  serviceDictionary: [required("请输入服务层代码文件夹")],
  controllerPosition: [required("请输入接口层代码位置")],
  sortCode: [required("请选择排序")],
  formLayout: [required("请选择表单布局")],
  gridWhether: [required("请选择是否使用栅格")],
  authorName: [required("请输入作者名")],
  funcList: [required("请选择功能列表")],
  moduleType: [required("请输入模版类型")],
  dataPermission: [required("请选择数据权限")],
  treeId: [required("请选择树Id字段")],
  treePid: [required("请选择树父Id字段")],
  treeName: [required("请选择树名称字段")],
  childTable: [required("请选择关联子表的表名")],
  childFk: [required("请选择关联子表的外键")]
});

/**
 * 打开表单
 * @param record 表单数据
 */
function onOpen(record: GenCode.GenBasic) {
  //加载默认的模块
  basicOptions.moduleOptions = userStore.moduleListGet.map(item => {
    return {
      label: item.title,
      value: item.id
    };
  });

  //获取程序集
  genBasicApi.assemblies().then(res => {
    const data = res.data;
    basicOptions.servicePositions = data.map(item => {
      return {
        value: item,
        label: item
      };
    });
    basicOptions.controllerPositions = data.map(item => {
      return {
        value: item,
        label: item
      };
    });
  });

  // 获取数据库中的所有表
  genBasicApi.tables({}).then(res => {
    const data = res.data;
    basicOptions.tableOptions = data.map(item => {
      return {
        value: item.tableName,
        tableName: item.tableName,
        configId: item.configId,
        label: `${item.tableDescription}-${item.configId}-${item.entityName}`,
        entityName: item.entityName,
        tableDescription: item.tableDescription || item.entityName,
        tableColumns: item.columns
      };
    });
    if (record && Object.keys(record).length > 0) {
      basicProps.record = record;
      autoModuleType(record); //自动填写模块类型
      moduleChange(record.module, true); //自动填写上级目录
      showIcon.value = true;
    } else {
      basicProps.record = {
        sortCode: 99,
        authorName: "superAdmin",
        tablePrefix: "Y",
        servicePosition: "",
        serviceDictionary: "Services",
        controllerPosition: "",
        controllerDictionary: "Controllers/Application",
        generateType: "PRO",
        functionName: "",
        functionNameSuffix: "管理",
        formLayout: "vertical",
        gridWhether: "N",
        leftTree: "null",
        moduleType: "single",
        dataPermission: "N",
        funcList: ["curd"],
        icon: ""
      };
      showIcon.value = true;
    }
  });
}

/**
 * 生成模版字段填充
 * @param record  表单数据
 */
function autoModuleType(record) {
  //获取表信息
  const table = basicOptions.tableOptions.find(item => {
    return item.tableName === record.dbTable && item.configId == record.configId;
  });
  if (table) {
    //树ID列表
    basicOptions.columnOptions = table.tableColumns.map(item => {
      return {
        value: item.columnName,
        label: `${item.columnName}:${item.columnDescription}`
      };
    });
    //获取子表列表
    basicOptions.childTableList = basicOptions.tableOptions.filter(item => item.configId == table.configId);
    //获取子表信息
    let subTable = basicOptions.childTableList.find(item => {
      return item.tableName === record.childTable && item.configId == record.configId;
    });
    if (subTable) {
      //获取子表字段
      basicOptions.childColumnOptions = subTable.tableColumns.map(item => {
        return {
          value: item.columnName,
          label: `${item.columnName}:${item.columnDescription}`
        };
      });
    }
  }
}

/**
 * 表前缀移除
 *
 */
function tablePrefixChange() {
  const tableName = basicProps.record.dbTable;
  if (tableName) {
    const tableNameHump = getTableNameToHump(tableName);
    basicProps.record.busName = tableNameHump.toLowerCase();
  }
}

/**
 * 获取表名转驼峰
 * @param tableName  表名
 */
function getTableNameToHump(tableName: string) {
  if (tableName) {
    const arr = tableName.toLowerCase().split("_");
    if (basicProps.record.tablePrefix === "Y") {
      arr.splice(0, 1);
    }
    for (let i = 0; i < arr.length; i++) {
      // charAt()方法得到第一个字母，slice()得到第二个字母以后的字符串
      arr[i] = arr[i].charAt(0).toUpperCase() + arr[i].slice(1);
    }
    return arr.join("");
  }
  return "";
}

/**
 * 获取表名转驼峰
 * @param tableName  表名
 */
function getClassName(tableName) {
  if (tableName) {
    const arr = tableName.toLowerCase().split("_");
    for (let i = 0; i < arr.length; i++) {
      // charAt()方法得到第一个字母，slice()得到第二个字母以后的字符串
      arr[i] = arr[i].charAt(0).toUpperCase() + arr[i].slice(1);
    }
    return arr.join("");
  }
  return "";
}

/**
 * 表单内设置默认的值
 * @param value
 * @param options
 */
function formFieldAssign(value: string) {
  const option = basicOptions.tableOptions.find(item => item.value === value);
  if (option) {
    const tableNameHump = getTableNameToHump(option.tableName);
    basicProps.record.busName = tableNameHump.toLowerCase();
    basicProps.record.className = getClassName(option.tableName);
    basicProps.record.entityName = option.entityName;
    basicProps.record.configId = option.configId;
    basicOptions.columnOptions = option.tableColumns.map(item => {
      return {
        value: item.columnName,
        label: `${item.columnName}:${item.columnDescription}`
      };
    });
    basicOptions.childTableList = basicOptions.tableOptions.filter(item => {
      return item.configId == option.configId;
    });
  }
}

/**
 * 选择子表
 * @param value
 */
function selectChildTable(value: string) {
  const option = basicOptions.childTableList.find(item => item.value === value);
  if (option) {
    basicProps.record.childTable = option.entityName; //子表名实体赋值
    basicOptions.childColumnOptions = option.tableColumns.map(item => {
      return {
        value: item.columnName,
        label: `${item["columnName"]}-${item.columnDescription}`
      };
    });
  }
}

// 选择模板的回调
function moduleChange(value: number, assign: boolean) {
  if (!assign) {
    // 先去掉值
    basicProps.record.menuPid = undefined;
  }
  userCenterApi.getAuthMenuList({ id: value }).then(res => {
    console.log("res", res.data);
    //去掉单页
    const menuTreeData = res.data.filter(item => item.category != MenuCategoryEnum.SPA);
    basicOptions.menuTreeData = menuTreeData;
  });
}

/**
 * 验证并提交数据
 */
function onSubmit(): Promise<GenCode.GenBasic> {
  console.log("basic submit");

  return new Promise((resolve, reject) => {
    basicFormRef.value
      ?.validate()
      .then(() => {
        genBasicApi.submitForm(basicProps.record, basicProps.record.id != undefined).then(res => {
          resolve(res.data);
        });
      })
      .catch(err => {
        reject(err);
      });
  });
}

/** 选中菜单事件 */
function changeMenu(value: string) {
  basicProps.record.routeName = value;
}

// 暴露给父组件的方法
defineExpose({
  onOpen,
  onSubmit
});
</script>

<style lang="scss" scoped></style>
