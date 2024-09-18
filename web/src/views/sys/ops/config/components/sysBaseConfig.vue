<!-- 
 * @Description: 系统配置
 * @Author: huguodong 
 * @Date: 2024-01-22 16:44:07
!-->
<template>
  <div>
    <el-form ref="sysConfigFormRef" :rules="rules" :model="sysBaseProps" label-width="auto" label-suffix=" :" label-position="top">
      <el-row :gutter="16">
        <el-col :span="8">
          <s-form-item label="系统LOGO" prop="SYS_LOGO">
            <UploadImg v-model:image-url="sysBaseProps.SYS_LOGO" width="135px" height="135px" :file-size="3" :auto-upload="false">
              <template #empty>
                <el-icon><Avatar /></el-icon>
                <span>请上传LOGO</span>
              </template>
            </UploadImg>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="系统ico文件" prop="SYS_ICO">
            <UploadImg
              v-model:image-url="sysBaseProps.SYS_ICO!"
              width="135px"
              height="135px"
              :file-size="1"
              :file-type="icoType"
              :auto-upload="false"
            >
              <template #empty>
                <el-icon><Picture /></el-icon>
                <span>请上传.ICO文件</span>
              </template>
            </UploadImg>
          </s-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="8">
          <s-form-item label="系统名称" prop="SYS_NAME">
            <s-input v-model="sysBaseProps.SYS_NAME"></s-input>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="系统版本" prop="SYS_VERSION">
            <s-input v-model="sysBaseProps.SYS_VERSION"></s-input>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="默认快捷方式" prop="SYS_DEFAULT_WORKBENCH_DATA">
            <MenuSelector
              v-model:menu-value="sysBaseProps.SYS_DEFAULT_WORKBENCH_DATA!.shortcut"
              multiple
              :check-strictly="false"
              :menu-tree-api="menuApi.menuTreeSelector"
            />
          </s-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="8">
          <s-form-item label="多租户配置" prop="SYS_TENANT_OPTIONS">
            <s-radio-group v-model="sysBaseProps.SYS_TENANT_OPTIONS" :options="tenantOptions" button />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="版权信息" prop="SYS_COPYRIGHT">
            <s-input v-model="sysBaseProps.SYS_COPYRIGHT"></s-input>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="版权链接URL" prop="SYS_COPYRIGHT_URL">
            <s-input v-model="sysBaseProps.SYS_COPYRIGHT_URL"></s-input>
          </s-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="8">
          <s-form-item label="底部超链接" prop="SYS_FOOTER_LINKS">
            <el-tag
              v-for="tag in sysBaseProps.SYS_FOOTER_LINKS"
              :key="tag.name"
              class="mx-1"
              closable
              :disable-transitions="false"
              @close="handleClose(tag.name)"
              @click="linkDetail(tag)"
            >
              {{ tag.name }}
            </el-tag>

            <el-button class="ml-1" size="small" @click="showLinkModel()">添加</el-button>
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="网站开启访问(仅业务)" prop="SYS_WEB_STATUS">
            <s-radio-group v-model="sysBaseProps.SYS_WEB_STATUS" :options="statusOptions" button />
          </s-form-item>
        </el-col>
        <el-col :span="8">
          <s-form-item label="网站关闭提示" prop="SYS_WEB_CLOSE_PROMPT">
            <s-input v-model="sysBaseProps.SYS_WEB_CLOSE_PROMPT" :rows="3" type="textarea"></s-input>
          </s-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="16">
        <el-col :span="24">
          <el-form-item>
            <el-button type="primary" :loading="submitLoading" @click="onSubmit()">保存</el-button>
            <el-button style="margin-left: 10px" @click="resetForm">重置</el-button>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <!-- 超链接弹窗 -->
    <el-dialog v-model="dialogVisible" title="超链接信息" width="30%" show-close destroy-on-close>
      <el-form ref="linkFormRef" :model="linkForm" :rules="linkRules" label-width="auto" label-suffix=" :" label-position="top">
        <s-form-item label="超链接名称" prop="name">
          <s-input v-model="linkForm.name"></s-input>
        </s-form-item>
        <s-form-item label="超链接地址" prop="url">
          <s-input v-model="linkForm.url"></s-input>
        </s-form-item>
        <s-form-item label="排序" prop="sortCode">
          <el-slider v-model="linkForm.sortCode" show-input :min="1" />
        </s-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="saveLink">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { SysConfig, menuApi, sysConfigApi } from "@/api";
import { required } from "@/utils/formRules";
import { useDictStore } from "@/stores/modules";
import { SysDictEnum, SysConfigTypeEnum, SysBaseEnum, TenantEnum } from "@/enums";
import { FormInstance } from "element-plus";

const dictStore = useDictStore(); //字典仓库
// 字典类型选项
const statusOptions = dictStore.getDictList(SysDictEnum.COMMON_STATUS);
//多租户选项
const tenantOptions = dictStore.getDictList(SysDictEnum.TENANT_OPTIONS);

