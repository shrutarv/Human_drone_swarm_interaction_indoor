using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassicConsoleApp1
{
    public class ViconTrackingObject : TrackingObject
    {
        private bool _occluded = false;
        private string _segmentName = null;

        public override string ToString()
        {
            return string.Format("{0} pos: x {1} y {2} z {3}", SegmentName, ROSposition().x, ROSposition().y, ROSposition().z);
        }

        public ViconTrackingObject(string name) : base(name) { }
        public string SegmentName
        {
            get
            {
                return _segmentName;
            }
            set
            {
                _segmentName = value;
            }
        }
        public override string Id
        {
            get
            {
                return SegmentName == null ? SubjectName : SubjectName + "_" + SegmentName;
            }
        }

        public override bool Visible
        {
            get
            {
                return ShouldDraw && TrackerX != 0f && TrackerY != 0f && !Occluded;
            }
        }

        public override float LaserX
        {
            get
            {
                return 1700f + (TrackerX + 75f) * 2.95f;
            }
        }

        public override float LaserY
        {
            get
            {
                return -15500 + (TrackerY + 4060f) * 2.95f;
            }
        }

        public bool Occluded
        {
            get
            {
                return _occluded;
            }
            set
            {
                _occluded = value;
            }
        }
    }
}
