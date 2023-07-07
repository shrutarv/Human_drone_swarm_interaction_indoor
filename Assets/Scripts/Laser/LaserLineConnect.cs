using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static iTween;

class LaserLineConnect : LaserLineRenderer
{
    public float x, y = 0.0f; 

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();

        points.Add(new Vector3(0, 0, 0));

        var inverseTransformedPoint = gameObject.transform.InverseTransformPoint(new Vector3(x, 0, y));

        points.Add(inverseTransformedPoint);

        var tempTransform = new GameObject().transform;
        tempTransform.parent = transform;
        tempTransform.localRotation = Quaternion.LookRotation(Vector3.Normalize(inverseTransformedPoint), Vector3.up);

        //points.Add(transform.InverseTransformPoint(tempTransform.forward * 1f));

        //var x1 = (float)(0.5 * Math.Cos(DegreeToRadian(45)));
        //var y1 = (float)(0.5 * Math.Sin(DegreeToRadian(45)));
        //points.Add(new Vector3(x1, 0, y1));

        //points.Add(new Vector3(x, 0, y));

        //var x2 = (float)(0.5 * Math.Sin(DegreeToRadian(45)));
        //var y2 = (float)(0.5 * Math.Cos(DegreeToRadian(45)));
        //points.Add(new Vector3(x1, 0, y1));

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

