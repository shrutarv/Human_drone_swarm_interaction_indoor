
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wandering3D
{
    public float velocity;
    public float directionChangeInterval = 1f;
    public float maxYawChange = 60;
    public float minPitch = -45f;
    public float maxPitch = 45f;

    float yaw;
    float pitch;
    public Quaternion targetRotation;
    float directionChangeElapsed = 0;

    public Transform transform;
    public Vector3 forward;


    public void Init(Transform transform) {
        this.transform = transform;
        yaw = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(0, yaw, 0);
        
    }

    public void Update(float deltaTime) {


        if(directionChangeElapsed > directionChangeInterval) {
            var floorYaw = transform.eulerAngles.y - maxYawChange;
            var ceilYaw = transform.eulerAngles.y + maxYawChange;

            yaw = Random.Range(floorYaw, ceilYaw);

            pitch = Random.Range(minPitch, maxPitch);

            targetRotation = Quaternion.Euler(pitch, yaw, 0);

            directionChangeElapsed = 0;
        }
        directionChangeElapsed += deltaTime;

    }

}

