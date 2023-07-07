using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public abstract class LaserGraphicsShape
    {
        public LaserBehaviour LaserBehaviour;
        public string Label { get; set; } = "";
        public Boolean Active { get; set; } = true;
        public Color Color { get; set; } = Color.white;
        public PointWithColor[] LaserPath { get; set; }
        public int PointCount { get; set; }
        public abstract void Update();
}

