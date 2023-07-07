using System;
using System.IO.Pipes;
using System.Runtime.InteropServices;

namespace BeyondApi
{
    /// <summary>
    /// Wrapper around the C API.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: Runs only under 32-bit compilation because of Beyond SDK
    /// </remarks>
    public static class Beyond
    {
#pragma warning disable IDE1006 // Naming Styles

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbCreate();

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbDestroy();

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbGetDllVersion();

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbBeyondExeStarted();

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbBeyondExeReady();

/*
        [DllImport(Beyond, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ldbGetBeyondVersion();
*/

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbGetProjectorCount();

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbGetZoneCount();

/*
        [DllImport(Beyond, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ldbEnableLaserOutput();
*/

/*
        [DllImport(Beyond, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ldbDisableLaserOutput();
*/

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbCreateZoneImage(int aZoneIndex, string aImageName);

/*
        [DllImport(Beyond, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ldbDeleteZoneImage(string aImageName);
*/

        [DllImport(ExternDll.Beyond, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ldbSendFrameToImage(string aImageName, int aCount, TSdkImagePoint[] aFrame,
            byte[] aZones, int aRate);

#pragma warning restore IDE1006 // Naming Styles
    }
}