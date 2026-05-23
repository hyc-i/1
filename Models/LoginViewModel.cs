using System.ComponentModel.DataAnnotations;

namespace SecondClassroomManager.Models;

public class LoginViewModel
{
    [Display(Name = "账号")]
    [Required(ErrorMessage = "请输入账号")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "密码")]
    [Required(ErrorMessage = "请输入密码")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
