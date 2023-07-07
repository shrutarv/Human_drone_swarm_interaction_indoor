using System;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Local

namespace BeyondApi
{
    internal static class User32Interop
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport(ExternDll.User32)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wparam, uint lparam);

        [DllImport(ExternDll.User32, EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
    }
}