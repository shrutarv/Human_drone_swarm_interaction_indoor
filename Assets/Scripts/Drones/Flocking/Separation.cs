using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Separation
{
    public int Id { get; set; }
    public float MinDist { get; set; } = 0.01f;
    public float MaxDist { get; set; } = 0.6f;
    public float Scalar { get; set; } = 1;

    public Transform Transform;


    public void Init(int id, Transform transform)
    {
        Id = id;
        this.Transform = transform;
    }

    public Vector3 GetSeparationVector( List<Boid> boids )
    {
        Vector3 result      = Vector3.zero;
        int count           = 0;
        foreach ( var boid in boids )
        {

            if (boid != null && Id != boid.Id )
            {

                if (boid != null && boid.Leader != null)
                {
                    var leaderXZ = boid.Leader.position;
                    leaderXZ.y = Transform.position.y;
                    Vector3 difLeader = Transform.position - leaderXZ;
                    float distLeader = Vector3.Magnitude(difLeader);
                    if (distLeader <= boid.SeparationDistance)// && distLeader >= MinDist)
                    {
                        difLeader.Normalize();
                        result += difLeader / distLeader;
                        count++;
                    }
                }

                var bodyXZ = boid.Body.position;
                bodyXZ.y = Transform.position.y;
                Vector3 difBody    = Transform.position - bodyXZ;
                float distBody = Vector3.Magnitude( difBody );
                if (distBody <= boid.SeparationDistance) // && distBody >= MinDist )
                {
                    difBody.Normalize( );
                    result      += difBody / distBody;
                    count++;
                }
            }
        }

        if ( count > 0 )
        {
            result /= count;
            result.Normalize( );
            result.y = 0f;
            return result * Scalar;
        }
        return result;
    }
}