const icoType: File.ImageMimeType[] = ["image/x-icon"]; //ico文件类型
const submitLoading = ref(false); //提交按钮loading
const sysConfigFormRef = ref<FormInstance>(); //表单实例
/**  系统配置 */
const sysBaseProps = reactive<SysConfig.SysBaseConfig>({
  SYS_NAME: "",
  SYS_LOGO: "",
  SYS_ICO: "",
  SYS_VERSION: "",
  SYS_COPYRIGHT: "",
  SYS_COPYRIGHT_URL: "",
  SYS_DEFAULT_WORKBENCH_DATA: { shortcut: [] },
  SYS_WEB_STATUS: statusOptions[0].value,
  SYS_WEB_CLOSE_PROMPT: "",
  SYS_TENANT_OPTIONS: TenantEnum.CLOSE,
  SYS_FOOTER_LINKS: []
});

//定义props
const props = defineProps({
  sysConfigs: {
    type: Array as PropType<SysConfig.ConfigInfo[]>,
    required: true,
    default: () => []
  }
});

//监听sysConfigs变化
watch(
  () => props.sysConfigs,
  (newVal: SysConfig.ConfigInfo[]) => {
    //重新赋值
    newVal.forEach((item: SysConfig.ConfigInfo) => {
      //如果是对象类型的属性就转成对象
      if (item.configKey == SysBaseEnum.SYS_DEFAULT_WORKBENCH_DATA) {
        const workBenchData: SysConfig.WorkBenchData = JSON.parse(item.configValue);
        sysBaseProps.SYS_DEFAULT_WORKBENCH_DATA = workBenchData;
      } else if (item.configKey == SysBaseEnum.SYS_FOOTER_LINKS) {
        const footerLinks: SysConfig.FooterLinkProps[] = JSON.parse(item.configValue);
        sysBaseProps.SYS_FOOTER_LINKS = footerLinks;
      } else {
        // 其他属性直接赋值
        (sysBaseProps[item.configKey as keyof SysConfig.SysBaseConfig] as string) = item.configValue;
      }
    });
  }
);

// 表单验证规则
const rules = reactive({
  SYS_NAME: [required("请输入系统名称")],
  SYS_LOGO: [required("请上传系统LOGO")],
  SYS_ICO: [required("请上传系统ICO")],
  SYS_VERSION: [required("请输入系统版本")],
  SYS_COPYRIGHT: [required("请输入系统版权")],
  SYS_COPYRIGHT_URL: [required("请输入系统版权链接")],
  SYS_DEFAULT_WORKBENCH_DATA: [required("请输入系统默认工作台数据")],
  SYS_WEB_STATUS: [required("请选择网站开启访问状态")],
  SYS_WEB_CLOSE_PROMPT: [required("请输入网站关闭提示")]
});

const dialogVisible = ref(false); // 是否显示弹窗
// 超链接临时对象
const linkTmp = ref<SysConfig.FooterLinkProps>({
  name: "",
  url: "",
  sortCode: 99
});
// 是否是添加超链接
const linkFormRef = ref<FormInstance>(); // 超链接表单实例
// 超链接表单参数
const linkForm = reactive<SysConfig.FooterLinkProps>({
  name: "",
  url: "",
  sortCode: 99
});

// 表单验证规则
const linkRules = reactive({
  name: [required("请输入超链接名称")],
  url: [required("请输入超链接地址")],
  sortCode: [required("请输入排序")]
});

/** 删除超链接 */
function handleClose(tag: string) {
  sysBaseProps.SYS_FOOTER_LINKS = sysBaseProps.SYS_FOOTER_LINKS.filter(item => item.name != tag);
}

/** 打开超链接详情 */
function linkDetail(value: SysConfig.FooterLinkProps) {
  linkTmp.value = value;
  linkForm.name = value.name;
  linkForm.url = value.url;
  linkForm.sortCode = value.sortCode;
  dialogVisible.value = true;
}

/** 显示超链接表单 */
function showLinkModel() {
  dialogVisible.value = true;
}

/** 保存超链接 */
function saveLink() {
  linkFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    //判断是编辑还是添加
    if (linkTmp.value.name) {
      //删除旧的值，添加新的值
      sysBaseProps.SYS_FOOTER_LINKS = sysBaseProps.SYS_FOOTER_LINKS.filter(item => item.name != linkTmp.value.name);
    }
    sysBaseProps.SYS_FOOTER_LINKS.push({ ...linkForm });
    //根据sortCode排序,sortCode越小越靠前
    sysBaseProps.SYS_FOOTER_LINKS.sort((a, b) => a.sortCode - b.sortCode);
    //关闭弹窗,恢复默认值
    dialogVisible.value = false;
    linkTmp.value.name = "";
    linkTmp.value.url = "";
    linkTmp.value.sortCode = 99;
    linkFormRef.value?.resetFields();
  });
}

/** 提交表单 */
function onSubmit() {
  sysConfigFormRef.value?.validate(async valid => {
    if (!valid) return; //表单验证失败
    submitLoading.value = true;
    //组装参数
    const param: SysConfig.ConfigInfo[] = Object.entries(sysBaseProps).map(item => {
      return {
        id: 0,
        category: SysConfigTypeEnum.SYS_BASE,
        configKey: item[0],
        //configValue 如果是对象就转成json字符串
        configValue: typeof item[1] === "object" ? JSON.stringify(item[1]) : String(item[1])
      };
    });
    //提交数据
    sysConfigApi.configEditForm(param).finally(() => {
      submitLoading.value = false;
    });
  });
}

/** 重置表单 */
function resetForm() {
  sysConfigFormRef.value?.resetFields();
  linkFormRef.value?.resetFields();
}
</script>

<style lang="scss" scoped></style>
