using System.Runtime.InteropServices;

namespace BeyondApi
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    internal struct TSdkImageFrame
    {
        public TSdkImageHeader Header;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] Zones;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8192)]
        public TSdkImagePoint[] Points;
    }
}