using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encircling
{
    public int Id { get; set; }
    public float MinDist { get; set; } = 1.0f;
    public float MaxDist { get; set; } = 1.8f;

    public Transform Transform;

    public void Init(int id, Transform transform)
    {
        Id = id;
        this.Transform = transform;
    }


    public Vector3 GetEncirclingPosition(Boid boid, float angle, float distance)
    {
        Vector3 result = Vector3.zero;


            if (boid != null && Id != boid.Id)
            {

                if (boid.Leader != null)
                {
                    Vector3 difLeader = Transform.position - boid.Leader.position;
                    float distLeader = Vector3.Magnitude(difLeader);
                    if (distLeader <= MaxDist && distLeader >= MinDist)
                    {
                        result += boid.Leader.forward;
                    }
                }
            }
       
        return result;
    }

}
