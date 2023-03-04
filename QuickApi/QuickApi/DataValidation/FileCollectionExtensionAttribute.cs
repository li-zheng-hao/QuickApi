using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace QuickApi.DataValidation;

/// <summary>
/// Microsoft.AspNetCore.Http.IFormFile IList集合所有文件后缀名验证
/// </summary>
public class FileCollectionExtensionAttribute:ValidationAttribute
{
    /// <summary>
    /// 允许的后缀名 例如：.jpg,.png
    /// </summary>
    public string[] AllowExtensions { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var fileList = validationContext.ObjectInstance as IList<IFormFile>;
        var allowList = AllowExtensions.ToList();
        foreach (var formFile in fileList)
        {
            var extension = System.IO.Path.GetExtension(formFile.FileName);
            if (!allowList.Contains(extension))
            {
                return new ValidationResult($"文件后缀名不正确，只允许上传{string.Join(",", allowList)}格式的文件");
            }
        }
        return ValidationResult.Success;
    }
}