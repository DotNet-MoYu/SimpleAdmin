# 文件管理大文件分片上传方案

## 方案摘要
在现有 `biz/document` 文件管理链路上新增一套分片上传能力，替代“大文件整包 `multipart/form-data` 一次性提交”的做法。

本期范围只覆盖文件管理页的“上传文件 / 上传文件夹”，支持大文件分片、同页内暂停继续、失败分片重试、已传分片探测、合并入库；不做跨刷新任务恢复，不扩散到 `sys/upload` 或 `sys/dev/file`。

核心原则：

1. 分片上传阶段不创建 `BizDocument` / `SysFile` 正式记录，只落临时分片状态。
2. 只有“全部分片上传完成并合并成功”后，才一次性生成正式文件记录和文档记录。
3. 权限、目录合法性、重名校验仍由 `DocumentService` 负责，避免把业务规则散到控制器或前端。
4. 存储先按当前页面的 `Engine=LOCAL` 落地，但接口与服务设计保持可扩展，后续可接 MinIO。

## 现状结论
当前上传链路如下：

1. 前端文件管理页 [index.vue](/D:/SimpleAdmin/web/src/views/biz/document/index.vue) 直接通过 `FormData` 调用 `uploadFiles` / `uploadFolder`。
2. 前端 API 在 [document.ts](/D:/SimpleAdmin/web/src/api/modules/biz/document.ts) 中只提供整文件上传接口。
3. 后端控制器 [DocumentController.cs](/D:/SimpleAdmin/api/SimpleAdmin/SimpleAdmin.Web.Core/Controllers/Application/Document/DocumentController.cs) 当前只有 `uploadFiles` 和 `uploadFolder` 两个 `multipart/form-data` 入口。
4. 业务服务 [DocumentService.cs](/D:/SimpleAdmin/api/SimpleAdmin/SimpleAdmin.Application/Services/Document/Document/DocumentService.cs) 在收到 `IFormFile` 后直接调用 `_documentStorageService.Upload(...)`，随后立即写入 `BizDocument`。
5. 存储适配层 [DocumentStorageService.cs](/D:/SimpleAdmin/api/SimpleAdmin/SimpleAdmin.Application/Services/Document/Common/DocumentStorageService.cs) 继续委托 `FileService.UploadFile(...)` 创建 `SysFile`。
6. 系统文件服务 [FileService.cs](/D:/SimpleAdmin/api/SimpleAdmin/SimpleAdmin.System/Services/Dev/File/FileService.cs) 目前只支持“接收完整 `IFormFile` 后一次性落盘/入 MinIO”。

这意味着大文件分片不能只改前端，必须新增一条“先临时分片、后合并、最后一次性入库”的后端链路。

## 目标能力
本期目标能力固定为：

1. 仅覆盖 `biz/document` 文件管理页。
2. 支持大文件分片上传。
3. 支持同页断点续传。
4. 支持失败分片重试。
5. 支持服务端查询已上传分片。
6. 支持上传完成后一次性生成正式文档记录。
7. 支持文件夹上传时按 `relativePath` 重建目录结构。

本期明确不做：

1. 不做跨页面刷新恢复任务。
2. 不做全局物理文件去重。
3. 不改 `sys/upload` 图片上传。
4. 不改 `sys/dev/file` 上传能力。
5. 不做空文件夹保真上传。

## 设计总览
### 总体流程
建议把上传流程拆成 5 段：

1. 前端对单文件计算 `fileHash`，按阈值判断是否走分片上传。
2. 前端调用 `upload/init` 创建上传会话，服务端返回 `uploadId`、总分片数、已上传分片索引。
3. 前端按分片并发调用 `upload/chunk` 上传每一片。
4. 全部分片完成后，前端调用 `upload/complete`，服务端负责校验、合并、创建 `SysFile` 与 `BizDocument`。
5. 成功后刷新当前目录树与列表；失败则允许继续/重试/取消。

### 职责分层
建议按下面方式分层：

1. `DocumentController`
   只暴露 HTTP 接口，不承担业务判断。
2. `DocumentService`
   负责权限、目录合法性、重名校验、目录重建、文档落库、日志写入。
3. `DocumentStorageService`
   负责上传会话、临时分片目录、分片落盘、文件合并、正式文件写入准备。
4. `FileService`
   扩展“从已存在完整文件生成 `SysFile`”的能力，避免合并后再包装成 `IFormFile` 重走旧链路。

## 后端设计
### 新增接口
建议在 `/biz/document` 下新增以下接口：

1. `POST /biz/document/upload/init`
   用途：初始化单文件分片上传会话。
2. `POST /biz/document/upload/chunk`
   用途：上传单个分片。
3. `GET /biz/document/upload/status`
   用途：查询某个会话已上传分片。
4. `POST /biz/document/upload/complete`
   用途：完成上传、服务端合并、生成正式文件记录。
5. `POST /biz/document/upload/cancel`
   用途：取消上传并清理临时分片。
6. `POST /biz/document/upload/folder/init`
   用途：文件夹批量初始化多个文件上传会话。

### DTO 设计
建议在 `Document/Document/Dto` 下新增以下入参与出参类型：

1. `ChunkUploadInitInput`
   字段：
   - `ParentId`
   - `Engine`
   - `FileName`
   - `FileSize`
   - `ChunkSize`
   - `FileHash`
   - `RelativePath`
2. `ChunkUploadInitOutput`
   字段：
   - `UploadId`
   - `ChunkSize`
   - `ChunkCount`
   - `UploadedChunks`
   - `SkipUpload`
   - `DocumentId`
3. `ChunkUploadPartInput`
   字段：
   - `UploadId`
   - `ChunkIndex`
   - `ChunkHash`
   - `Chunk`
4. `ChunkUploadStatusInput`
   字段：
   - `UploadId`
5. `ChunkUploadStatusOutput`
   字段：
   - `UploadId`
   - `UploadedChunks`
   - `ChunkCount`
   - `Status`
   - `IsCompleted`
6. `ChunkUploadCompleteInput`
   字段：
   - `UploadId`
7. `ChunkUploadCompleteOutput`
   字段：
   - `UploadId`
   - `DocumentId`
   - `FileId`
   - `FileName`
8. `ChunkUploadCancelInput`
   字段：
   - `UploadId`

### 上传会话持久化
需要新增一张上传会话表，建议命名为 `biz_document_upload_session`。

字段建议至少包含：

1. `Id`
2. `UploadId`
3. `ParentId`
4. `RootId`
5. `Engine`
6. `FileName`
7. `RelativePath`
8. `FileHash`
9. `FileSize`
10. `ChunkSize`
11. `ChunkCount`
12. `UploadedChunkIndexesJson`
13. `TempDir`
14. `Status`
15. `SysFileId`
16. `DocumentId`
17. `ExpireTime`
18. 审计字段

状态值建议固定为：

1. `Init`
2. `Uploading`
3. `Merging`
4. `Completed`
5. `Cancelled`
6. `Failed`

### 关键业务规则
必须固定以下规则，避免实现阶段再临时拍板：

1. `init` 时先做父目录合法性校验和上传权限校验，不允许未授权目录建立上传会话。
2. `complete` 时再次做权限、目录存在性和重名校验，防止上传期间目录状态变化。
3. 分片上传阶段不写正式 `BizDocument` 和 `SysFile`。
4. 同一个 `uploadId` 的同一分片重复上传时，如果分片 hash 一致，则直接幂等返回成功。
5. `complete` 重复调用时，如果会话已完成，则直接返回已有 `DocumentId`。
6. 文件夹上传时，每个文件独立建会话；目录结构仍由 `RelativePath` 驱动重建。
7. 普通用户仍不能上传到根目录。
8. 父节点不是文件夹时直接拒绝上传。
9. `Cancel` 或 `Failed` 会话必须删除临时分片目录。

### 存储链路改造
建议扩展 [IDocumentStorageService.cs](/D:/SimpleAdmin/api/SimpleAdmin/SimpleAdmin.Application/Services/Document/Common/IDocumentStorageService.cs)，增加：

1. `InitChunkUpload(...)`
2. `SaveChunk(...)`
3. `GetChunkUploadStatus(...)`
4. `CompleteChunkUpload(...)`
5. `CancelChunkUpload(...)`

建议扩展 [FileService.cs](/D:/SimpleAdmin/api/SimpleAdmin/SimpleAdmin.System/Services/Dev/File/FileService.cs) 与 `IFileService`：

1. 新增“从本地临时完整文件创建 `SysFile`”的方法。
2. 该方法负责把合并后的完整文件转成正式存储文件，并写入 `sys_file`。
3. 如果后续接 MinIO，也由这里统一接管“从完整文件推送到对象存储”的逻辑。

### 临时目录设计
建议临时目录结构如下：

1. 上传根目录：`<upload-root>/chunks/`
2. 单文件会话目录：`<upload-root>/chunks/<uploadId>/`
3. 单分片文件：`<upload-root>/chunks/<uploadId>/<chunkIndex>.part`
4. 合并后临时完整文件：`<upload-root>/chunks/<uploadId>/merged/<fileName>`

### 合并流程
`upload/complete` 的服务端流程建议固定为：

1. 查询上传会话。
2. 校验会话状态是否合法。
3. 校验分片数量是否齐全。
4. 按顺序读取 `<chunkIndex>.part` 合并为完整文件。
5. 调用扩展后的 `FileService` 创建正式 `SysFile`。
6. 调用 `DocumentService` 创建正式 `BizDocument`。
7. 写入文件日志。
8. 更新会话状态为 `Completed`，回填 `SysFileId` 和 `DocumentId`。
9. 删除临时分片目录。

失败处理要求：

