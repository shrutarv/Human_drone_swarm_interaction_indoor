using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RunTrajectoryAction : MonoBehaviour, DroneAction
{
    public CreateTrajectoryAction trajectoryAction;
    public GameObject endPoint;
    public DroneController controller;
    public AutoPilot autoPilot;
    public readonly Guid id = Guid.NewGuid();
    public GameObject drone;

    public GameObject temporaryEndPoint;
    public bool ignoreFirstTrigger = false;

    public float timescale = 1;
    public string groupMask = "0b00000001";

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
        
    }

    public void Execute()
    {
        if (!running)
        {            
            endPoint = trajectoryAction.GetEndPointGameObject();
            if(endPoint == null)
            {
                CreateTemporaryEndPoint();
                endPoint = temporaryEndPoint;
                ignoreFirstTrigger = true;
            }

            autoPilot.GetConnection().StartTrajectory(autoPilot.id, 0, timescale, groupMask);
            running = true;
        }
    }

    public void CreateTemporaryEndPoint()
    {
        temporaryEndPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        temporaryEndPoint.name = "temp_" + id;
        temporaryEndPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        temporaryEndPoint.transform.position = trajectoryAction.GetEndPoint().Value;
        var collider = temporaryEndPoint.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        var rb = temporaryEndPoint.AddComponent<Rigidbody>();
        rb.useGravity = false;


    }

    public void DestroyTemporaryEndPoint()
    {
        Destroy(temporaryEndPoint);
        temporaryEndPoint = null;
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
        Debug.Log($"TRIGGER {col.gameObject.name} ");
        if (col.gameObject == endPoint && running)
        {
            if (ignoreFirstTrigger)
            {
                ignoreFirstTrigger = false;
                return;
            } else
            {
                if (temporaryEndPoint != null)
                {
                    DestroyTemporaryEndPoint();
                }
                endPoint = null;
            }

            autoPilot.FinishedAction(id);
            running = false;
            nearlyFinished = false;
        }
    }

}

[CustomEditor(typeof(RunTrajectoryAction))]
public class RunTrajectoryActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RunTrajectoryAction r = (RunTrajectoryAction)target;
        if (GUILayout.Button("Execute"))
        {
            r.Execute();
        }

        if (GUILayout.Button("Create Temporary Endpoint"))
        {
            r.CreateTemporaryEndPoint();
        }

        if (GUILayout.Button("Destroy Temporary Endpoint"))
        {
            r.DestroyTemporaryEndPoint();
        }

    }
}
