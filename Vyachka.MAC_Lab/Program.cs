﻿using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace Vyachka.MAC_Lab
{
    public class Program
    {
        public static void Main()
        {
            string hostName = Dns.GetHostName();
            Console.WriteLine($"Имя устройства: {hostName}");
            ArrayList interfaces = new ArrayList();
            foreach (var ip in Dns.GetHostEntry(hostName).AddressList)
            {
                if (!ip.ToString().Contains(":"))
                {
                    NetInterface netInterface = new NetInterface(ip, GetSubnetMask(ip));
                    interfaces.Add(netInterface);
                    Console.WriteLine($"MAC-адрес интерфейса: {MAC.ConvertIpToMAC(netInterface.IP.ToString(), 0.ToString())} " +
                        $"IP-адрес: {netInterface.IP}  Маска сети: {netInterface.Mask}");
                }
            }

            /*Console.WriteLine("Интерфейсы:");
            foreach(NetInterface netInterface in interfaces)
            {
                Console.WriteLine($"IP-адрес: {netInterface.IP}  Маска сети: {netInterface.Mask}");
            }*/

            Console.WriteLine("\nАктивные устройства в подсети:");
            Console.WriteLine("------------------------------");

            foreach(NetInterface netInterface in interfaces)
            {
                PingCheck.PreparationToPing(netInterface);
            }

            Console.WriteLine();
        }

        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var unicastIPAddressInformation in adapter.GetIPProperties()
                    .UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }

            throw new ArgumentException(string.Format($"Can't find subnetmask for IP address '{address}'"));
        }
    }
}