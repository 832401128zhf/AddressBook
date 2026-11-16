using AddressBook.Data;
using AddressBook.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 只使用控制台日志：避免依赖 Windows 事件日志（在受限环境或 Linux 上部署时更稳定）
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 注册 MVC 服务
builder.Services.AddControllersWithViews();

// 注册数据库（SQLite 文件型数据库，无需单独安装数据库软件）
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册 Excel 导入导出服务
builder.Services.AddScoped<ExcelService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 首次运行时自动创建数据库并写入示例数据
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DbInitializer.Seed(db);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contacts}/{action=Index}/{id?}");

app.Run();
