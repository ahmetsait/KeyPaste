using System;
using System.Runtime.InteropServices;
using System.Text;

namespace KeyPaste.WinApi
{
    public static class Psapi
    {
        [DllImport("psapi.dll")]
        public static extern uint GetModuleBaseName(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);
    }
}
