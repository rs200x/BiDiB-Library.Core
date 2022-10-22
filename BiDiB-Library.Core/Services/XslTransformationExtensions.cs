using System;

namespace org.bidib.netbidibc.core.Services
{
    public class XslTransformationExtensions
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string NewDate()
        {
            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        public string NewDateTime()
        {
            return DateTime.Now.ToString("s");
        }
    }
}
