using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateTrajectoryAction : MonoBehaviour, DroneAction
{
    public DroneController controller;
    public AutoPilot autoPilot;
    public readonly Guid id = Guid.NewGuid();
    public GameObject drone;

    public float vMax = 5;
    public float aMax = 5;
    public string groupMask = "0b00000001";

    public float timeLeft;
    public float nearlyFinishedTime;
    public bool nearlyFinished = false;
    public bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        autoPilot = transform.parent.gameObject.GetComponent<AutoPilot>();
        drone = autoPilot.GetDrone();
        controller = autoPilot.GetController();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timeLeft -= Time.deltaTime;
            autoPilot.TimeLeft(id, timeLeft);

            if (timeLeft <= 0)
            {
                autoPilot.FinishedAction(id);
                running = false;
                nearlyFinished = false;
                timeLeft = 0;
            }
        }
    }

    public List<Vector3> GetTargetPointList()
    {
        var list = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var point = transform.GetChild(i).gameObject.GetComponent<DroneAction>().TargetPoint();
            if (point.HasValue)
            {
                list.Add(point.Value);
            }
        }
        return list;
    }

    public void Execute()
    {
        if (!running)
        {
            timeLeft = autoPilot.trajectoryUploadDuration;

            autoPilot.homeLandPosition = drone.transform.position;
            autoPilot.homeHoverPosition = drone.transform.position;
            autoPilot.homeHoverPosition.y = controller.height;
            autoPilot.isHomePositionInitialized = true;

            List<Vector3> waypoints = GetTargetPointList();

            waypoints.Insert(0, autoPilot.homeHoverPosition);

            //if number of waypoints is even then compute an extra point between first and second point in list
            //this is a specialty of crazyswarm trajectories.  Only uneven number of waypoints a correctly handled
            if(waypoints.Count % 2 == 0)
            {
                waypoints.Insert(1, GetIntermediatePoint(waypoints[0], waypoints[1]));
            }

            autoPilot.GetConnection().CreateTrajectory(autoPilot.id, vMax, aMax, groupMask, waypoints);
            running = true;
        }
    }

    public Vector3 GetIntermediatePoint(Vector3 first, Vector3 second)
    {
        return Vector3.Lerp(first, second, 0.5f);
    }

    public Guid Id()
    {
        return id;
    }

    public Vector3? TargetPoint()
    {
        return null;
    }

    public GameObject TargetGameObject()
    {
        return null;
    }

    public void Trigger(Collider col)
    {
        //nothing
    }


    public GameObject GetEndPointGameObject()
    {
        if(transform.childCount > 0)
        {
            return transform.GetChild(transform.childCount - 1).gameObject.GetComponent<DroneAction>().TargetGameObject();
        }
        else
        {
            return null;
        }
    }

    public Vector3? GetEndPoint()
    {
        if (transform.childCount > 0)
        {
            return transform.GetChild(transform.childCount - 1).gameObject.GetComponent<DroneAction>().TargetPoint();
        }
        else
        {
            return null;
        }
    }
}

[CustomEditor(typeof(CreateTrajectoryAction))]
public class CreateTrajectoryActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CreateTrajectoryAction c = (CreateTrajectoryAction)target;
        if (GUILayout.Button("Execute"))
        {
            c.Execute();
        }

    }
}
