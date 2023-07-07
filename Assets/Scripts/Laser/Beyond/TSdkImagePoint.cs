using System;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace BeyondApi
{
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct TSdkImagePoint
    {
        public float X, Y, Z; // 32bit float point, Coordinate system -32K to +32K
        public Int32 Color; // RGB in Windows style
        public byte RepCount; // Repeat counter
        public byte Focus; // Beam brush reserved, leave it zero
        public byte Status; // bitmask - attributes
        public byte Zero; // Leave it zero

		public static TSdkImagePoint CreatePoint(float x, float y, Color32 c, byte status)
		{
			TSdkImagePoint point = new TSdkImagePoint();
			point.X = x;
			point.Y = y;
			point.Z = 0f;
			byte[] a = new byte[4];
			a[0] = c.r;
			a[1] = c.g;
			a[2] = c.b;
			a[3] = 0;

			point.Color = BitConverter.ToInt32(a, 0);
			point.RepCount = 0;    // Repeat counter
			point.Focus = 0;   // Beam brush reserved, leave it zero
			point.Status = status;    // bitmask - attributes
			point.Zero = 0;    // Leave it zero
			return point;
		}
	}

}