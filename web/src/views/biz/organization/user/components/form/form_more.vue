<!-- 
 * @Description: 更多信息
 * @Author: huguodong 
 * @Date: 2023-12-15 15:45:55
!-->
<template>
  <el-tab-pane label="更多信息" name="more">
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="民族" prop="nation">
          <s-select v-model="userInfo.nation" :options="nationOptions"></s-select>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="籍贯" prop="nativePlace">
          <s-input v-model="userInfo.nativePlace"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="家庭住址" prop="homeAddress">
          <s-input v-model="userInfo.homeAddress" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }"></s-input>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="通信地址" prop="mailingAddress">
          <s-input v-model="userInfo.mailingAddress" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="证件类型" prop="nation">
          <s-select v-model="userInfo.idCardType" :options="cardTypeOptions"></s-select>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="证件号码" prop="idCardNumber">
          <s-input v-model="userInfo.idCardNumber"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="文化程度" prop="cultureLevel">
          <s-select v-model="userInfo.cultureLevel" :options="cultureLevelOptions"></s-select>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="政治面貌" prop="politicalOutlook">
          <s-input v-model="userInfo.politicalOutlook"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="毕业学校" prop="college">
          <s-input v-model="userInfo.college"></s-input>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="学历" prop="education">
          <s-input v-model="userInfo.education"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="学制" prop="eduLength">
          <s-input v-model="userInfo.eduLength"></s-input>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="学位" prop="degree">
          <s-input v-model="userInfo.degree"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="家庭电话" prop="homeTel">
          <s-input v-model="userInfo.homeTel"></s-input>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="办公电话" prop="officeTel">
          <s-input v-model="userInfo.officeTel"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="紧急联系人" prop="emergencyContact">
          <s-input v-model="userInfo.emergencyContact"></s-input>
        </s-form-item>
      </el-col>
      <el-col :span="12">
        <s-form-item label="紧急联系电话" prop="emergencyPhone">
          <s-input v-model="userInfo.emergencyPhone"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
    <el-row :gutter="16">
      <el-col :span="12">
        <s-form-item label="紧急联系人地址" prop="emergencyAddress">
          <s-input v-model="userInfo.emergencyAddress"></s-input>
        </s-form-item>
      </el-col>
    </el-row>
  </el-tab-pane>
</template>

<script setup lang="ts">
import { SysUser } from "@/api";
import { useDictStore } from "@/stores/modules";
import { SysDictEnum } from "@/enums";

const dictStore = useDictStore(); //字典仓库
// props
interface FormProps {
  modelValue: Partial<SysUser.SysUserInfo>;
}
const emit = defineEmits(["update:modelValue"]); //定义emit
const props = defineProps<FormProps>(); //定义props
// 人员信息
const userInfo = computed({
  get: () => props.modelValue,
  set: val => emit("update:modelValue", val)
});

// 通用状态选项
const nationOptions = dictStore.getDictList(SysDictEnum.NATION);
// 证件类型选项
const cardTypeOptions = dictStore.getDictList(SysDictEnum.IDCARD_TYPE);
//文化程度选项
const cultureLevelOptions = dictStore.getDictList(SysDictEnum.CULTURE_LEVEL);
onMounted(() => {
  // 初始化默认值
  // userInfo.value.nation = userInfo.value.nation ? userInfo.value.nation : nationOptions[0].value;
  userInfo.value.idCardType = userInfo.value.idCardType ? userInfo.value.idCardType : cardTypeOptions[0].value;
});
</script>

<style lang="scss" scoped></style>
