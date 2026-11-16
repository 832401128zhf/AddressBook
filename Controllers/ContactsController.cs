using AddressBook.Data;
using AddressBook.Models;
using AddressBook.Services;
using AddressBook.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Controllers;

public class ContactsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ExcelService _excel;

    public ContactsController(AppDbContext db, ExcelService excel)
    {
        _db = db;
        _excel = excel;
    }

    // GET: Contacts —— 列表 + 搜索 + 仅看收藏筛选
    public async Task<IActionResult> Index(string? search, bool favoritesOnly = false)
    {
        var query = _db.Contacts.Include(c => c.ContactMethods).AsQueryable();

        if (favoritesOnly)
            query = query.Where(c => c.IsFavorite);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(c =>
                c.Name.Contains(s) ||
                (c.Company != null && c.Company.Contains(s)) ||
                c.ContactMethods.Any(m => m.Value.Contains(s)));
        }

        var contacts = await query
            .OrderByDescending(c => c.IsFavorite)
            .ThenBy(c => c.Name)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.FavoritesOnly = favoritesOnly;
        return View(contacts);
    }

    // GET: Contacts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var contact = await _db.Contacts
            .Include(c => c.ContactMethods)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        return View(contact);
    }

    // GET: Contacts/Create
    public IActionResult Create()
    {
        var vm = new ContactViewModel
        {
            ContactMethods = new List<ContactMethodViewModel> { new ContactMethodViewModel() }
        };
        return View(vm);
    }

    // POST: Contacts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactViewModel vm)
    {
        vm.ContactMethods ??= new List<ContactMethodViewModel>();
        vm.ContactMethods = vm.ContactMethods.Where(m => !string.IsNullOrWhiteSpace(m.Value)).ToList();

        if (!ModelState.IsValid)
            return View(vm);

        var contact = new Contact
        {
            Name = vm.Name.Trim(),
            Company = Normalize(vm.Company),
            Notes = Normalize(vm.Notes),
            IsFavorite = vm.IsFavorite,
            CreatedAt = DateTime.Now,
            ContactMethods = vm.ContactMethods
                .Select(m => new ContactMethod { Type = m.Type, Value = m.Value!.Trim() })
                .ToList()
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"已添加联系人“{contact.Name}”。";
        return RedirectToAction(nameof(Index));
    }

    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var contact = await _db.Contacts
            .Include(c => c.ContactMethods)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        var vm = new ContactViewModel
        {
            Id = contact.Id,
            Name = contact.Name,
            Company = contact.Company,
            Notes = contact.Notes,
            IsFavorite = contact.IsFavorite,
            ContactMethods = contact.ContactMethods
                .Select(m => new ContactMethodViewModel { Type = m.Type, Value = m.Value })
                .ToList()
        };

        if (vm.ContactMethods.Count == 0)
            vm.ContactMethods.Add(new ContactMethodViewModel());

        return View(vm);
    }

    // POST: Contacts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContactViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        vm.ContactMethods ??= new List<ContactMethodViewModel>();
        vm.ContactMethods = vm.ContactMethods.Where(m => !string.IsNullOrWhiteSpace(m.Value)).ToList();

        if (!ModelState.IsValid)
            return View(vm);

        var contact = await _db.Contacts
            .Include(c => c.ContactMethods)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        contact.Name = vm.Name.Trim();
        contact.Company = Normalize(vm.Company);
        contact.Notes = Normalize(vm.Notes);
        contact.IsFavorite = vm.IsFavorite;

        // 先移除旧的联系方式（必填关系孤儿项会被自动删除），再写入新的
        contact.ContactMethods.Clear();
        foreach (var m in vm.ContactMethods)
        {
            contact.ContactMethods.Add(new ContactMethod { Type = m.Type, Value = m.Value.Trim() });
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = $"已更新联系人“{contact.Name}”。";
        return RedirectToAction(nameof(Index));
    }

    // POST: Contacts/ToggleFavorite/5 —— 收藏 / 取消收藏
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorite(int id, string? search, bool favoritesOnly)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact == null) return NotFound();

        contact.IsFavorite = !contact.IsFavorite;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { search, favoritesOnly });
    }

    // GET: Contacts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var contact = await _db.Contacts.FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        return View(contact);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contact = await _db.Contacts
            .Include(c => c.ContactMethods)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return NotFound();

        _db.Contacts.Remove(contact);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"已删除联系人“{contact.Name}”。";
        return RedirectToAction(nameof(Index));
    }

    // GET: Contacts/Export —— 导出为 Excel
    public IActionResult Export()
    {
        var contacts = _db.Contacts
            .Include(c => c.ContactMethods)
            .OrderBy(c => c.Name)
            .ToList();

        var bytes = _excel.ExportToExcel(contacts);
        var fileName = $"通讯录_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    // GET: Contacts/Template —— 下载导入模板
    public IActionResult Template()
    {
        var bytes = _excel.ExportTemplate();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "导入模板.xlsx");
    }

    // GET: Contacts/Import
    public IActionResult Import() => View();

    // POST: Contacts/Import —— 上传 Excel 并导入
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "请选择要上传的 Excel 文件。");
            return View();
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("", "只支持 .xlsx 格式的 Excel 文件。");
            return View();
        }

        try
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var (added, skipped) = _excel.ImportFromExcel(ms, _db);
            TempData["Success"] = $"导入完成：成功添加 {added} 条联系人，跳过 {skipped} 条空行。";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "导入失败：" + ex.Message);
            return View();
        }
    }

    private static string? Normalize(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
