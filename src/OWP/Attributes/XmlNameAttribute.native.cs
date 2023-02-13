namespace OWP.Attributes;

[AttributeUsage(AttributeTargets.All)]
public sealed class XmlNameAttribute : Attribute
{
    public string Name { get; set; }

    public XmlNameAttribute(string name)
    {
        Name = name;
    }
}