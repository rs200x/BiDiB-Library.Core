using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace org.bidib.netbidibc.core.Utils
{
    public static class SerialPortUtils
    {
        public static IEnumerable<Tuple<string, string>> GetSerialPortInfos()
        {
            var portNames = new List<Tuple<string, string>>();
            try
            {
                using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0"))
                {
                    var ports = searcher.Get()
                                .OfType<ManagementObject>()
                                .Where(x =>
                                        x["Caption"] != null && x["Caption"].ToString().Contains("(COM") 
                                        //&& x["DeviceId"] != null && x["DeviceId"].ToString().Contains("VID_0403")
                                        ).ToList();

                    foreach (ManagementObject instance in ports)
                    {
                        string caption = instance["caption"].ToString();
                        int index = caption.LastIndexOf("(COM", StringComparison.Ordinal);
                        string portName = caption.Substring(index + 1, caption.LastIndexOf(")", StringComparison.Ordinal) - index - 1);
                        portNames.Add(new Tuple<string, string>(portName, $"{portName,-15}{instance["Description"]}"));
                    }
                }
            }
            catch (Exception ex)
            {
                //Logger.Warn("Could not determine available ports", ex);
                portNames = new List<Tuple<string, string>>(SerialPort.GetPortNames().Select(x => new Tuple<string, string>(x, null)));
            }

            return portNames;
        }
    }
}