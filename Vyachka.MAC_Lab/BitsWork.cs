using System;
using System.Net;

namespace Vyachka.MAC_Lab
{
    public class BitsWork
    {
        public static long ConvertToLong(IPAddress ipAddress)
        {
            string ip = ipAddress.ToString();
            string result = "";
            string[] numbers = ip.Trim().Split('.');
            foreach (var s in numbers)
            {
                string oct = Convert.ToString(int.Parse(s), 2);
                while (oct.Length < 8)
                {
                    oct = "0" + oct;
                }

                result += oct;
            }

            long l = 0;
            int k = 0;
            for (int i = 31; i >= 0; i--)
            {
                l += (long)((result[k] - '0') * Math.Pow(2, i));
                k++;
            }

            return l;
        }

        public static int GetCount(long mask)
        {
            string m = Convert.ToString(mask, 2);
            int count = 0;
            int i = 31;
            while (m[i] != '1')
            {
                count++;
                i--;
            }

            return (int)Math.Pow(2, count) - 2;
        }
    }
}
