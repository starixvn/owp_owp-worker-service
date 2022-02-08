namespace OWP.Core.Attributes;

[AttributeUsage(AttributeTargets.All)]
public sealed class XmlNameAttribute : Attribute
{
    public string Name { get; set; }

    public XmlNameAttribute(string name)
    {
        this.Name = name;
    }
}