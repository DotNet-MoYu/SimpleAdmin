# SimpleAdmin Web（管理端）

## 模块简介

`web` 目录是 SimpleAdmin 管理端项目，基于 Vue 3 + Vite + TypeScript 构建，默认作为后台管理系统的前端入口。

## 技术栈

- Vue 3
- Vite
- TypeScript
- Pinia
- Element Plus
- ESLint + Prettier + Stylelint

## 先决条件

- Node.js >= 16（建议使用 Node 20 LTS）
- npm 可用

## 本地开发

在仓库根目录执行：

```bash
cd web
npm install
npm run dev
```

## 质量检查与构建

```bash
cd web
npm run type:check
npm run lint:eslint
npm run build:pro
```

## 常见问题入口

- 项目整体结构、后端联调、移动端说明：请查看根目录 README  
  [../README.md](../README.md)
- 若命令执行失败，请先确认 Node 与依赖版本是否满足要求后重新安装依赖。
