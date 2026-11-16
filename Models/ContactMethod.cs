using System.ComponentModel.DataAnnotations;

namespace AddressBook.Models;

/// <summary>联系方式实体（电话 / 邮箱 / 社交账号 / 地址 / 其他）</summary>
public class ContactMethod
{
    public int Id { get; set; }

    public int ContactId { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "类型")]
    public string Type { get; set; } = "电话";

    [Required]
    [StringLength(200)]
    [Display(Name = "内容")]
    public string Value { get; set; } = "";

    public Contact? Contact { get; set; }
}
