using UnityEngine;

public class Wandering
{
    public float velocity;
    public float directionChangeInterval = 1f;
    public float maxHeadingChange = 60;

    float heading;
    public Quaternion targetRotation;
    float directionChangeElapsed = 0;

    public Transform transform;
    public Vector3 forward;


    public void Init(Transform transform) {
        this.transform = transform;
        heading = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(0, heading, 0);
    }

    public void Update(float deltaTime) {

        if(directionChangeElapsed > directionChangeInterval) {
            var floor = transform.eulerAngles.y - maxHeadingChange;
            var ceil = transform.eulerAngles.y + maxHeadingChange;
            heading = Random.Range(floor, ceil);
            targetRotation = Quaternion.Euler(0, heading, 0);
            directionChangeElapsed = 0;
        }
        directionChangeElapsed += deltaTime;

    }

}

