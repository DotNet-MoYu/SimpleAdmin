# lime-svg 矢量图标
一款基于UTS实现的原生矢量图标插件，支持uniapp和uniappx。该插件提供了两种渲染机制：原生插件和webview，可以根据需求选择合适的渲染方式。支持加载本地、网络、源文本和base64格式的SVG图标，并且可以自定义图标颜色。

## 文档链接
📚 组件详细文档请访问以下站点：
- [矢量图标文档 - 站点1](https://limex.qcoon.cn/components/svg.html)
- [矢量图标文档 - 站点2](https://limeui.netlify.app/components/svg.html)
- [矢量图标文档 - 站点3](https://limeui.familyzone.top/components/svg.html)

## 安装方法
1. 在uni-app插件市场中搜索并导入`lime-svg`
2. 由于普通授权版无法自定义基座，如需使用请购买源码版
3. 在页面中使用`l-svg`组件

## 代码演示

### 渲染机制说明
安卓和iOS提供基于`原生插件`和`webview`两种渲染机制：
- 使用`原生插件`需要`自定义基座`
- 原生插件实现不支持动画，如果需要动画请选择`webview`

### 路径加载方式
通过设置`src`来加载svg图标，支持多种加载方式：

```html
<!-- 本地文件加载 -->
<l-svg style="width: 150rpx;height: 150rpx;" src="/static/svg/a.svg"></l-svg>

<!-- 网络文件加载 -->
<l-svg style="width: 150rpx;height: 150rpx;" src="https://www.xmplus.cn/uploads/images/20221228/b9e9d45054ab5795992a1e92584a278b.svg"></l-svg>

<!-- SVG源文本加载 -->
<l-svg style="width: 150rpx;height: 150rpx;" src='<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M6 15h1.5V9H5v1.5h1zm2.5 0H13V9H8.5zm1.5-1.5v-3h1.5v3zm4 1.5h1.5v-2.25L17.25 15H19l-2.25-3L19 9h-1.75l-1.75 2.25V9H14zM3 21V3h18v18z"/></svg>'></l-svg>

<!-- Base64编码加载 -->
<l-svg style="width: 150rpx;height: 150rpx;" src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxZW0iIGhlaWdodD0iMWVtIiB2aWV3Qm94PSIwIDAgMjQgMjQiPjxwYXRoIGZpbGw9ImN1cnJlbnRDb2xvciIgZD0iTTYgMTVoMS41VjlINXYxLjVoMXptMi41IDBIMTNWOUg4LjV6bTEuNS0xLjV2LTNoMS41djN6bTQgMS41aDEuNXYtMi4yNUwxNy4yNSAxNUgxOWwtMi4yNS0zTDE5IDloLTEuNzVsLTEuNzUgMi4yNVY5SDE0ek0zIDIxVjNoMTh2MTh6Ii8+PC9zdmc+"></l-svg>
```

### 颜色设置
通过设置`color`来改变svg图标颜色，注意：只支持纯色图标，多色图标无效。

```html
<!-- 设置红色 -->
<l-svg 
  style="width: 150rpx;height: 150rpx;" 
  color="red" 
  src='<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M6 15h1.5V9H5v1.5h1zm2.5 0H13V9H8.5zm1.5-1.5v-3h1.5v3zm4 1.5h1.5v-2.25L17.25 15H19l-2.25-3L19 9h-1.75l-1.75 2.25V9H14zM3 21V3h18v18z"/></svg>'
></l-svg>

<!-- 设置红色（Base64） -->
<l-svg 
  style="width: 150rpx;height: 150rpx;" 
  color="red" 
  src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxZW0iIGhlaWdodD0iMWVtIiB2aWV3Qm94PSIwIDAgMjQgMjQiPjxwYXRoIGZpbGw9ImN1cnJlbnRDb2xvciIgZD0iTTYgMTVoMS41VjlINXYxLjVoMXptMi41IDBIMTNWOUg4LjV6bTEuNS0xLjV2LTNoMS41djN6bTQgMS41aDEuNXYtMi4yNUwxNy4yNSAxNUgxOWwtMi4yNS0zTDE5IDloLTEuNzVsLTEuNzUgMi4yNVY5SDE0ek0zIDIxVjNoMTh2MTh6Ii8+PC9zdmc+"
></l-svg>
```

### WebView渲染
通过设置`:web="true"`使用`webview`渲染，支持动画效果。

```html
<!-- WebView渲染SVG源文本 -->
<l-svg 
  style="width: 150rpx;height: 150rpx;" 
  :web="true" 
  src='<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M6 15h1.5V9H5v1.5h1zm2.5 0H13V9H8.5zm1.5-1.5v-3h1.5v3zm4 1.5h1.5v-2.25L17.25 15H19l-2.25-3L19 9h-1.75l-1.75 2.25V9H14zM3 21V3h18v18z"/></svg>'
></l-svg>

<!-- WebView渲染Base64 -->
<l-svg 
  style="width: 150rpx;height: 150rpx;" 
  :web="true" 
  src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIxZW0iIGhlaWdodD0iMWVtIiB2aWV3Qm94PSIwIDAgMjQgMjQiPjxwYXRoIGZpbGw9ImN1cnJlbnRDb2xvciIgZD0iTTYgMTVoMS41VjlINXYxLjVoMXptMi41IDBIMTNWOUg4LjV6bTEuNS0xLjV2LTNoMS41djN6bTQgMS41aDEuNXYtMi4yNUwxNy4yNSAxNUgxOWwtMi4yNS0zTDE5IDloLTEuNzVsLTEuNzUgMi4yNVY5SDE0ek0zIDIxVjNoMTh2MTh6Ii8+PC9zdmc+"
></l-svg>
```

## 快速预览
导入插件后，可以直接使用以下标签查看演示效果：

```html
<!-- 代码位于 uni_modules/lime-svg/components/lime-svg -->
<lime-svg />
```

## 插件标签说明
- 默认 `l-svg` 为组件标签
- 默认 `lime-svg` 为演示标签

## Vue2使用说明
插件使用了`composition-api`，如果你希望在Vue2中使用，请按官方教程[vue-composition-api](https://uniapp.dcloud.net.cn/tutorial/vue-composition-api.html)配置。

关键代码是在main.js中的Vue2部分添加以下代码：

```js
// vue2
import Vue from 'vue'
import VueCompositionAPI from '@vue/composition-api'
Vue.use(VueCompositionAPI)
```

## API文档

### Props

| 属性名 | 说明 | 类型 | 默认值 |
| --- | --- | --- | --- |
| src | SVG图标的源，支持本地路径、网络URL、SVG源文本和Base64 | <em>string</em> | - |
| color | 图标颜色，仅对纯色图标有效 | <em>string</em> | - |
| web | 是否使用WebView渲染，支持动画效果 | <em>boolean</em> | `false` |
| width | 图标宽度 | <em>string \| number</em> | - |
| height | 图标高度 | <em>string \| number</em> | - |

### Events

| 事件名 | 说明 | 回调参数 |
| --- | --- | --- |
| click | 点击图标时触发 | event: Event |
| load | 图标加载完成时触发 | - |
| error | 图标加载失败时触发 | error: Error |

## 支持与赞赏

如果你觉得本插件解决了你的问题，可以考虑支持作者：

| 支付宝赞助 | 微信赞助 |
|------------|------------|
| ![](https://testingcf.jsdelivr.net/gh/liangei/image@1.9/alipay.png) | ![](https://testingcf.jsdelivr.net/gh/liangei/image@1.9/wpay.png) |