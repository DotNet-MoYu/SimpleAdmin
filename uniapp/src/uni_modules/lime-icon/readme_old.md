# lime-icon 图标
图标组件，方便快捷地使用[iconify](https://iconify.design/)图标集合，提供超过150,000个开源矢量图标。支持自定义颜色、大小、前缀等属性，还可以使用自定义图标和图标URL。

> 注意：插件依赖的`lime-svg`为收费插件，若不需要svg功能，删除svg插件即可。

## 文档链接
📚 组件详细文档请访问以下站点：
- [图标文档 - 站点1](https://limex.qcoon.cn/components/icon.html)
- [图标文档 - 站点2](https://limeui.netlify.app/components/icon.html)
- [图标文档 - 站点3](https://limeui.familyzone.top/components/icon.html)

## 安装方法
1. 在uni-app插件市场中搜索并导入`lime-icon`
2. 导入后可能需要重新编译项目
3. 在页面中使用`l-icon`组件（组件）或`lime-icon`（演示）

::: tip 注意🔔 
本插件依赖的[【lime-svg】](https://ext.dcloud.net.cn/plugin?id=18519)是原生插件，如果购买(收费为6元)则需要自定义基座，才能使用，
若不需要删除即可
:::

## 代码演示

### 基础使用
使用`name`属性指定要显示的图标。👉️[【全部图标】](#全部图标)

```html
<l-icon name="circle" />
```

### 使用Iconify
到 [icones](https://icones.js.org/) 网站找到需要的图标,通过 `name` 属性来指定需要使用的图标

```html
<l-icon name="ri:account-box-fill" />
<l-icon name="icon-park-outline:acoustic" />
```

![](https://img-cdn-tx.dcloud.net.cn/stream/plugin_screens/263cfd20-39e6-11ee-b4f0-9bc760224a38_1.png?1735701321)
![](https://img-cdn-tx.dcloud.net.cn/stream/plugin_screens/263cfd20-39e6-11ee-b4f0-9bc760224a38_2.png?1735701324)


### 使用图标URL
```html
<l-icon name="https://fastly.jsdelivr.net/npm/@vant/assets/icon-demo.png"></l-icon>
```

### 图标颜色
通过 `color` 属性来设置图标的颜色。

```html
<l-icon name="ri:aliens-fill" color="#1989fa" />
<l-icon name="icon-park-outline:acoustic" color="#ee0a24" />
```

### 图标大小

通过 `size` 属性来设置图标的尺寸大小，可以指定任意 CSS 单位。

```html
<!-- 不指定单位，默认使用 px -->
<l-icon name="ri:aliens-fill" size="40" />
<!-- 指定使用 rpx 单位 -->
<l-icon name="ri:aliens-fill" size="34rpx" />
```


### 自定义图标
通过`prefix`设置iconfot图标类，通过`name`传入`Unicode`字符
```html
<l-icon size="30px" prefix="keyicon" :name="`\uE6EF`" color="blue"></l-icon>
```
```css
@font-face {
	font-family: keyicon;
	src: url('https://at.alicdn.com/t/c/font_4741157_ul7wcp52yys.ttf');
}
.keyicon {
	font-family: keyicon;
}
```

## 私有化iconify
默认会使用`iconify`的API，如果你想私有化可按以下步骤来
### 第一步 安装

```cmd
yarn add @iconify/json @iconify/tools @iconify/utils
```
### 第二步 配置
- 需要在根目录新建一个`lime-icons.config.js`文件

```
// 在根目录新建一个lime-icons.config.js文件
// lime-icons.config.js
module.exports = {
	// 输入的文件目录，自有的SVG，如果没有则不需要
	input: {
		prefix: "my-icons",
		dir: '/static/svg',
	},
	// 输出的配置
	output: {
		// 输出的文件目录
		dir: '/static/icons',
		// 输出的文件的格式，如果是JSON则是一个图标合集
		// file: 'icons.json',
		// 如果是SVG则是每个图标做为单独的文件
		file: '*.svg',
	},
	// 指定使用的图标
	icons: [
		'el:address-book', 
		'uil:12-plus',
		'icon-park-outline:abdominal',
		'icon-park-outline:acoustic'
	]
}
```
在终端执行脚本
```
node ./uni_modules/lime-icon/generate-icons.js
```

### ~~2、自动引入~~
~~如果使用的是`vue3`，通过配置 `vite.config.js` 达到自动引入~~
这个方法作废，因有些图标是动态的，在编译阶段不知道图标的名称无法捕获
```js
import uni from '@dcloudio/vite-plugin-uni';
import limeIcon from './uni_modules/lime-icon/vite-plugin';
import path from 'path'
export default defineConfig({
    plugins: [uni(), limeIcon({
        // 输出的配置
        output: {
            // 输出的文件目录
            dir: path.join(__dirname, '/static/icons'),
            // 输出的文件的格式，如果是JSON则是生成一个图标合集， 例如： /static/icons/icons.json
            // file: 'icons.json',
            // 如果是SVG则是每个图标做为单独的文件 例如： /static/icons/xx/xxx.svg
            file: '*.svg',
        },
        // 可选
        icons: []
    })]
})
```



### 第三步 挂载图标地址

>  注意：如果使用了`iconify` 的API, 小程序需要去公众平台设置下载白名单 `https://api.iconify.design`
```js
// main.js | main.ts | main.uts
// 配置svg指定路径，后期可上传到后端，不占用本地空间，如果使用的是`iconify`也可以不配置这一步
import {limeIcons} from '@/uni_modules/lime-icon'

// 第一个参数是icon host地址，没有则填null
// 第二个参数是icons json合集，没有则填null
// app.use(limeIcons, null, null)

// 示例1 配置icons地址
app.use(limeIcons, 'https://xxx.cn/static/icons', null)

// 示例2 配置icons集合json
import icons from './static/icons/icons.json'
app.use(limeIcons, null, icons)
```

## 快速预览
导入插件后，可以直接使用以下标签查看演示效果：

```html
<!-- 代码位于 uni_modules/lime-icon/components/lime-icon -->
<lime-icon />
```

## 插件标签说明
- `l-icon`: 组件标签，用于实际开发中
- `lime-icon`: 演示标签，用于查看示例效果
## Vue2使用说明
本插件使用了`composition-api`，如需在Vue2项目中使用，请按照[官方教程](https://uniapp.dcloud.net.cn/tutorial/vue-composition-api.html)配置。

关键配置代码（在main.js中添加）：

```js
// vue2
import Vue from 'vue'
import VueCompositionAPI from '@vue/composition-api'

// 配置svg指定路径，后期可上传到后端，不占用本地空间，如果使用的是`iconify`也可以不配置这一步
import {limeIcons} from '@/uni_modules/lime-icon'

Vue.use(VueCompositionAPI)

// 示例1 配置icons地址
Vue.use(limeIcons, ['https://xxx.cn/static/icons', null])

// 示例2 配置icons集合json
import icons from './static/icons/icons.json'
Vue.use(limeIcons, [null, icons])

```



## API

### Props

| 参数                       | 说明                                                         | 类型             | 默认值       |
| --------------------------| ------------------------------------------------------------ | ---------------- | ------------ |
| name                      | 图标名称                                                      | <em>string</em>  | ``     |
| color                     | 颜色                                   | <em>string</em>  | ``     |
| size                     | 尺寸                         | <em>string</em>  | `square`     |
| prefix                   | 字体图标前缀                                 | <em>string</em>  | ``     |
| inherit                  | 是否继承颜色                          | <em>boolean</em>  | `true`     |
| web                  | 原生`app(nvue,uvue)`是否使用web渲染                          | <em>boolean</em>  | `false`     |

### Events
| 参数                       | 说明                                                         | 参数             | 
| --------------------------| ------------------------------------------------------------ | ---------------- |
| click              		| 点击  |  | 


## 打赏

如果你觉得本插件，解决了你的问题，赠人玫瑰，手留余香。  
![](https://testingcf.jsdelivr.net/gh/liangei/image@1.9/alipay.png)
![](https://testingcf.jsdelivr.net/gh/liangei/image@1.9/wpay.png)