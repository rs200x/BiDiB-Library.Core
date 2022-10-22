namespace org.bidib.netbidibc.core.Models.Xml
{
    public interface IXmlValidationInfo
    {
         XmlValidationResult Result { get; }

        string Message { get; }
    }
}