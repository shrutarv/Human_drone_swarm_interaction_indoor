using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DroneSimulator : MonoBehaviour, IDroneConnection
{

    public GameObject floor;
    public DroneController controller;
    public Vector3 homePosition;
    public Vector3 homeLandedPosition;
    public Vector3[] trajectory = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetFloor()
    {
        return floor;
    }

    public DroneController GetController()
    {
        if (controller == null)
        {
            controller = gameObject.GetComponent<DroneController>();
        }
        return controller;
    }

    public void CreateTrajectory(int id, double vmax, double amax, string groupMask, List<Vector3> waypoints)
    {
        trajectory = waypoints.ToArray();
    }

    public void Land(int id, double starttime, double duration)
    {
        var pos = transform.position;
        pos.y = homeLandedPosition.y;
        iTween.MoveTo(gameObject, pos, (float)duration);
    }

    public void LandWithHeight(int id, double starttime, double duration, double height)
    {
        var pos = transform.position;
        pos.y = (float)height;
        iTween.MoveTo(gameObject, pos, (float)duration);
    }

    public void MoveHome(int id, double starttime, double duration)
    {
        iTween.MoveTo(gameObject, homePosition, (float)duration);
    }

    public void MoveRelative(int id, double starttime, double duration, double x, double y, double z, double yaw)
    {
        var pos = transform.position;
        pos.x += (float)x;
        pos.y += (float)z;
        pos.z += (float)y;
        iTween.MoveTo(gameObject, pos, (float)duration);
    }

    public void MoveTo(int id, double starttime, double duration, double x, double y, double z, double yaw)
    {
        var pos = new Vector3((float)x, (float)z, (float)y);
        iTween.MoveTo(gameObject, pos, (float)duration);
    }

    public void Start(int id, double starttime, double duration, double height)
    {
        var pos = transform.position;
        homeLandedPosition = transform.position;
        pos.y = (float)height;
        homePosition = transform.position;
        iTween.MoveTo(gameObject, pos, (float)duration);
    }

    public void StartTrajectory(int id, double starttime, double timescale, string groupMask)
    {
        iTween.MoveTo(gameObject, iTween.Hash("path", trajectory, "speed", 1, "easetype", iTween.EaseType.spring));
    }

    public void UseSimulator()
    {
        GetController().SetDroneConnection(this);
    }

    public void UseLiveConnection()
    {
        GetController().SetToCrazyflieConnection();
    }
}

[CustomEditor(typeof(DroneSimulator))]
public class DroneSimulatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DroneSimulator s = (DroneSimulator)target;
        if (GUILayout.Button("Use Simulator"))
        {
            s.UseSimulator();
        }
        if (GUILayout.Button("Use Live Connection"))
        {
            s.UseLiveConnection();
        }

    }
}
