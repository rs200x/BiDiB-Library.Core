namespace org.bidib.netbidibc.core.Utils
{
    public static class DccRcnUtils
    {
        public static int Dcc2Rcn(int addr)
        {
            return addr switch
            {
                < 4 => addr + 2044,
                < 2047 => addr - 3,
                2047 => 2047,
                _ => -1
            };
        }

        public static int Rcn2Dcc(int addr)
        {
            return addr switch
            {
                < 2044 => addr + 3,
                <= 2047 => addr - 2044, // 2047 == 0x7FF
                _ => -1
            };
        }
    }
}