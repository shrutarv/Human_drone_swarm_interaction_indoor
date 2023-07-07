using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class LaserBox : LaserGraphicsShape
{
    public LaserBox() : this(Color.white) { }

    public LaserBox(Color _color)
    {
        this.Color = _color;
        //GraphicsPath path = new GraphicsPath();
        LaserPath = new PointWithColor[6];
        PointCount = 6;
        LaserPath[0] = new PointWithColor(0, 0, this.Color);
        LaserPath[1] = new PointWithColor(0, 0, this.Color);
        LaserPath[2] = new PointWithColor(0, 0, this.Color);
        LaserPath[3] = new PointWithColor(0, 0, this.Color);
        LaserPath[4] = new PointWithColor(0, 0, this.Color);
        LaserPath[5] = new PointWithColor(0, 0, this.Color);
    }

    public override void Update()
    {
        //Path.Reset();
        //Path.AddRectangle(new RectangleF(new PointF((float)TrackingObject.LaserX, (float)TrackingObject.LaserY), new SizeF(100f, 100f)));

        float height = 1500f;
        float width = 1500f;

        float x = LaserBehaviour.LaserX;
        float y = LaserBehaviour.LaserY;
        LaserPath[0].Color = Color;
        LaserPath[0].X = x - width / 2;
        LaserPath[0].Y = y - height / 2;

        LaserPath[1].Color = Color;
        LaserPath[1].X = x - width / 2;
        LaserPath[1].Y = y + height / 2;

        LaserPath[2].Color = Color;
        LaserPath[2].X = x + width / 2;
        LaserPath[2].Y = y + height / 2;

        LaserPath[3].Color = Color;
        LaserPath[3].X = x + width / 2;
        LaserPath[3].Y = y - height / 2;

        LaserPath[4].Color = Color;
        LaserPath[4].X = x - width / 2;
        LaserPath[4].Y = y - height / 2;

        LaserPath[5].Color = Color;
        LaserPath[5].X = x - width / 2;
        LaserPath[5].Y = y - height / 2;

    }
}