1. 合并失败不创建正式记录。
2. 正式文件落库失败时，回滚数据库事务，并删除已生成的半成品文件。
3. 会话状态更新为 `Failed`，保留错误信息便于排查。

### 清理策略
需要补一个临时分片清理机制，建议如下：

1. `Cancelled` 和 `Failed` 会话立即清理临时目录。
2. `Init` / `Uploading` 超过 24 小时未更新的会话，由定时清理任务删除。
3. 清理任务只处理未完成上传会话，不碰正式文件。

## 前端设计
### 上传模式切换
文件管理页 [index.vue](/D:/SimpleAdmin/web/src/views/biz/document/index.vue) 当前是直接 `FormData` 提交，建议改成：

1. 小文件低于阈值时，保留现有普通上传逻辑。
2. 大于阈值时，自动切换到分片上传。

默认阈值建议：

1. `20MB` 以上走分片上传。
2. 默认分片大小 `5MB`。
3. 默认并发数 `3`。

### 前端任务管理器
建议新增一层上传任务管理器，例如：

1. `web/src/views/biz/document/hooks/useChunkUpload.ts`
2. `web/src/views/biz/document/components/uploadPanel.vue`

职责：

1. 计算文件 hash。
2. 维护任务状态。
3. 控制并发上传。
4. 支持暂停、继续、取消、重试。
5. 汇总总进度和单文件进度。

### 前端任务状态
每个文件任务状态建议固定为：

1. `waiting`
2. `hashing`
3. `uploading`
4. `paused`
5. `merging`
6. `success`
7. `error`

每个任务需要记录：

1. `file`
2. `fileName`
3. `relativePath`
4. `uploadId`
5. `fileHash`
6. `chunkSize`
7. `chunkCount`
8. `uploadedChunks`
9. `progress`
10. `status`
11. `errorMessage`

### 前端接口扩展
建议在 [document.ts](/D:/SimpleAdmin/web/src/api/modules/biz/document.ts) 中增加：

1. `uploadInit`
2. `uploadChunk`
3. `uploadStatus`
4. `uploadComplete`
5. `uploadCancel`
6. `uploadFolderInit`

建议在 [document.ts](/D:/SimpleAdmin/web/src/api/interface/biz/document.ts) 中增加：

1. `ChunkUploadInitReq`
2. `ChunkUploadInitRes`
3. `ChunkUploadStatusRes`
4. `ChunkUploadCompleteReq`
5. `ChunkUploadCompleteRes`
6. `ChunkTaskState`

### 前端页面交互
页面上建议新增一个轻量上传面板，显示：

1. 批次总进度
2. 当前活跃任务数
3. 单文件进度条
4. 单文件状态
5. 暂停按钮
6. 继续按钮
7. 取消按钮
8. 重试按钮

交互要求：

1. 选择文件后立即进入任务列表。
2. 上传成功后自动刷新目录树和表格。
3. 失败文件可单独重试，不阻断其余文件。
4. 文件夹上传时，按 `webkitRelativePath` 建立文件任务。

## 测试与验收
### 后端测试场景
至少验证以下场景：

1. 小文件普通上传不回归。
2. 100MB 以上文件能成功分片上传并生成 `SysFile`、`BizDocument`。
3. 中途中断后在同一页面继续上传，只补传缺失分片。
4. 同一分片重复上传时结果幂等。
5. `complete` 重复调用时不重复创建文档。
6. 父节点非文件夹时拒绝上传。
7. 普通用户上传到根目录被拒绝。
8. 缺失分片或 hash 不一致时合并失败且不生成正式记录。
9. 取消上传后临时目录被删除。
10. 文件夹上传后目录结构正确重建。

### 前端测试场景
至少验证以下场景：

1. 单个大文件上传时进度正确。
2. 大文件可暂停与继续。
3. 单个分片失败后可重试。
4. 多文件并发上传时总进度与单文件进度一致。
5. 文件夹上传时每个文件都有独立状态。
6. 成功上传后列表与目录树都能刷新到最新状态。

## 默认实现决策
本方案采用以下默认决策：

1. 只覆盖 `biz/document`。
2. 只做同页断点续传。
3. v1 先按 `LOCAL` 引擎落地。
4. 保留旧的 `uploadFiles` / `uploadFolder` 作为兼容接口。
5. 大文件阈值默认 `20MB`。
6. 默认分片大小 `5MB`。
7. 默认并发数 `3`。
8. 上传会话过期时间默认 `24` 小时。

## 实施顺序
建议按以下顺序落地：

1. 先加上传会话表、DTO、Controller 路由。
2. 再扩展 `DocumentStorageService` 和 `FileService`，打通临时分片到正式文件的后端链路。
3. 然后改造 `DocumentService`，把最终文档创建与日志写入接到 `complete`。
4. 最后在前端引入任务管理器和上传面板，替换当前直接 `FormData` 上传行为。
5. 完成后执行后端构建和前端类型检查，最后做手工上传验收。
