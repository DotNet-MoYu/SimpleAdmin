# SimpleAdmin API（.NET 后端）

## 模块简介

`api` 目录承载 SimpleAdmin 后端服务，核心方案位于 `api/SimpleAdmin/SimpleAdmin.sln`。项目采用分层架构，包含应用层、系统层、核心能力层与 Web 入口层，适合企业后台与业务系统二次开发。

## 技术栈

- .NET 6/7/8
- SqlSugar（CodeFirst + 仓储）
- RBAC + 多机构权限模型
- Web API（前后端分离）

## 先决条件

- 已安装 .NET SDK（推荐 .NET 8）

## 本地开发

在仓库根目录执行：

```bash
cd api/SimpleAdmin
dotnet restore
dotnet build SimpleAdmin.sln
dotnet run --project SimpleAdmin.Web.Entry
```

## 常用命令

```bash
cd api/SimpleAdmin
dotnet restore
dotnet build SimpleAdmin.sln
```

仅运行启动项目：

```bash
cd api/SimpleAdmin
dotnet run --project SimpleAdmin.Web.Entry
```

## 常见问题入口

- 启动失败、功能说明、项目整体结构：请先查看根目录 README  
  [../README.md](../README.md)
- 历史文档与更新记录：见根 README 的“相关连接”章节。
