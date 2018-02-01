
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace BlueprintReality.MixCast
{
    public class MachineId
    {
        public string MachineName
        {
            get {
                return Environment.MachineName;
            }
        }

        public string WindowsProductId { get; private set; }

        public string[] MACAddresses { get; private set; }

        public MachineId()
        {
            MACAddresses = MapNetworkInterfaces(GetMACAddress);
            WindowsProductId = GetWindowsProductId();
        }

        public bool IsSameComputer(MachineId other)
        {
            return string.Compare(this.ToString(), other.ToString()) == 0;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("Machine Name: " + MachineName);
            result.AppendLine("Windows Product Id: " + WindowsProductId);
            foreach (var str in MACAddresses) { result.AppendLine("MAC Address: " + str); }
            return result.ToString();
        }

        public byte[] ComputeHash()
        {
            using (SHA1 hashProvider = SHA1.Create()) {
                return hashProvider.ComputeHash(Encoding.UTF8.GetBytes(this.ToString()));
            }
        }

        public delegate string NetworkInterfaceToString(NetworkInterface adapter);

        private string[] MapNetworkInterfaces(NetworkInterfaceToString mapFunc)
        {
            List<string> result = new List<string>();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces()) {
                string address = mapFunc(adapter);
                if (!string.IsNullOrEmpty(address)) {
                    result.Add(address);
                }
            }

            return result.ToArray();
        }

        private string GetMACAddress(NetworkInterface adapter)
        {
            return adapter.GetPhysicalAddress().ToString();
        }

        private string GetDHCPAddress(NetworkInterface adapter)
        {
            var addresses = adapter.GetIPProperties().DhcpServerAddresses;
            return addresses.Count > 0 ? addresses[0].ToString() : null;
        }

        private string GetWindowsProductId()
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion");
            return reg != null ? reg.GetValue("ProductId", null) as string : null;
        }
    }
}
