# 通讯录管理系统（Address Book）

软件工程（FZU–MU）· 极限编程作业 —— 通讯录管理系统。

基于 **ASP.NET Core MVC (.NET 8)** 的网页版通讯录，支持联系人的增删改查、**收藏联系人**、**一个联系人多个联系方式**、**Excel 导入/导出** 等功能。

## 功能清单（对应作业评分项）

| 评分项 | 分值 | 实现情况 |
| --- | --- | --- |
| 页面风格（Page Style） | 10 | Bootstrap 5 响应式界面，列表/表单/详情页统一风格 |
| 收藏联系人（Bookmark Contacts） | 25 | 列表页 ⭐ 一键收藏/取消；支持“仅看收藏”筛选；收藏联系人置顶 |
| 添加多个联系方式（Multiple Contact Methods） | 10 | 一个联系人可添加多个电话/邮箱/社交账号/地址，动态增删行 |
| 导入导出（Import & Export） | 25 | 导出为 .xlsx（每人一行、每类信息一列）；导入正确格式的 .xlsx；提供导入模板下载 |
| 云端部署（Web Deployment，可选） | +15 | 见 `操作说明.md` 第 7 节 |

## 技术栈

- .NET 8（ASP.NET Core MVC）
- Entity Framework Core + SQLite（文件型数据库，无需单独安装）
- ClosedXML（Excel 读写）
- Bootstrap 5（CDN，页面样式）

## 目录结构

```
AddressBook/
├── AddressBook.sln
├── AddressBook.csproj
├── Program.cs
├── appsettings.json
├── Controllers/
│   ├── HomeController.cs
│   └── ContactsController.cs      # 核心控制器（CRUD/收藏/导入导出）
├── Models/
│   ├── Contact.cs                 # 联系人实体
│   └── ContactMethod.cs           # 联系方式实体
├── ViewModels/
│   └── ContactViewModel.cs        # 表单视图模型
├── Data/
│   ├── AppDbContext.cs            # EF Core 上下文
│   └── DbInitializer.cs           # 示例数据
├── Services/
│   └── ExcelService.cs            # Excel 导入/导出
├── Views/
│   ├── Contacts/                  # 列表/详情/新建/编辑/删除/导入
│   └── Shared/_Layout.cshtml      # 页面布局
└── wwwroot/
    ├── css/site.css
    └── js/site.js                 # 联系方式动态增删行
```

## 运行方式

1. 用 Visual Studio 2022 打开 `AddressBook.sln`。
2. 确保已安装「ASP.NET 和 Web 开发」工作负载（或 .NET 8 SDK）。
3. 首次打开会自动还原 NuGet 包（需要联网）。
4. 按 `Ctrl+F5`（或 F5）运行，浏览器自动打开 `http://localhost:5208`。
5. 首次运行自动创建 SQLite 数据库 `addressbook.db` 并写入 3 条示例数据。

> 详细的“一步一步”操作请见 [`操作说明.md`](./操作说明.md)。
