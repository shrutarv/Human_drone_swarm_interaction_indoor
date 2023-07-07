using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClassicConsoleApp1
{
    public abstract class TrackingObject
    {
        public string SubjectName { get; set; }
        public abstract string Id { get; }
        public Types Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public bool ShouldDraw { get; set; }
        public abstract bool Visible { get; }
        public float TrackerX { get; set; }
        public float TrackerY { get; set; }
        public float TrackerZ { get; set; }
        public Quaternion Rotation { get; set; }
        public float RotationX = 0f;
        public float RotationY = 0f;
        public float RotationZ = 0f;
        public float RotationW = 0f;
        private Types _type = Types.None;

        public abstract float LaserX { get; }
        public abstract float LaserY { get; }
        public float ROSFromLaserX
        {
            get
            {
                return (((-1700f + LaserX) / 2.95f) - 75f) / 1000f;
            }
        }

        public float ROSFromLaserY
        {
            get
            {
                return (((15500f + LaserY) / 2.95f) - 4060f) / 1000f;
            }
        }

        public Vector3 ROSposition()
        {
            return new Vector3(TrackerX / 1000f, TrackerY / 1000f, TrackerZ / 1000f);
        }

        public List<LaserGraphicsShape> Shapes { get; private set; }
        public List<AbstractBehaviour> Behaviours { get; private set; }

        public TrackingObject(string subjectName)
        {
            if (subjectName == null)
            {
                throw new ArgumentNullException("subjectName");
            }
            SubjectName = subjectName;
            ShouldDraw = true;
            Shapes = new List<LaserGraphicsShape>();
            Behaviours = new List<AbstractBehaviour>();
            TrackerX = 0f;
            TrackerY = 0f;
        }

        public void Update(long millis)
        {
            foreach (AbstractBehaviour beh in Behaviours)
            {
                beh.Update(millis);
            }
        }

        public enum Types
        {
            None,
            Drone,
            Robot,
            Box,
            Target,
            Controller,
            Visualization,
        }

    }
}
