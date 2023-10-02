namespace org.bidib.Net.Core.Models.Xml;

public struct XmlNamespace
{
    public XmlNamespace(string prefix, string url, string xsdName)
    {
        Prefix = prefix;
        Url = url;
        XsdName = xsdName;
    }

    public string Url { get; }

    public string Prefix { get; }

    public string XsdName { get; }
}