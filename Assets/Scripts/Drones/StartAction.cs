using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StartAction : MonoBehaviour, DroneAction
{
    public Vector3 target;
    public float startHeight;
    public DroneController controller;
    public AutoPilot autoPilot;
    public readonly Guid id = Guid.NewGuid();
    public GameObject drone;


    public float velocity = 1;
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

    public void Execute()
    {
        if (!running)
        {
            if (!autoPilot.isHomePositionInitialized)
            {
                autoPilot.homeLandPosition = drone.transform.position;
                autoPilot.homeHoverPosition = drone.transform.position;
                autoPilot.homeHoverPosition.y = controller.height;
                autoPilot.isHomePositionInitialized = true;
            }
            target = autoPilot.homeHoverPosition;
            controller.homeHoverPosition = target;
            var duration = ComputeDuration();
            timeLeft = duration;
            nearlyFinishedTime = timeLeft * 0.1f;
            autoPilot.GetConnection().Start(autoPilot.id, 0, duration, startHeight);
            running = true;
        }
    }

    public Guid Id()
    {
        return id;
    }

    public Vector3? TargetPoint()
    {
        return target;
    }

    public GameObject TargetGameObject()
    {
        return null;
    }

    public void Trigger(Collider col)
    {
        //nothing
    }

    internal float ComputeDuration()
    {
        float distance = Vector3.Distance(drone.transform.position, target);
        return Math.Max((distance / velocity), 2f);
    }
}

[CustomEditor(typeof(StartAction))]
public class StartActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StartAction s = (StartAction)target;
        if (GUILayout.Button("Execute"))
        {
            s.Execute();
        }

    }
}
