using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public struct PointWithColor
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Color Color { get; set; }


        public PointWithColor(float x, float y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }

