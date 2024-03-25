<div align="center"><img  src="https://cdn.52moyu.net/logo.png" width="120" height="120" style="margin-bottom: 10px;"/></div>
<div align="center"><strong><span style="font-size: x-large;">SimpleAdmin</span></strong></div>
<div align="center"><h4 align="center">🐔没有花里胡哨，只有简单、稳定、灵活、高效🐔</h4></div>

<div align="center">

<label>[![AUR](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](https://gitee.com/dotnetmoyu/SimpleAdmin/blob/master/LICENSE)</label> <label>[![](https://img.shields.io/badge/Author-少林寺驻北固山办事处大神父王喇嘛-orange.svg)](https://gitee.com/huguodong520)</label> <label>[![](https://img.shields.io/badge/🏀-酝酿时长两年半-orange.svg)](https://gitee.com/huguodong520)</label> <label>[![](https://img.shields.io/badge/Blog-个人博客-blue.svg)](https://www.cnblogs.com/huguodong/)</label> <label>[![star](https://gitee.com/dotnetmoyu/SimpleAdmin/badge/star.svg?theme=dark)](https://gitee.com/dotnetmoyu/SimpleAdmin/stargazers)</label> <label>[![fork](https://gitee.com/dotnetmoyu/SimpleAdmin/badge/fork.svg?theme=dark)](https://gitee.com/dotnetmoyu/SimpleAdmin/members)</label>

</div>

### 如果您觉得有帮助，请点右上角 "Star" 支持一下谢谢

## 🎨 框架介绍

🪶SimpleAdmin 是一个小而美的通用业务型后台管理系统，专为解决开发过程中的痛点难点而生。前端基于 ElementUI+Vue3+TypeScript,后端基于.NET6/7/8+SqlSugar 单例模式。采用 RBAC+多机构的权限管理模式，实现全网最灵活的接口级别数据权限控制。代码注释覆盖率大于`90%`,非常适合二次开发。将日常开发中的业务场景和框架紧密结合，并坚持以人为本,以业务为中心,做到开箱即用,代码简洁、易扩展，注释详细，文档齐全，让你的开发少走弯路。

## 🎓 设计初衷和理念

一个卓越的后台管理框架是开发者提升效率、降本增质的关键工具。我曾经尝试在 Git 上搜索开源的后台管理系统，希望能用它们作为公司项目的起点。可惜的是，我发现很多开源系统难以满足我的期待：要么缺失清晰的文档指导，要么代码质量让人担忧，要么系统的扩展性极为有限，要么系统功能过于庞大而失去焦点，积重难返。
正因为这样的挑战，我下定决心，要从无到有打造一个全新的后台管理框架,这就是`SimpleAdmin`的诞生背景。SimpleAdmin 是一个聚焦业务需求的管理系统，它的核心理念是`精简至上`。我们深知，在功能和实用性之间找到完美的平衡点至关重要，因此在 SimpleAdmin 中，我们只保留了那些最关键的、行之有效的功能，并且全力以赴将它们做得更完美。
SimpleAdmin 不只是工具，它更是一种承诺：确保每位开发者都能毫无障碍地掌握、进而深入定制，从而使得框架适应项目，而非让项目被迫适应框架。一个优秀的框架，应该是那样自解释的优雅，同时又能轻松应对二次开发的需求。
SimpleAdmin 更像是一种信念，它以代码的形式表达了我们对简约而不简单的追求。唯有包含这种哲学的框架，才算得上是有灵魂，有价值的。它不只是一个平台或者框架，而是一个为业务精准打造的解决方案，一个始终在为你省时、省心和省力的可靠盟友。

## ✨ 系统特色

### ⛏️ 对于后端

- 启动及运行,不会出现拉下来代码跑不起来的情况，自动生成数据库表和种子文件。
- 基于 SqlSugar 单例模式+CodeFirst+仓储的结构，无需担心作用域问题，直接爽撸。
- 插件式开发，代码更灵活，架构更清晰,每个业务层都可以是独立的，避免后期功能越来越多导致项目成为屎山。
- 极致缓存,系统快人一步,一些基础配置和用户权限信息都放在了缓存中，用户首次登录后，下次再登录接口耗时实测`10-30ms`。
- RBAC+多机构的权限+动态刷新,修改用户权限后无需重新登录即可刷新用户权限。
- 全网最灵活的接口级别数据范围权限控制，可以指定某个角色/人员的某个接口的数据权限范围。
- 后端源码注释覆盖率超过<font color="#dd0000">90%</font><br />,每一个方法,每一步都有详细的解释和说明，根据注释和文档也能轻易看懂和二次开发,不用每天在群里问一些非常基础的问题，节约了大量的开发时间。

### 📱 对于前端

- 基于 Vue3.3、Vite4、TS、Pinia、Element-Plus 等最新技术栈开发
- 基于[GeeKer Admin](https://docs.spicyboy.cn/),界面比同类型 UI 更清爽又美观。
- 配置 Eslint、Prettier、Husky、Commitlint、Lint-staged 规范前端工程代码规范
- 提供丰富的组件、常用 Hooks 封装，在一定程度上节省你造轮子的时间。
- 提供多种布局方式切换，主题颜色配置，暗黑、灰色、色弱等模式。
- 丰富的代码注释,每一个页面都有解释说明,新手也能快速上手。

## 💵 适用场景

- 搭建企业内部后台管理框架
- 接外包项目/私活
- 个人学习.Net 搭建 Web 框架
- 搭建商用项目

## 👨 适用人群

- 对系统 UI 美观有要求
- 对学习新技术有兴趣，愿意接受新的技术
- .NET 新手,想找一个框架学习,增加知识
- 对技术有要求,喜欢高质量代码
- 想要花更多时间陪陪家人,出去走走
- 不想在各种低级 bug 上浪费时间

## 🍔 分支说明

- master
  正式稳定版本，具体版本升级内容看更新标签

- dev
  开发的分支（代码可能随时会推，不保证运行和使用）

- js
  js 版本的分支,适配于 js 版本前端的代码

## 🚩 效果展示

<table>
    <tr>
        <td><img src="https://cdn.52moyu.net/login.png"/></td>
        <td><img src="https://cdn.52moyu.net/index.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.52moyu.net/settings.png"/></td>
        <td><img src="https://cdn.52moyu.net/menu.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.52moyu.net/role.png"/></td>
        <td><img src="https://cdn.52moyu.net/user.png"/></td>
    </tr>
    <tr>
        <td><img src="https://cdn.52moyu.net/icon.png"/></td>
        <td><img src="https://cdn.52moyu.net/choose.png"/></td>
    </tr>
</table>

## 🎈 相关连接

预览地址： [点击查看](http://153.101.199.83:12802)

更新日志：[点击查看](https://gitee.com/dotnetmoyu/SimpleAdmin/commits/master)

文档地址(旧)：[https://www.cnblogs.com/huguodong/p/17021233.html](https://www.cnblogs.com/huguodong/p/17021233.html)

文档地址(新)：[https://dotnetmoyu.gitee.io/simpleadmin-doc/](https://dotnetmoyu.gitee.io/simpleadmin-doc/)

## 🔖 友情链接

- 👉 Geeker Admin：[https://docs.spicyboy.cn/](https://docs.spicyboy.cn/)
- 👉 MoYu：[https://gitee.com/dotnetmoyu/MoYu](https://gitee.com/dotnetmoyu/MoYu)
- 👉 SqlSugar：[https://www.donet5.com/Doc/1/1180](https://www.donet5.com/Doc/1/1180)
- 👉 NewLife：[https://www.newlifex.com/](https://www.newlifex.com/)
- 👉 IdGenerator：[https://github.com/yitter/idgenerator](https://github.com/yitter/idgenerator)
- 👉 Masuit.Tools：[https://gitee.com/masuit/Masuit.Tools](https://gitee.com/masuit/Masuit.Tools)
- 👉 Emqx：[https://www.emqx.com/zh](https://www.emqx.com/zh)
- 👉 MagicodesIE: [https://github.com/dotnetcore/Magicodes.IE](https://github.com/dotnetcore/Magicodes.IE)

## 👏 鸣谢 👏

- 感谢 JetBrains 提供的免费开源 License：

<p>
<img src="https://images.gitee.com/uploads/images/2020/0406/220236_f5275c90_5531506.png" >
</p>

## 🤌 赞助

```
如果对您有帮助，请点击右上角⭐Star关注或扫码捐赠，感谢支持开源！捐赠金额≥99元即可加入内部交流群一起讨论学习，捐赠之后加q531035580即可。
```

<img src="https://cdn.52moyu.net/zanshang.png"/>

## 💾 版权声明 💾

- 请不要删除和修改根目录下的 LICENSE 文件。
- 请不要删除和修改 SimpleAdmin 源码头部的版权声明。
- 分发源码时候，请注明软件出处。
- 基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
- 请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为。
- 任何基于本软件而产生的一切法律纠纷和责任，均于作者无关。
