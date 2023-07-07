using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEdge : LaserLineRenderer
{
    public Transform source;
    public Transform sink;

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();


        points.Add(source.position);
        points.Add(sink.position);


    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();

        var diff = sink.position - source.position;
        points[0] = source.position;
        points[1] = sink.position;

    }
}
