using System.Reflection;

namespace OWP.Extensions;

public static class EnumExtension
{
    public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())[0]
            .GetCustomAttribute<TAttribute>();
    }
}