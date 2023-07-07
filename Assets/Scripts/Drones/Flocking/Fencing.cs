using UnityEngine;

public class Fencing {

    public Vector3 center = Vector3.zero;
    public float xMax = 9f;
    public float xMin = -2f;
    public float zMax = 5f;
    public float zMin = -3f;

    public Transform transform;


    public void Init(Transform transform) {
        this.transform = transform;
        var xRange = xMax - xMin;
        var xMiddle = xMin + xRange / 2;
        var zRange = zMax - zMin;
        var zMiddle = zMin + zRange / 2;
        center = new Vector3(xMiddle, 0, zMiddle);
    }

    public Vector3? GetCenterVectorWhenAtFence() {

        var x = transform.position.x;
        var z = transform.position.z;
        center.y = transform.position.y;

        if (x > xMax || x < xMin || z > zMax || z < zMin)
        {
            var result = center - transform.position;
            result.Normalize();
            return result;
        } else
        {
            return null;
        }

    }

    public Vector3 GetRandomPosition()
    {
        var x = Random.Range(xMin + 0.5f, xMax - 0.5f);
        var z = Random.Range(zMin + 0.5f, zMax - 0.5f);
        return new Vector3(x, 0, z);
    }

}

