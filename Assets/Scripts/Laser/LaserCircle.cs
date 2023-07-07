using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static iTween;

class LaserCircle : LaserLineRenderer
{
    public float Size { get; set; } = 1;

    public int numberOfPoints = 8;
    private float angleFraction;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();

        angleFraction = 360 / numberOfPoints;

        float x, y;
        for (int i = 0; i < numberOfPoints; i++)
        {
            float degree = i * angleFraction;
            double radian = DegreeToRadian(degree);
            x = Size * (float)Math.Cos(radian);
            y = Size * (float)Math.Sin(radian);
            points.Add(new Vector3(x, 0, y));
        }
        x = Size * (float)Math.Cos(0);
        y = Size * (float)Math.Sin(0);
        points.Add(new Vector3(x, 0, y));

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoPulse()
    {
        System.Collections.Hashtable hash =
                  new System.Collections.Hashtable
                  {
                      { "amount", new Vector3(0.35f, 0, 0.35f) },
                      { "time", 0.5f },
                      { "looptype", LoopType.loop }
                  };
        iTween.PunchScale(gameObject, hash);
    }


    private double RadianToDegree(double angle)
    {
        return angle * (180.0 / Math.PI);
    }

    private double DegreeToRadian(double angle)
    {
        return angle * Math.PI / 180.0;
    }
}

