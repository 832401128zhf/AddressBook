using AddressBook.Data;
using AddressBook.Models;
using ClosedXML.Excel;

namespace AddressBook.Services;

/// <summary>Excel 导入 / 导出服务（基于 ClosedXML）</summary>
public class ExcelService
{
    private static readonly string[] Headers = { "姓名", "公司", "电话", "邮箱", "社交账号", "地址", "备注", "收藏" };

    /// <summary>把所有联系人及其联系方式导出为 .xlsx（每人一行、每类信息一列）</summary>
    public byte[] ExportToExcel(List<Contact> contacts)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("通讯录");
        WriteHeader(ws);

        var row = 2;
        foreach (var c in contacts)
        {
            ws.Cell(row, 1).Value = c.Name;
            ws.Cell(row, 2).Value = c.Company ?? "";
            ws.Cell(row, 3).Value = JoinByType(c, "电话");
            ws.Cell(row, 4).Value = JoinByType(c, "邮箱");
            ws.Cell(row, 5).Value = JoinByType(c, "社交账号");
            ws.Cell(row, 6).Value = JoinByType(c, "地址");
            ws.Cell(row, 7).Value = c.Notes ?? "";
            ws.Cell(row, 8).Value = c.IsFavorite ? "是" : "否";
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    /// <summary>导出“导入模板”（含表头和一行示例）</summary>
    public byte[] ExportTemplate()
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("通讯录");
        WriteHeader(ws);

        ws.Cell(2, 1).Value = "张三";
        ws.Cell(2, 2).Value = "示例公司";
        ws.Cell(2, 3).Value = "13800000001；0591-88888888";
        ws.Cell(2, 4).Value = "zhangsan@example.com";
        ws.Cell(2, 5).Value = "WeChat: zhangsan01";
        ws.Cell(2, 6).Value = "福建省福州市";
        ws.Cell(2, 7).Value = "示例备注";
        ws.Cell(2, 8).Value = "是";

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    /// <summary>读取正确格式的 Excel，并把联系人加入通讯录。返回（成功条数，跳过条数）。</summary>
    public (int Added, int Skipped) ImportFromExcel(Stream stream, AppDbContext db)
    {
        using var wb = new XLWorkbook(stream);
        var ws = wb.Worksheet(1);

        var used = ws.RangeUsed();
        if (used == null)
            throw new InvalidOperationException("Excel 文件为空。");

        // 第一行为表头，按表头文本定位各列
        var headerRow = used.Row(1);
        var columns = new Dictionary<string, int>();
        foreach (var cell in headerRow.CellsUsed())
        {
            var text = cell.GetString().Trim();
            if (!string.IsNullOrEmpty(text) && !columns.ContainsKey(text))
                columns[text] = cell.Address.ColumnNumber;
        }

        int Col(string name) => columns.TryGetValue(name, out var i) ? i : -1;

        var nameCol = Col("姓名");
        if (nameCol < 0)
            throw new InvalidOperationException("找不到“姓名”列，请使用“下载导入模板”的格式。");

        var added = 0;
        var skipped = 0;

        for (var r = 2; r <= used.RowCount(); r++)
        {
            var name = ReadCell(ws, r, nameCol);
            if (string.IsNullOrEmpty(name))
            {
                skipped++;
                continue;
            }

            var contact = new Contact
            {
                Name = name,
                Company = ReadCell(ws, r, Col("公司")),
                Notes = ReadCell(ws, r, Col("备注")),
                IsFavorite = IsTrue(ReadCell(ws, r, Col("收藏"))),
                CreatedAt = DateTime.Now,
                ContactMethods = new List<ContactMethod>()
            };

            AddMethods(contact, "电话", ReadCell(ws, r, Col("电话")));
            AddMethods(contact, "邮箱", ReadCell(ws, r, Col("邮箱")));
            AddMethods(contact, "社交账号", ReadCell(ws, r, Col("社交账号")));
            AddMethods(contact, "地址", ReadCell(ws, r, Col("地址")));

            db.Contacts.Add(contact);
            added++;
        }

        db.SaveChanges();
        return (added, skipped);
    }

    private static void WriteHeader(IXLWorksheet ws)
    {
        for (var i = 0; i < Headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = Headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
        }
    }

    private static string JoinByType(Contact c, string type) =>
        string.Join("；", c.ContactMethods.Where(m => m.Type == type).Select(m => m.Value));

    private static string ReadCell(IXLWorksheet ws, int row, int col) =>
        col > 0 ? ws.Cell(row, col).GetString().Trim() : "";

    private static bool IsTrue(string value) =>
        value is "是" or "1" or "true" or "TRUE" or "True" or "y" or "Y" or "√" or "yes" or "YES";

    private static void AddMethods(Contact c, string type, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        foreach (var part in value.Split(new[] { '；', ';', '、', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var v = part.Trim();
            if (!string.IsNullOrEmpty(v))
                c.ContactMethods.Add(new ContactMethod { Type = type, Value = v });
        }
    }
}
