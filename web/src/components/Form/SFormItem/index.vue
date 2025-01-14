<!-- 
 * @Description: 表单Item组件封装
 * @Author: huguodong 
 * @Date: 2023-12-15 15:38:14
!-->
<template>
  <el-form-item ref="formItemRef" v-bind="$attrs">
    <slot></slot>
    <template #label>
      <el-space :size="3" v-if="tooltip">
        <el-tooltip :effect="tooltip.effect" :placement="tooltip.placement" :raw-content="tooltip.rawContent" :hide-after="tooltip.hideAfter">
          <template #content>
            <!-- 插槽允许父组件传递自定义的 tooltip 内容 -->
            <slot name="tooltip">{{ tooltip.content }}</slot>
          </template>
          <svg-icon icon="uiw:question-circle-o"></svg-icon>
        </el-tooltip>
        {{ $attrs.label }}
      </el-space>
      <el-space :size="3" v-else-if="popover">
        <el-popover :width="popover.width" :placement="popover.placement" :trigger="popover.trigger" :title="popover.title">
          <template #default>
            <!-- 插槽允许父组件传递自定义的 tooltip 内容 -->
            <slot name="popover">{{ popover.content }}</slot>
          </template>
          <template #reference>
            <svg-icon icon="uiw:question-circle-o"></svg-icon>
          </template>
        </el-popover>
        {{ $attrs.label }}
      </el-space>
      <span v-else>{{ $attrs.label }}{{ formContext?.labelSuffix }}</span>
    </template>
  </el-form-item>
</template>

<script setup lang="ts" name="SFormItem">
import { FormItemContext, formContextKey } from "element-plus";

const formContext = inject(formContextKey, undefined); //表单实例
const formItemRef = ref<FormItemContext>(); //表单Item实例

interface Props {
  /*是否显示 tooltip*/
  tooltip?: {
    /*内容*/
    content?: any;
    /*content 中的内容是否作为 HTML 字符串处理*/
    rawContent?: boolean;
    /*tooltip 显示的位置*/
    placement?:
      | "top"
      | "top-start"
      | "top-end"
      | "bottom"
      | "bottom-start"
      | "bottom-end"
      | "left"
      | "left-start"
      | "left-end"
      | "right"
      | "right-start"
      | "right-end";
    /*tooltip 主题*/
    effect?: "dark" | "light";
    /*延迟关闭，单位毫秒*/
    hideAfter?: number;
  };
  /* 是否显示 tooltip*/
  popover?: {
    /*出现位置*/
    placement?:
      | "top"
      | "top-start"
      | "top-end"
      | "bottom"
      | "bottom-start"
      | "bottom-end"
      | "left"
      | "left-start"
      | "left-end"
      | "right"
      | "right-start"
      | "right-end";
    /*标题*/
    title: string;
    /*宽度*/
    width?: string;
    /*触发方式*/
    trigger?: "click" | "focus" | "hover" | "contextmenu";
    /*内容*/
    content?: any;
  };
}

//默认值
withDefaults(defineProps<Props>(), {
  tooltip: undefined,
  popover: undefined
});
</script>

<style lang="scss" scoped></style>
