using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Vyachka.MAC_Lab
{
    public static class PingCheck
    {
        private const int _timeToWait = 1;

        private static IPAddress GetIP(long startIP)
        {
            IPAddress ipAddress = new IPAddress(startIP);
            byte[] address = ipAddress.GetAddressBytes();

            byte temp = address[0];
            address[0] = address[3];
            address[3] = temp;
            temp = address[1];
            address[1] = address[2];
            address[2] = temp;
            
            return new IPAddress(address);
        }

        public static void PreparationToPing(NetInterface netInterface)
        {
            int amountOfAddresses = BitsWork.GetCount(BitsWork.ConvertToLong(netInterface.Mask));
            long subnetAddress = BitsWork.ConvertToLong(netInterface.IP) & BitsWork.ConvertToLong(netInterface.Mask);
            SplitIntoThreads(BitsWork.ConvertToLong(netInterface.IP), subnetAddress, amountOfAddresses);
        }

        public static void SplitIntoThreads(long interfaceIP, long subnetIP, int amountOfAddr)
        {
            int time = (int)Math.Ceiling(Math.Log2(amountOfAddr));
            int amountOfThreads = (amountOfAddr * _timeToWait) / time;
            int amountForThread = amountOfAddr / amountOfThreads;
            int remainder = amountOfAddr - amountForThread * amountOfThreads;
            amountOfThreads = (remainder > 0) ? (amountOfThreads + 1) : amountOfThreads;

            subnetIP += 1;
            Thread[] threads = new Thread[amountOfThreads];
            ThreadData[] data = new ThreadData[amountOfThreads];

            for(int i = 0; i < ((remainder > 0) ? (amountOfThreads - 1) : amountOfThreads); i++)
            {
                var threadData = new ThreadData(amountForThread, subnetIP, interfaceIP);
                threads[i] = new Thread(new ParameterizedThreadStart(ThreadFunc));
                data[i] = threadData;
                subnetIP += amountForThread;
            }    

            if(remainder > 0)
            {
                var threadData = new ThreadData(remainder, subnetIP, interfaceIP);
                threads[amountOfThreads - 1] = new Thread(new ParameterizedThreadStart(ThreadFunc));
                data[amountOfThreads - 1] = threadData;
            }

            for (int i = 0; i < amountOfThreads; i++)
            {
                threads[i].Start(data[i]);
            }

            for (int i = 0; i < amountOfThreads; i++)
            {
                threads[i].Join();
            }
        }

        private static void ThreadFunc(object obj)
        {
            ThreadData data = (ThreadData)obj;
            int amount = data.NumOfAddr;
            long interfaceIP = data.InterfaceAddr;
            long startIP = data.StartAddr;

            IPAddress srcIP = GetIP(interfaceIP);
            for(int i = 0; i < amount; i++)
            {
                Ping ping = new Ping();
                IPAddress ipAddress = GetIP(startIP);
                if(interfaceIP == startIP)
                {
                    startIP++;
                    continue;
                }

                PingReply pingReply = ping.Send(ipAddress, _timeToWait);
                if (pingReply != null && pingReply.Status == IPStatus.Success)
                {
                    try
                    {
                        Console.WriteLine($"Name: {Dns.GetHostEntry(ipAddress).HostName}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Name: failed getting name");
                    }

                    Console.WriteLine($"IP: {ipAddress}");
                    Console.WriteLine($"MAC-address: {MAC.GetMAC(ipAddress.ToString(), srcIP.ToString())}");
                    Console.WriteLine("------------------------------");
                }

                GC.Collect();
                startIP++;
            }
        }
    }
}