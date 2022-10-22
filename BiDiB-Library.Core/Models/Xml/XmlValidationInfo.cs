namespace org.bidib.netbidibc.core.Models.Xml
{
    public class XmlValidationInfo : IXmlValidationInfo
    {
        public XmlValidationInfo(XmlValidationResult validationResult, string validationMessage)
        {
            Result = validationResult;
            Message = validationMessage;
        }

        public XmlValidationResult Result { get; private set; }
        public string Message { get; private set; }
    }
}