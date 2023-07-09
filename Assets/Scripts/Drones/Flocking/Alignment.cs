using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Alignment
{
    public int Id { get; set; }
    public float MinDist { get; set; } = 1.0f;
    public float MaxDist { get; set; } = 1.8f;
    public float Scalar { get; set; } = 1;

    public Transform Transform;

    public void Init(int id, Transform transform)
    {
        Id = id;
        this.Transform = transform;
    }

    public Vector3 GetAlignmentVector(List<Boid> boids)
    {
        Vector3 result = Vector3.zero;
        int count = 0;
        foreach (var boid in boids)
        {

            if (boid != null && Id != boid.Id)
            {

                if (boid.Leader != null)
                {
                    Vector3 difLeader = Transform.position - boid.Leader.position;
                    float distLeader = Vector3.Magnitude(difLeader);
                    if (distLeader <= MaxDist && distLeader >= MinDist)
                    {
                        result += boid.Leader.forward;
                        count++;
                    }
                }
            }
        }

        if (count > 0)
        {
            result /= count;
            result.Normalize();
            result.y = 0f;
            return result * Scalar;
        }
        return result;    
    }
}