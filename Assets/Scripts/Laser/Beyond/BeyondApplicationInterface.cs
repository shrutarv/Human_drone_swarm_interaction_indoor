using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace BeyondApi
{
    public class BeyondApplicationInterface
    {
        private static readonly object Lock = new object();

        private const string SkdImageWndClass = "BeyondSDKWndCls";
        private const string SkdCoreWndClass = "BeyondSDKCoreWndCls";

        // ReSharper disable InconsistentNaming
        // ReSharper disable ArrangeTypeMemberModifiers
        // ReSharper disable UnusedMember.Local
        const int NO_WINDOW = -1;

        const int CDT_SINGLE_FRAME = 1;
        const int CDT_CREATE_ZONE_IMG = 2;
        const int CDT_CREATE_SCAN_IMG = 3;
        const int CDT_DELETE_ZONE_IMG = 4;
        const int CDT_DELETE_SCAN_IMG = 5;
        const int CDT_DMX = 6;
        const int CDT_KINECT = 7;
        const int CDT_CHANNEL = 8;

        const int WM_COPYDATA = 0x004A;
        const int WM_USER = 0x0400;
        const int WM_GETVERSION = WM_USER + 1;
        const int WM_GETSCANCOUNT = WM_USER + 2;
        const int WM_GETPZCOUNT = WM_USER + 3;
        const int WM_GETTIMECODE = WM_USER + 4;
        const int WM_SETTIMECODE = WM_USER + 5;
        const int WM_MIDI_IN = WM_USER + 6;
        const int WM_MIDI_OUT = WM_USER + 7;
        const int WM_ENABLE_LASER = WM_USER + 8;
        const int WM_DISABLE_LASER = WM_USER + 9;

        const int WM_BLACKOUT = WM_USER + 10;

        private const int MaxNumberOfPoints = 8192;
        // ReSharper restore ArrangeTypeMemberModifiers
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local


        public static IntPtr IntPtrAlloc<T>(T param)
        {
            var retval = Marshal.AllocCoTaskMem(Marshal.SizeOf(param));
            Marshal.StructureToPtr(param, retval, false);
            return retval;
        }

        public static void IntPtrFree(IntPtr preAllocated)
        {
            if (IntPtr.Zero == preAllocated) throw (new Exception("Go Home"));
            Marshal.FreeCoTaskMem(preAllocated);
            // ReSharper disable once RedundantAssignment
            preAllocated = IntPtr.Zero;
        }

        public static string PointToString(TSdkImagePoint p)
        {
            return $"X: {p.X}, Y: {p.Y}, Color: {p.Color.ToString()}";
        }

        public static uint ConvertIntPtr(IntPtr ptr)
        {
            // ReSharper disable once InvertIf
            if (IntPtr.Size > 4)
            {
                // ReSharper disable once SuggestVarOrType_BuiltInTypes
                long tempLong =
                    (ptr.ToInt64() >> 32) <<
                    32; //shift it right then left 32 bits, which zeroes the lower half of the long
                return (uint) (ptr.ToInt64() - tempLong);
            }

            return (uint) ptr.ToInt32();
        }

        public static bool CreateZoneImage__Unsafe(int zone, string name)
        {
            var hWnd = User32Interop.FindWindow(SkdImageWndClass, null);
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }


            var msg = new TSdkCoreMessageText
                {
                    Signature = Encoding.ASCII.GetBytes("PANGOLIN"),
                    Param1 = zone,
                    Text = new byte[4169]
                };
                
                //Encoding.ASCII.GetBytes(name);
                Array.Copy(Encoding.ASCII.GetBytes(name), msg.Text, Encoding.ASCII.GetBytes(name).Length);
                msg.Text[Encoding.ASCII.GetBytes(name).Length + 1] = 0;

                var msgPtr = IntPtrAlloc(msg);
                var cds = new CopyDataStruct
                {
                    dwData = CDT_CREATE_ZONE_IMG,
                    cbData = Marshal.SizeOf(msg),
                    lpData = ConvertIntPtr(msgPtr)
                };

                var cdsPtr = IntPtrAlloc(cds);
                var messageReceived = ((int) User32Interop.SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ConvertIntPtr(cdsPtr))) != 0;
                IntPtrFree(cdsPtr);
                IntPtrFree(msgPtr);

            return messageReceived;
        }

        public bool CreateZoneImage(int zone, string name)
        {
            lock (Lock)
            {
                return CreateZoneImage__Unsafe(zone, name);                
            }
        }

        public static bool SendImageToFrame__Unsafe(string name, TSdkImagePoint[] points)
        {
            var hWnd = User32Interop.FindWindow(SkdImageWndClass, name);
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }

            const int scanrate = 100;
            var pointCount = Math.Min(points.Length, MaxNumberOfPoints);
            var zones = new byte[256];
            zones[0] = 1;
            zones[1] = 0;

            var frame = new TSdkImageFrame
            {
                Header = new TSdkImageHeader
                {
                    Signature = Encoding.ASCII.GetBytes("PANGOLIN"),
                    PointCount = pointCount,
                    SampleRate = 0,
                    ScanRate = scanrate
                },
                Zones = zones,
                Points = new TSdkImagePoint[8192]
            };
            Array.Copy(points, frame.Points, pointCount);

            var framePtr = IntPtrAlloc(frame);
            var cds = new CopyDataStruct
            {
                dwData = CDT_SINGLE_FRAME,
                cbData = Marshal.SizeOf(frame),
                lpData = ConvertIntPtr(framePtr)
            };

            var cdsPtr = IntPtrAlloc(cds);
            var messageReceived = (int) User32Interop.SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ConvertIntPtr(cdsPtr)) != 0;

            IntPtrFree(cdsPtr);
            IntPtrFree(framePtr);
            
            return messageReceived;
        }

        public static bool SendImageToFrame(string name, TSdkImagePoint[] points)
        {
            lock (Lock)
            {
                return SendImageToFrame__Unsafe(name, points);
            }
        }

        public static bool ImageExists(string name)
        {
            var hWnd = User32Interop.FindWindow(SkdImageWndClass, name);
            return hWnd != IntPtr.Zero;
        }

        public static int GetVersion()
        {
            var hWnd = User32Interop.FindWindow(SkdCoreWndClass, "");
            if (hWnd == IntPtr.Zero)
            {
                return NO_WINDOW;
            }

            return User32Interop.SendMessage(hWnd, WM_GETVERSION, IntPtr.Zero, 0).ToInt32();
        }

        /*
static void Main(string[] args)
{
    var header = new TSdkImageHeader();
    header.Signature = Encoding.ASCII.GetBytes("PANGOLIN");
    Console.WriteLine("Size of {0}: {1}", nameof(TSdkImageHeader), Marshal.SizeOf(header));

    var point = new TSdkImagePoint();
    Console.WriteLine("Size of {0}: {1}", nameof(TSdkImagePoint), Marshal.SizeOf(point));

    var frame = new TSdkImageFrame();
    Console.WriteLine("Size of {0}: {1}", nameof(TSdkImageFrame), Marshal.SizeOf(frame));

    Console.WriteLine("IntPtr.Size: {0}", IntPtr.Size);
    //IntPtr ptr = new IntPtr()


    // BeyondAPI p = new BeyondAPI();
    // p.Start();
}

private void Start()
{
    var imageName = "Moritz Test";
    Console.WriteLine("Beyond Version: {0}", GetVersion());
    Console.WriteLine("Image {0} exists: {1}", imageName, ImageExists(imageName));

    float xMov = 0000;
    float yMov = 0;

    Console.WriteLine("Cursor x: {0}, y: {1}", xMov, yMov);
    var color = Color.GreenYellow;

    var points = new TSdkImagePoint[4];
    points[0] = TSdkImagePoint.Create(xMov + 1f, yMov + 1f, Color.Black, 1);
    points[1] = TSdkImagePoint.Create(xMov + 1f, yMov + 1f, color, 1);
    points[2] = TSdkImagePoint.Create(xMov + 1f, yMov + 10000f, color, 1);
    points[3] = TSdkImagePoint.Create(xMov + 10000f, yMov + 10000f, color, 1);

    Console.WriteLine("Create Zone Image with name {0}: {1}", imageName, CreateZoneImage(0, imageName));
    Console.WriteLine("Send Image Result: {0}", SendImageToFrame(imageName, points));
    Console.WriteLine("How many bytes? {0}", Encoding.ASCII.GetBytes("PANGOLIN").Length);
}
*/
    }
}