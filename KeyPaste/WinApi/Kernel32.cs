using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KeyPaste.WinApi
{
    class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
    }
}
