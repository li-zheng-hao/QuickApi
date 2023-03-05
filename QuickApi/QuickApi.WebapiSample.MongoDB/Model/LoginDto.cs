using System.ComponentModel.DataAnnotations;

namespace QuickApi.WebapiSample.Model;

public class LoginDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; }
}