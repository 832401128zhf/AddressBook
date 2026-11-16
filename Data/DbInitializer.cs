using AddressBook.Models;

namespace AddressBook.Data;

/// <summary>数据库初始化：首次运行时写入示例数据，便于演示</summary>
public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Contacts.Any())
            return;

        var contacts = new List<Contact>
        {
            new Contact
            {
                Name = "张三",
                Company = "福州大学",
                Notes = "同班同学",
                IsFavorite = true,
                ContactMethods = new List<ContactMethod>
                {
                    new ContactMethod { Type = "电话", Value = "13800000001" },
                    new ContactMethod { Type = "电话", Value = "0591-88888888" },
                    new ContactMethod { Type = "邮箱", Value = "zhangsan@example.com" },
                    new ContactMethod { Type = "社交账号", Value = "WeChat: zhangsan01" },
                    new ContactMethod { Type = "地址", Value = "福建省福州市" }
                }
            },
            new Contact
            {
                Name = "李四",
                Company = "XX科技有限公司",
                Notes = "实习同事",
                IsFavorite = false,
                ContactMethods = new List<ContactMethod>
                {
                    new ContactMethod { Type = "电话", Value = "13900000002" },
                    new ContactMethod { Type = "邮箱", Value = "lisi@example.com" }
                }
            },
            new Contact
            {
                Name = "王五",
                Company = "梅努斯国际工程学院",
                Notes = "项目队友",
                IsFavorite = true,
                ContactMethods = new List<ContactMethod>
                {
                    new ContactMethod { Type = "电话", Value = "13700000003" },
                    new ContactMethod { Type = "社交账号", Value = "QQ: 123456789" }
                }
            }
        };

        db.Contacts.AddRange(contacts);
        db.SaveChanges();
    }
}
