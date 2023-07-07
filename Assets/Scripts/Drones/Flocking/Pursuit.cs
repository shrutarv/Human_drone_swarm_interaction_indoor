
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pursuit
{
    public Transform leaderTransform;
    public Transform droneTransform;
    public Vector3? target = null;
    public float targetReachedDistance = 0.2f;


    public void Init(Transform leaderTransform, Transform droneTransform)
    {
        this.leaderTransform = leaderTransform;
        this.droneTransform = droneTransform;

    }

    public void SetTarget(Vector3? target)
    {
        this.target = target;
    }

    public float GetDistanceFromLeaderToTarget()
    {
        if (target.HasValue)
        {
            return Vector3.Distance(leaderTransform.position, target.Value);
        }
        else
        {
            return float.PositiveInfinity;
        }
    }

    public bool HasDroneReachedTarget()
    {
        if (target.HasValue)
        {
            return GetDistanceFromDroneToTarget() < targetReachedDistance;
        }
        else
        {
            return false;
        }
    }

    public float GetDistanceFromDroneToTarget()
    {
        if (target.HasValue)
        {
            return Vector3.Distance(droneTransform.position, target.Value);
        }
        else
        {
            return float.PositiveInfinity;
        }
    }

    public bool HasLeaderReachedTarget()
    {
        if (target.HasValue)
        {
            return GetDistanceFromLeaderToTarget() < targetReachedDistance;
        }
        else
        {
            return false;
        }
    }

    public Vector3? GetPursuitVector()
    {
        if (target.HasValue)
        {
            var direction = target.Value - leaderTransform.position;
            direction.Normalize();
            return direction;
        }
        else
        {
            return null;
        }
         
    }
}

