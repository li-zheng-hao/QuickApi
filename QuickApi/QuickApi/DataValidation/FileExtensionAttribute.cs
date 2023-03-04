using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuickApi.DataValidation;

/// <summary>
/// Microsoft.AspNetCore.Http.IFormFile 文件后缀名验证
/// </summary>
public class FileExtensionAttribute : ValidationAttribute
{
    /// <summary>
    /// 允许的后缀名 例如：.jpg,.png
    /// </summary>
    public string[] AllowExtensions { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var file = validationContext.ObjectInstance as Microsoft.AspNetCore.Http.IFormFile;
        var allowList = AllowExtensions.ToList();
        if (file != null)
        {
            var extension = System.IO.Path.GetExtension(file.FileName);
            if (!allowList.Contains(extension))
            {
                return new ValidationResult($"文件后缀名不正确，只允许上传{string.Join(",", allowList)}格式的文件");
            }
        }

        return ValidationResult.Success;
    }
}