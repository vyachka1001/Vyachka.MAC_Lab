namespace Vyachka.MAC_Lab
{
    public class ThreadData
    {
        public int NumOfAddr { set; get; }
        public long StartAddr { set; get; }
        public long InterfaceAddr { set; get; }

        public ThreadData(int amount, long start, long interfaceAddr)
        {
            NumOfAddr = amount;
            StartAddr = start;
            InterfaceAddr = interfaceAddr;
        }
    }
}
