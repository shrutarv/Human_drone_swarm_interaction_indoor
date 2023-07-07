using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cohesion
{
    public int Id { get; set; }
    public float MinDist { get; set; } = 2.0f;
    public float MaxDist { get; set; } = 3.0f;
    public float Scalar { get; set; } = 1;

    public Transform Transform;


    public void Init(int id, Transform transform)
    {
        Id = id;
        this.Transform = transform;
    }

    //create an empty vector to store the result of the flocking rule.
    public Vector3 GetCohesionVector(List<Boid> boids)
    {
        Vector3 result = Vector3.zero;
        int count = 0;
        foreach (var boid in boids)
        {
            if (boid != null && Id != boid.Id)
            {

                if (boid.Leader != null)
                {
                    Vector3 difLeader = boid.Leader.position - Transform.position;
                    float distLeader = Vector3.Magnitude(difLeader);
                    if (distLeader <= MaxDist && distLeader >= MinDist)
                    {
                        result += boid.Leader.position;
                        count++;
                    }
                }

            }
        }

        if (count > 0)
        {
            result /= count;
            Vector3 dir = result - Transform.position;
            dir.Normalize();
            return dir * Scalar;
        }
        return result;
        /* int count           = 0;
         for ( int i = 0; i < boids.Count; ++i )
         {
             //don't do anything for self.
             if ( i != current )
             {
                 Boid b              = boids[current];
                 Boid other          = boids[i];
                 Vector3 otherPos    = other.position;
                 //get the vector between the current boid and the neighbor boid.
                 Vector3 dif         = otherPos - b.position;

                 //get the squared magnitude. Only update the velocity if the magnitude is bigger than the minimum distance and smaller than the maximum distance.
                 float dist          = Vector3.SqrMagnitude( dif );
                 if ( dist <= maxDist * maxDist && dist >= minDist * minDist )
                 {
                     //normalize the difference and add it to the result.
                     result          += otherPos;
                     count++;
                 }
             }
         }

         if ( count > 0 )
         {
             //get the average 
             result /= count;
             Vector3 dir = result - boids[current].position;
             return dir.normalized * scalar;
         }*/
    }
}