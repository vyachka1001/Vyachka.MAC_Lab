using System.Net;

namespace Vyachka.MAC_Lab
{
    public class NetInterface
    {
        public IPAddress IP { get; set; }
        public IPAddress Mask { get; set; }

        public NetInterface(IPAddress ip, IPAddress mask)
        {
            IP = ip;
            Mask = mask;
        }
    }
}