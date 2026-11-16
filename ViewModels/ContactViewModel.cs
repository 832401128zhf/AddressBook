using System.ComponentModel.DataAnnotations;

namespace AddressBook.ViewModels;

/// <summary>用于“添加 / 编辑联系人”表单的视图模型</summary>
public class ContactViewModel
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

    [Display(Name = "收藏该联系人")]
    public bool IsFavorite { get; set; }

    public List<ContactMethodViewModel> ContactMethods { get; set; } = new();
}

public class ContactMethodViewModel
{
    public string Type { get; set; } = "电话";
    public string Value { get; set; } = "";
}
