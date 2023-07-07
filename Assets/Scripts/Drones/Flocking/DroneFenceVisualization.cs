using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneFenceVisualization : MonoBehaviour
{

    Fencing3D fencing;
    public Vector3 center = Vector3.zero;
    public float xRange;
    public float zRange;

    // Start is called before the first frame update
    void Start()
    {
        fencing = new Fencing3D();
        xRange = fencing.xMax - fencing.xMin;
        var xMiddle = fencing.xMin + xRange / 2;
        zRange = fencing.zMax - fencing.zMin;
        var zMiddle = fencing.zMin + zRange / 2;
        center = new Vector3(xMiddle, 0, zMiddle);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = center;
        transform.localScale = new Vector3(xRange, 1, zRange);
    }
}
