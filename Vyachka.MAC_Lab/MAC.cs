using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Vyachka.MAC_Lab
{
    public static class MAC
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(uint DestIP, uint SrcIP, byte[] pMacAddr, ref int PhyAddrLen);

        public static string ConvertIpToMAC(string destIP, string srcIP)
        {
            IPAddress dst = IPAddress.Parse(destIP);
            IPAddress src = IPAddress.Parse(srcIP);
            uint dstAddr = BitConverter.ToUInt32(dst.GetAddressBytes(), 0);
            uint srcAddr = BitConverter.ToUInt32(src.GetAddressBytes(), 0);

            byte[] macAddr = new byte[6];
            int macAddrLen = macAddr.Length;

            int retValue = SendARP(dstAddr, srcAddr, macAddr, ref macAddrLen);
            if (retValue != 0)
            {
                Console.WriteLine("SendARP failed.");
            }

            return BitConverter.ToString(macAddr);
        }
    }
}
