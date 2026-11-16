# 代码规范（Coding Standards）

本项目为 **通讯录管理系统**（ASP.NET Core MVC + .NET 8），团队协作遵循以下代码规范。

## 1. 命名规范

| 类型 | 规则 | 示例 |
| --- | --- | --- |
| 类 / 接口 / 枚举 | PascalCase | `ContactsController`、`AppDbContext` |
| 方法 / 属性 | PascalCase | `ToggleFavorite`、`CreatedAt` |
| 局部变量 / 参数 | camelCase | `contactId`、`favoritesOnly` |
| 私有字段 | `_` 前缀 + camelCase | `_db`、`_excel` |
| 常量 | PascalCase | `Headers` |
| 异步方法 | 以 `Async` 结尾 | `SaveChangesAsync` |

## 2. 文件与项目结构

- 按职责分目录：`Models`（数据模型）、`ViewModels`（表单模型）、`Data`（EF Core 上下文与初始化）、`Services`（业务服务）、`Controllers`（控制器）、`Views`（Razor 视图）。
- 一个类一个文件，文件名与类名一致。
- 视图文件放在 `Views/控制器名/` 下。

## 3. C# 编码约定

- 使用 **可空引用类型**（已启用 `<Nullable>enable</Nullable>`），可空参数/属性显式标注 `?`。
- 所有数据库访问使用 **异步方法**（`ToListAsync`、`FirstOrDefaultAsync`、`SaveChangesAsync`），避免阻塞线程。
- 通过 **依赖注入** 获取 `AppDbContext`、`ExcelService`，不在控制器里 `new` 实例。
- 使用 LINQ 查询，不写裸 SQL；EF Core 负责安全的参数化查询。
- 输入校验优先使用数据注解（`[Required]`、`[StringLength]`），并在控制器中做二次清洗（`Normalize`）。
- 对外部文件（Excel 上传）先校验扩展名、再解析，异常统一捕获后返回友好提示。

## 4. Razor 视图约定

- 使用 Bootstrap 5 类名构建页面，不内联大量自定义样式（自定义样式统一放 `wwwroot/css/site.css`）。
- 表单使用 `asp-for` / `asp-action` 标签助手，提交按钮配合 `[ValidateAntiForgeryToken]` 防跨站伪造。
- 动态行（联系方式）的 JS 逻辑集中放在 `wwwroot/js/site.js`。

## 5. Git 提交规范（Conventional Commits）

提交信息格式：`<类型>: <简述>`

| 类型 | 含义 |
| --- | --- |
| `feat` | 新功能 |
| `fix` | 修复缺陷 |
| `docs` | 文档 |
| `style` | 样式 / 格式 |
| `refactor` | 重构 |

示例：`feat: 实现收藏联系人与仅看收藏筛选`

## 6. 协作约定

- 主分支 `main` 保持可编译、可运行。
- 功能开发走独立分支（如 `feature/import-export`），完成后发起 Pull Request 合并。
- 合并前确保 `dotnet build` 0 错误、0 警告。
