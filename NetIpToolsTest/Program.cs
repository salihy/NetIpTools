using NetIpTools;
using NetTools;
using System;

namespace NetIpToolsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IPRange ipRange = new IPRange();
            ipRange.Parse("192.168.0.12/24");
            var ipList = ipRange.GetAllIpAddresses();
            var ipRangeList25 = ipRange.GetSmallerRanges(25);
            var ipRangeList26 = ipRange.GetSmallerRanges(26);
            var ipRangeList27 = ipRange.GetSmallerRanges(27);

            foreach (var item in ipRangeList25)
            {
                var v1 = item.GetAllIpAddresses();
                var v2 = v1;
            }

            foreach (var item in ipRangeList26)
            {
                var v1 = item.GetAllIpAddresses();
                var v2 = v1;
            }

            foreach (var item in ipRangeList27)
            {
                var v1 = item.GetAllIpAddresses();
                var v2 = v1;
            }
            var x = ipList;

            //var ipList2 = IPAddressRange.Parse("192.168.0.12/25");
            //var y = ipList2;
        }
    }
}
