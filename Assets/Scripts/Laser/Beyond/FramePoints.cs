using System.Runtime.InteropServices;
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace BeyondApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FramePoints
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ImgePointCount)]
        public TSdkImagePoint[] Points;
        public int Count;
        
        public const int ImgePointCount = 8192;

        public static byte[] ToBytes(FramePoints framePoints)
        {
            var size = Marshal.SizeOf(framePoints);
            var arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(framePoints, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static FramePoints FromBytes(byte[] arr)
        {
            var frame = new FramePoints();
            var size = Marshal.SizeOf(frame);
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);
            frame = (FramePoints) Marshal.PtrToStructure(ptr, frame.GetType());
            Marshal.FreeHGlobal(ptr);
            return frame;
        }

        public static int FrameSize => Marshal.SizeOf(new FramePoints());
    }
}