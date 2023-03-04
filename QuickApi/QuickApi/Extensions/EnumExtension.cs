namespace System.Linq;

public static class EnumExtension
{
    /// <summary>
    /// 获取枚举的描述
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
     public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
}