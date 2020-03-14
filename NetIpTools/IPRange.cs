using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;


namespace NetIpTools
{
    public class IPRange
    {
        public int Prefix { get; set; }
        private BitArray Bits = null;
        public string BaseAddress { get; set; }

        public bool Parse(string ipAddressRange)
        {
            Regex rgx = new Regex(@"^([0-9]{1,3}\.){3}[0-9]{1,3}(\/([0-9]|[1-2][0-9]|3[0-2]))?$");
            if (!rgx.IsMatch(ipAddressRange))
            {
                return false;
            }
            var ipRepr = ipAddressRange.Split('/');
            Prefix = Int32.Parse(ipRepr[1]);
            BaseAddress = ipRepr[0];
            var ipPart = ipRepr[0].Split('.');

            byte[] bytes = new byte[4];
            bytes[0] = Byte.Parse(ipPart[3]);
            bytes[1] = Byte.Parse(ipPart[2]);
            bytes[2] = Byte.Parse(ipPart[1]);
            bytes[3] = Byte.Parse(ipPart[0]);

            Bits = new BitArray(bytes);

            return true;
        }

        public List<IPAddress> GetAllIpAddresses() {
            List<IPAddress> ipAddressList = new List<IPAddress>();
            
            BitArray BitsClone = new BitArray(32);

            BitsClone = (BitArray)Bits.Clone();

            for (int i = 0; i <= 31-Prefix; i++)
            {
                BitsClone.Set(i, false);
            }

            do
            {
                byte[] ipBytes = new byte[4];
                BitsClone.CopyTo(ipBytes, 0);
                ipBytes = ipBytes.Reverse().ToArray();
                ipAddressList.Add(new IPAddress(ipBytes));

            } while (IncreaseBit(BitsClone, Prefix));

            return ipAddressList;
        }

        public IPAddress GetStartIpAddress() {

            BitArray BitsClone = new BitArray(32);

            BitsClone = (BitArray)Bits.Clone();

            for (int i = 0; i <= 31 - Prefix; i++)
            {
                BitsClone.Set(i, false);
            }

            byte[] ipBytes = new byte[4];
            BitsClone.CopyTo(ipBytes, 0);
            var ipAddress = new IPAddress(ipBytes.Reverse().ToArray());

            return ipAddress;
        }

        public IPAddress GetEndIpAddress()
        {

            BitArray BitsClone = new BitArray(32);

            BitsClone = (BitArray)Bits.Clone();

            for (int i = 0; i <= 31-Prefix; i++)
            {
                BitsClone.Set(i, true);
            }

            byte[] ipBytes = new byte[4];
            BitsClone.CopyTo(ipBytes, 0);
            var ipAddress = new IPAddress(ipBytes.Reverse().ToArray());

            return ipAddress;
        }

        public List<IPRange> GetSmallerRanges(int prefix) {
            if (prefix < Prefix)
            {
                throw new ArgumentException("Prefix should be bigger then original IPRange prefix");
            }

            if (prefix == Prefix)
            {
                return new List<IPRange>() { new IPRange() { Prefix = this.Prefix, Bits = (BitArray)this.Bits.Clone() } };
            }

            List<IPRange> ipRangeList = new List<IPRange>();
            int Count = 1;

            Count <<= (prefix - Prefix); // 25 - 24

            BitArray BitsClone = new BitArray(32);
            BitsClone = (BitArray)Bits.Clone();

            for (int i = 0; i <= 31-Prefix; i++)
            {
                BitsClone.Set(i, false);
            }

            for (int i = 0; i < Count; i++)
            {
                IPRange ipRange = new IPRange();
                ipRange.Prefix = prefix;
                ipRange.Bits = (BitArray)BitsClone.Clone();

                byte[] ipBytes = new byte[4];
                BitsClone.CopyTo(ipBytes, 0);
                var ipAddress = new IPAddress(ipBytes.Reverse().ToArray());

                ipRange.BaseAddress = ipAddress.ToString();

                ipRangeList.Add(ipRange);

                for (int idx = 31 - prefix + 1; idx < 31 - Prefix + 1; idx++)
                {
                    BitsClone[idx] ^= true;
                    if (BitsClone[idx])
                    {
                        break;
                    }
                }

            }

            return ipRangeList;
        }

        private bool IncreaseBit(BitArray bitsClone, int prefix)
        {
            bool IsMax = true;
            for (int i = 0; i <= 31 - Prefix; i++)
            {
                IsMax &= bitsClone[i];
            }

            if (IsMax)
            {
                return false;
            }

            for (int i = 0; i <= 31-Prefix; i++)
            {
                bitsClone[i] ^= true;
                if (bitsClone[i])
                {
                    return true;
                }
            }

            return true;
        }
    }
}
