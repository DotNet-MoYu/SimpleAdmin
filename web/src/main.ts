import { createApp } from "vue";
import App from "./App.vue";
// reset style sheet
import "@/styles/reset.scss";
// CSS common style sheet
import "@/styles/common.scss";
// iconfont css
import "@/assets/iconfont/iconfont.scss";
// iconfontPlus css
import "@/assets/iconfontPlus/iconfont.scss";
// font css
import "@/assets/fonts/font.scss";
// element css
import "element-plus/dist/index.css";
// element dark css
import "element-plus/theme-chalk/dark/css-vars.css";
// custom element dark css
import "@/styles/element-dark.scss";
// custom element css
import "@/styles/element.scss";
// svg icons
import "virtual:svg-icons-register";
// element plus
import ElementPlus from "element-plus";
// element icons
import * as Icons from "@element-plus/icons-vue";
// custom directives
import directives from "@/directives/index";
// vue Router
import router from "@/routers";
// pinia store
import pinia from "@/stores";
// errorHandler
import errorHandler from "@/utils/errorHandler";
// uno.css
import "virtual:uno.css";
// highlight 的样式，依赖包，组件
import "highlight.js/styles/atom-one-dark.css";
import hljsCommon from "highlight.js/lib/common";
import hljsVuePlugin from "@highlightjs/vue-plugin";

const app = createApp(App);

app.config.errorHandler = errorHandler;

// register the element Icons component
Object.keys(Icons).forEach(key => {
  app.component(key, Icons[key as keyof typeof Icons]);
});

// 注意：解决Vue使用highlight.js build打包发布后样式消失问题，原因是webpack在打包的时候没有把未被使用的代码打包进去，因此，在此处引用一下，看似无意义实则有用
hljsCommon.highlightAuto("<h1>Highlight.js has been registered successfully!</h1>").value;

app.use(ElementPlus).use(directives).use(router).use(pinia).use(hljsVuePlugin).mount("#app");
