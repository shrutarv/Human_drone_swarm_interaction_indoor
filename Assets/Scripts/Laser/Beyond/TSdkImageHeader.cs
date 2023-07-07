using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace BeyondApi
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    // ReSharper disable once InconsistentNaming
    internal struct TSdkImageHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Signature; //  PANG OLIN,   yes.. simplest answer, 8 characters should have PANGOLIN

        public int PointCount; // reflect number of points in frame
        public int SampleRate; // In points. 30000 means 30KPPS. Leave 0 if not used
        public int ScanRate; // Percens of default sample rate. Range 10..400. Leave 0 if not used
        public int Tag; // 
    }
}