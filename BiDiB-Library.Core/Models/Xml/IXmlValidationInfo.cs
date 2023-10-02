namespace org.bidib.Net.Core.Models.Xml;

public interface IXmlValidationInfo
{
    XmlValidationResult Result { get; }

    string Message { get; }
}