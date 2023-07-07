using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLLeaderController : MonoBehaviour
{
    public Queue<Transform> waypoints = new Queue<Transform>();
    public Vector3 targetPosition;
    public bool directFlight = false;
    public DroneController droneController;
    public Transform drone;
    public float velocity = 1.2f;

    public bool isWanderingActive = false;
    public Boid boid;
    public List<Boid> boids;
    public Wandering wanderingBehavior = new Wandering();
    public Fencing fencingBehavior = new Fencing();
    public Separation separationBehavior = new Separation();
    public bool isCohesionActive = false;
    public Cohesion cohesionBehavior = new Cohesion();
    public bool isAlignmentActive = false;
    public Alignment alignmentBehavior = new Alignment();

    public bool leadingDroneActive = false;
    internal float droneTime = 0;

    public Rigidbody rb;

    public Quaternion nextMLRotation = new Quaternion();
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        wanderingBehavior.Init(transform);
        fencingBehavior.Init(transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        

        
    }

    void FixedUpdate()
    {
        var dronepos = transform.position;
        //dronepos.y = droneController.transform.position.y;
        transform.position = dronepos;


        Quaternion nextRotation = transform.rotation;
        

        nextRotation = nextMLRotation;

        rb.MoveRotation(nextRotation);

        //always use fencing after wandering
        var fencingVector = fencingBehavior.GetCenterVectorWhenAtFence();
        if (fencingVector.HasValue)
        {
            rb.MoveRotation(Quaternion.LookRotation(fencingVector.Value));
        }

        //var distanceBetweenLeaderandBody = Vector3.Distance(drone.position, transform.position);

        
        //if (distanceBetweenLeaderandBody < 0.8f) // stick of the carrot
        //{
            var forward = transform.TransformDirection(Vector3.forward);
            rb.MovePosition(transform.position + forward * velocity * Time.deltaTime);
        //}

        /*if (leadingDroneActive)
        {
            droneTime += Time.deltaTime;
            if (droneTime > 0.4f)
            {
                if (Vector3.Distance(drone.position, transform.position) > 0.1f)
                {
                    droneController.targetPosition = transform.position;
                    droneController.GetConnection().MoveTo(droneController.id, 0, ComputeDuration(drone.position, transform.position), transform.position.x, transform.position.z, droneController.height, 0);
                }
                droneTime = 0;
            }
        }*/
    }

    internal float ComputeDuration(Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        return Math.Max((distance / velocity), 1.4f);
    }


    void OnDisable()
    {
        boids.Remove(boid);
    }
}
