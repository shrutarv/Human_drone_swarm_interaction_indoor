using System.Runtime.InteropServices;

namespace BeyondApi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CopyDataStruct
    {
        public uint dwData;
        public int cbData;
        public uint lpData;
    }
}