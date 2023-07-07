using System.Runtime.InteropServices;
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace BeyondApi
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    internal struct TSdkCoreMessageText
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Signature; //  PANG OLIN,   yes.. simplest answer, 8 characters should have PANGOLIN

        public int Command;
        public int Param1;
        public int Param2;
        public int Param3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4169)] //  0:( Text    :array[0..4192-24] of char;);
        public byte[] Text;
    }
}