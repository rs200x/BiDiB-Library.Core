using System.Linq;
using System.Text;
using org.bidib.netbidibc.core.Enumerations;
using org.bidib.netbidibc.core.Utils;

namespace org.bidib.netbidibc.core.Models.Messages.Input
{
    public class SysErrorMessage : BiDiBInputMessage
    {
        public SysErrorMessage(byte[] messageBytes) : base(messageBytes, BiDiBMessage.MSG_SYS_ERROR, 1)
        {
            Error = (SystemError)MessageParameters[0];
            AnalyzeError();
        }

        private void AnalyzeError()
        {
            switch (Error)
            {
                case SystemError.BIDIB_ERR_NONE:
                case SystemError.BIDIB_ERR_IDDOUBLE:
                case SystemError.BIDIB_ERR_OVERRUN:
                case SystemError.BIDIB_ERR_RESET_REQUIRED:
                    {
                        Data = "-";
                        break;
                    }
                case SystemError.BIDIB_ERR_TXT:
                {
                    SetErrorText();
                    break;
                }
                case SystemError.BIDIB_ERR_CRC:
                case SystemError.BIDIB_ERR_SIZE:
                case SystemError.BIDIB_ERR_PARAMETER:
                    Data = $"MSG_NUM: {MessageParameters[1]:X2}";
                    break;
                case SystemError.BIDIB_ERR_SEQUENCE:
                    Data = $"Last good MSG_NUM: {MessageParameters[1]:X2}";
                    break;
                case SystemError.BIDIB_ERR_BUS:
                case SystemError.BIDIB_ERR_HW:
                case SystemError.BIDIB_ERR_NO_ACK_BY_HOST:
                    Data = $"ErrorCode: {MessageParameters[1]:X2}";
                    break;
                case SystemError.BIDIB_ERR_ADDRSTACK:
                    var items = MessageParameters.Skip(2).Select(x => $"{x:X2}");
                    Data = $"Node: {MessageParameters[1]:X2}, ADDR_STACK: {string.Join(".", items)}";
                    break;
                case SystemError.BIDIB_ERR_SUBCRC:
                case SystemError.BIDIB_ERR_SUBTIME:
                case SystemError.BIDIB_ERR_SUBPAKET:
                    Data = $"Node: {MessageParameters[1]:X2}";
                    break;
                default:
                    Data = MessageParameters.Skip(1).ToArray().GetDataString();
                    break;
            }
        }

        public SystemError Error { get; }

        public string Data { get; private set; }

        public override string ToString()
        {
            return $"{base.ToString()}, Error: {Error}, Data: {Data}";
        }
        private void SetErrorText()
        {
            int len = MessageParameters[1] + 2;
            StringBuilder sb = new();
            for (int k = 2; k < len; k++)
            {
                sb.Append((char)MessageParameters[k]);
            }

            Data = $"MSG: {sb}";
        }
    }
}