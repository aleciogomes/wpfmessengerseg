using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Management;
using System.Net;

namespace WPFMessengerSeg.Core.util
{
    public static class Win32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        const uint FLASHW_ALL = 3;
        const uint FLASHW_TIMERNOFG = 12; 

        public static void Flash(Window w)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = new WindowInteropHelper(w).Handle; ;
            fi.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fi.uCount = 10;
            fi.dwTimeout = 0;
            FlashWindowEx(ref fi);
        }

        private static string motherBoardID = String.Empty;
        public static string MotherBoardID
        {
            get
            {
                if(string.IsNullOrEmpty(motherBoardID))
                {
                    motherBoardID = GetMotherBoardID();
                }
                    
                return motherBoardID;

            }
        }

        private static string GetMotherBoardID()
        {
            string mbID = String.Empty;

            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
            mbsList = mbs.Get();

            foreach (ManagementObject mo in mbsList)
            {
                mbID = mo["SerialNumber"].ToString();
                break;
            }

            return mbID;
        }

        public static string GetIP()
        {
            string ipAddress = String.Empty;
            string host = String.Empty;

            host = Dns.GetHostName();

            IPHostEntry ipEntry = Dns.GetHostByName(host);
            IPAddress[] addr = ipEntry.AddressList;

            foreach(IPAddress address in addr){
                ipAddress = address.ToString();
                break;
            }

            return ipAddress;
        }


    }
}
