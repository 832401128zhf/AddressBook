using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models;

/// <summary>联系人实体</summary>
public class Contact
{
    public int Id { get; set; }

    [Required(ErrorMessage = "请输入姓名")]
    [StringLength(50, ErrorMessage = "姓名长度不能超过 50 个字符")]
    [Display(Name = "姓名")]
    public string Name { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "公司")]
    public string? Company { get; set; }

    [StringLength(500)]
    [Display(Name = "备注")]
    public string? Notes { get; set; }

    [Display(Name = "收藏")]
    public bool IsFavorite { get; set; }

    [Display(Name = "创建时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // 一个联系人对应多个联系方式
    public List<ContactMethod> ContactMethods { get; set; } = new();

    // 以下为“计算属性”，仅用于列表页显示，不会被映射到数据库
    public string Phones => string.Join("；", ContactMethods.Where(m => m.Type == "电话").Select(m => m.Value));
    public string Emails => string.Join("；", ContactMethods.Where(m => m.Type == "邮箱").Select(m => m.Value));
    public string Socials => string.Join("；", ContactMethods.Where(m => m.Type == "社交账号").Select(m => m.Value));
    public string Addresses => string.Join("；", ContactMethods.Where(m => m.Type == "地址").Select(m => m.Value));
}
