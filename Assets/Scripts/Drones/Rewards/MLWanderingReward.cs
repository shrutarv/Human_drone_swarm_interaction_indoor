using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLWanderingReward
{
    public Transform transform;

    public float lastAngle;

    public float directionChangeInterval = 1f;
    public float timeSinceLastDirectionChange = 0;

    public Dictionary<Tuple<int, int>, int> visitedSectorsDict = new Dictionary<Tuple<int, int>, int>();

    public void Init(Transform transform)
    {
        this.transform = transform;
        lastAngle = transform.rotation.eulerAngles.y;
    }

    public float ComputeFrequentDirectionChangeReward(float deltaTime)
    {
        var angleDiff = Math.Abs(transform.rotation.eulerAngles.y - lastAngle);
        lastAngle = transform.rotation.eulerAngles.y;

        if (angleDiff > 0.01f)
        {
            timeSinceLastDirectionChange = 0;
        }

        if (timeSinceLastDirectionChange > directionChangeInterval)
        {            
            return -0.1f;
        }

        timeSinceLastDirectionChange += deltaTime;
        return 0f;
        
    }

    public float ComputeMaximumVisitedAreaReward()
    {
        Tuple<int, int> sector = new Tuple<int, int>((int)transform.position.x, (int)transform.position.z);
        if (visitedSectorsDict.ContainsKey(sector))
        {
            return -0.01f;
        } else
        {
            visitedSectorsDict.Add(sector, 1);
            return 0.2f;
        }
        
    }

    public void Reset()
    {
        visitedSectorsDict.Clear();
    }

}
