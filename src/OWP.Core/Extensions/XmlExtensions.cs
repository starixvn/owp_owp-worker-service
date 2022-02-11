using OWP.Core.Attributes;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OWP.Core.Extensions;

public static class XmlExtensions
{
    public static XElement ToXml<T>(this IEnumerable<T> source)
    {
        var lambda = GetElement<T>(typeof(T));

        return new XElement("Root", source.Select(lambda).ToArray());
    }

    private static XName GetElementName(Type t)
    {
        var attr = t.GetCustomAttribute<XmlNameAttribute>();
        return attr == null ? t.Name : attr.Name;
    }

    private static XName GetPropertyName(PropertyInfo p)
    {
        var attr = p.GetCustomAttribute<XmlNameAttribute>();
        return attr == null ? p.Name : attr.Name;
    }

    private static Func<T, XElement> GetElement<T>(Type t)
    {
        Expression<Func<XName, object, XAttribute>> attrSetter = (name, obj) => new XAttribute(name, obj == null ? string.Empty : obj.ToString());
        var elementName = GetElementName(t);
        var p = Expression.Parameter(t);

        var args = GetExpressions(t, p, attrSetter);

        var constructor = typeof(XElement).GetConstructor(new[] { typeof(XName), typeof(object[]) });
        var body = Expression.New(constructor, Expression.Constant(elementName), Expression.NewArrayInit(typeof(object), args));

        return Expression.Lambda<Func<T, XElement>>(body, p).Compile();
    }

    private static IEnumerable<Expression> GetExpressions(Type type, Expression obj, Expression lambda)
        => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<XmlIgnoreAttribute>() == null)
            .SelectMany(p => GetExpressions(p, obj, lambda));

    private static bool IsValidPropertyInfo(PropertyInfo p)
        => p.PropertyType.IsPrimitive
            || p.PropertyType == typeof(string)
            || p.PropertyType == typeof(DateTime);

    private static IEnumerable<Expression> GetExpressions(PropertyInfo p, Expression obj, Expression lambda)
    {
        Console.WriteLine(p.Name);
        var prop = Expression.Property(obj, p);

        if (IsValidPropertyInfo(p)
            || p.PropertyType.IsGenericType
                && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var name = GetPropertyName(p);
            return new[] { Expression.Invoke(lambda, Expression.Constant(name), Expression.Convert(prop, typeof(object))) };
        }

        return GetExpressions(p.PropertyType, prop, lambda);
    }
}