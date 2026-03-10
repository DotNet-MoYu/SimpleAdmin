<div align="center"><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/ikun.png" width="120" height="120" style="margin-bottom: 10px;"/></div>
<div align="center"><strong><span style="font-size: x-large;">SimpleAdmin</span></strong></div>
<div align="center"><h4 align="center">🐔简单之名，非凡之质——稳定、灵活、高效，实力不需夸耀。🐔</h4></div>

<div align="center">

<label>[![AUR](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](https://gitee.com/dotnetmoyu/SimpleAdmin/blob/master/LICENSE)</label>
<label>[![](https://img.shields.io/badge/Author-少林寺驻北固山办事处大神父王喇嘛-orange.svg)](https://gitee.com/huguodong520)</label>
<label>[![](https://img.shields.io/badge/🏀-酝酿时长两年半-orange.svg)](https://gitee.com/huguodong520)</label>
<label>[![](https://img.shields.io/badge/Blog-个人博客-blue.svg)](https://www.cnblogs.com/huguodong/)</label>
<label>[![star](https://gitee.com/dotnetmoyu/SimpleAdmin/badge/star.svg?theme=dark)](https://gitee.com/dotnetmoyu/SimpleAdmin/stargazers)</label>
<label>[![fork](https://gitee.com/dotnetmoyu/SimpleAdmin/badge/fork.svg?theme=dark)](https://gitee.com/dotnetmoyu/SimpleAdmin/members)</label>

</div>

### 如果您觉得有帮助，请点右上角 "Star" 支持一下谢谢

## 🎨 框架介绍

🪶SimpleAdmin 是一个小而美的通用业务型后台管理系统，专为解决开发过程中的痛点难点而生。前端基于 ElementUI + Vue3 + TypeScript，后端基于 .NET6/7/8 + SqlSugar 单例模式。移动端基于 Uniapp + TypeScript + Vue3。采用 RBAC + 多机构的权限管理模式，实现全网最灵活的接口级别数据权限控制。代码注释覆盖率大于 `90%`，非常适合二次开发。

## ✨ 系统特色

### ⛏️ 对于后端

- 启动及运行不会出现拉下来代码跑不起来的情况，自动生成数据库表和种子文件。
- 基于 SqlSugar 单例模式 + CodeFirst + 仓储结构，无需担心作用域问题，直接爽撸。
- 集成国密加密，是国`首个`.NET 内置国密算法且前后端分离快速开发平台。其他说首个的都是吹牛皮的！
- 插件式开发，代码更灵活，架构更清晰，每个业务层都可以是独立的，避免后期功能越来越多导致项目成为屎山。
- 极致缓存，系统快人一步，一些基础配置和用户权限信息都放在了缓存中，用户首次登录后，下次再登录接口耗时实测 `10-30ms`。
- RBAC + 多机构权限 + 动态刷新，修改用户权限后无需重新登录即可刷新用户权限。
- 全网最灵活的接口级别数据范围权限控制，可以指定某个角色/人员某个接口的数据权限范围。
- 后端源码注释覆盖率超过 <font color="#dd0000">90%</font><br/>，每一个方法、每一步都有详细的解释和说明。

### 💻 对于前端

- 基于 Vue3、Vite、TS、Pinia、Element-Plus 等技术栈开发。
- 基于 [GeeKer Admin](https://docs.spicyboy.cn/)，界面清爽美观。
- 配置 ESLint、Prettier、Husky、Commitlint、Lint-staged，规范前端工程代码质量。
- 提供丰富的组件与常用 Hooks 封装，在一定程度上节省造轮子时间。
- 提供多种布局方式切换、主题颜色配置、暗黑/灰色/色弱等模式。
- 丰富的代码注释，新手也能快速上手。

### 📱 对于移动端

- Vue 3 + Vite + pnpm + esbuild，启动和构建更快。
- UnoCSS 原子化样式能力，灵活且高性能。
- pinia + pinia-plugin-persistedstate，统一状态管理。
- uni.request 请求封装 + uni-mini-router 路由拦截，便于快速开发。
- 多环境配置分离，支持不同部署场景。

## 🚀 快速开始

### 环境要求

- .NET SDK 6.0/7.0/8.0（推荐 8.0）
- Node.js（推荐 20 LTS，至少满足各子项目要求）
- npm（用于 `web`）
- pnpm（用于 `uniapp`）

### 后端（api）

```bash
cd api/SimpleAdmin
dotnet restore
dotnet build SimpleAdmin.sln
dotnet run --project SimpleAdmin.Web.Entry
```

### 管理端（web）

```bash
cd web
npm install
npm run dev
```

常用检查命令：

```bash
npm run type:check
npm run lint:eslint
npm run build:pro
```

### 移动端（uniapp）

```bash
cd uniapp
pnpm install
pnpm dev:h5
```

常用检查命令：

```bash
pnpm type-check
pnpm lint
pnpm build:h5
```

## 🧩 仓库结构

| 路径 | 模块说明 | 典型入口 |
| --- | --- | --- |
| `api/SimpleAdmin` | .NET 后端解决方案，含应用层、系统层、核心层等 | `SimpleAdmin.sln` / `SimpleAdmin.Web.Entry` |
| `web` | Vue 3 + Vite 管理端 | `web/src` |
| `uniapp` | uni-app 移动端 | `uniapp/src` |
| `images` | README 展示图片等静态资源 | `images/*` |
| `nginx` | 部署相关配置 | `nginx/*` |

## 🎈 相关连接

预览地址： [点击查看](http://153.101.199.83:12802)

更新日志： [点击查看](https://gitee.com/dotnetmoyu/SimpleAdmin/commits/master)

文档地址（旧）： [https://www.cnblogs.com/huguodong/p/17021233.html](https://www.cnblogs.com/huguodong/p/17021233.html)

文档地址（新）： [http://118.190.201.181/](http://118.190.201.181/)

## 📣 推荐服务（广告）

> 个人使用推荐，非强制选项，请根据自身需求自行甄别与评估风险。

- 性价比机场（单月 12 元 / 128G，纯净 IP，畅用 AI）：[https://www.zou666.net/#/register?code=hfkhGLG5](https://www.zou666.net/#/register?code=hfkhGLG5)
- 性价比 AI 中转站（纯血 Codex 中转，支持 GPT-5.4，无需梯子即可访问）：请联系作者
- 腾讯云轻量服务器推荐（适合部署后台管理系统、Web 服务和轻量业务应用）：[https://curl.qcloud.com/Uw9jZRJr](https://curl.qcloud.com/Uw9jZRJr)
- 腾讯云云产品优惠推荐（云服务器、云数据库、COS、CDN、短信等云产品特惠热卖中）：[https://curl.qcloud.com/Im4ILEeM](https://curl.qcloud.com/Im4ILEeM)

## 💵 适用场景

- 搭建企业内部后台管理框架
- 接外包项目/私活
- 个人学习 .NET 搭建 Web 框架
- 搭建商用项目

## 👨 适用人群

- 对系统 UI 美观有要求
- 对学习新技术有兴趣，愿意接受新的技术
- .NET 新手，想找一个框架学习、增加知识
- 对技术有要求，喜欢高质量代码
- 想要花更多时间陪陪家人，出去走走
- 不想在各种低级 bug 上浪费时间

## 🍔 分支说明

- `master`：正式稳定版本，具体版本升级内容看更新标签
- `dev`：开发分支（代码可能随时会推，不保证运行和使用）
- `js`：JS 版本分支，适配于 JS 版本前端的代码

## 🚩 桌面端展示

<table>
    <tr>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/login.png"/></td>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/index.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/settings.png"/></td>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/menu.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/role.png"/></td>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/user.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/icon.png"/></td>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/choose.png"/></td>
    </tr>
</table>

## 🚩 移动端展示

<table>
    <tr>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/uniapp_login.png"/></td>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/uniapp_index.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/uniapp_work.png"/></td>
        <td><img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/uniapp_mine.png"/></td>
    </tr>
</table>

## 🎓 设计初衷和理念

一个卓越的后台管理框架是开发者提升效率、降本增质的关键工具。我曾经尝试在 Git 上搜索开源后台管理系统，希望能用它们作为公司项目的起点。可惜的是，很多开源系统难以满足我的期待：要么缺失清晰文档指导，要么代码质量让人担忧，要么扩展性有限，要么功能过于庞大而失去焦点。

正因为这样的挑战，我下定决心从无到有打造一个全新的后台管理框架，这就是 `SimpleAdmin` 的诞生背景。SimpleAdmin 聚焦业务需求，核心理念是 `精简至上`。在功能与实用性之间寻找平衡，只保留关键且有效的能力，并把它们做到更好。

SimpleAdmin 不只是工具，更是一种承诺：确保每位开发者都能无障碍掌握并深入定制，使框架适应项目，而不是项目迁就框架。它以代码表达我们对“简约而不简单”的追求，成为真正为业务服务的可靠盟友。

## 🔖 友情链接

- 👉 Geeker Admin：[https://docs.spicyboy.cn/](https://docs.spicyboy.cn/)
- 👉 MoYu：[https://gitee.com/dotnetmoyu/MoYu](https://gitee.com/dotnetmoyu/MoYu)
- 👉 SqlSugar：[https://www.donet5.com/Doc/1/1180](https://www.donet5.com/Doc/1/1180)
- 👉 NewLife：[https://www.newlifex.com/](https://www.newlifex.com/)
- 👉 IdGenerator：[https://github.com/yitter/idgenerator](https://github.com/yitter/idgenerator)
- 👉 Masuit.Tools：[https://gitee.com/masuit/Masuit.Tools](https://gitee.com/masuit/Masuit.Tools)
- 👉 Emqx：[https://www.emqx.com/zh](https://www.emqx.com/zh)
- 👉 MagicodesIE：[https://github.com/dotnetcore/Magicodes.IE](https://github.com/dotnetcore/Magicodes.IE)

[![驰骋工作流](images/hz1.png)](https://ccflow.org/index.html?frm=simple)

## 👏 鸣谢 👏

- 感谢 JetBrains 提供的免费开源 License：

<p>
<img src="https://images.gitee.com/uploads/images/2020/0406/220236_f5275c90_5531506.png">
</p>

## 🤌 赞助

```text
如果对您有帮助，请点击右上角⭐Star关注或扫码捐赠，感谢支持开源！捐赠金额≥99元即可加入内部交流群一起讨论学习，捐赠之后加q531035580即可。
```

<img src="https://cdn.jsdelivr.net/gh/huguodong/doc-images@main/simple/zanshang.jpg"/>

## 💾 版权声明 💾

- 请不要删除和修改根目录下的 LICENSE 文件。
- 请不要删除和修改 SimpleAdmin 源码头部的版权声明。
- 分发源码时候，请注明软件出处。
- 基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
- 请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
- 任何基于本软件而产生的一切法律纠纷和责任，均与作者无关。
