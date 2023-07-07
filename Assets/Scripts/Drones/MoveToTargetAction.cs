using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveToTargetAction : MonoBehaviour, DroneAction
{
    public GameObject target;
    public GameObject drone;
    public Vector3 offset;
    public float velocity = 1;
    public float timeLeft;
    public float nearlyFinishedTime;
    public bool nearlyFinished = false;
    public bool running = false;
    public AutoPilot autoPilot;
    public DroneController controller;
    public readonly Guid id = Guid.NewGuid();


    // Start is called before the first frame update
    void Start()
    {
        autoPilot = GetAutoPilot();
        drone = autoPilot.GetDrone();
        controller = autoPilot.GetController();
    }

    internal AutoPilot GetAutoPilot()
    {
        var a = transform.parent.gameObject.GetComponent<AutoPilot>();
        if(a == null)
        {
            a = transform.parent.parent.gameObject.GetComponent<AutoPilot>();
        }
        return a;
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timeLeft -= Time.deltaTime;
            autoPilot.TimeLeft(id, timeLeft);
            
            if(timeLeft <= 0)
            {
                autoPilot.FinishedAction(id);
                running = false;
                nearlyFinished = false;
                timeLeft = 0;
            }
        }
    }

    public Guid Id()
    {
        return id;
    }

    public void Trigger(Collider col)
    {
        if(col.gameObject == target && !nearlyFinished)
        {
            autoPilot.NearlyFinished(id);
            nearlyFinished = true;
        }
    }

    public void Execute()
    {
        if (!running)
        {
            var duration = ComputeDuration();
            timeLeft = duration;
            nearlyFinishedTime = timeLeft * 0.1f;
            autoPilot.GetConnection().MoveTo(autoPilot.id, 0, duration, target.transform.position.x, target.transform.position.z, target.transform.position.y, 0);
            running = true;
        }
    }

    internal float ComputeDuration()
    {
        float distance = Vector3.Distance(drone.transform.position, target.transform.position);
        return Math.Max((distance / velocity), 2f);
    }

    public Vector3? TargetPoint()
    {
        return target.transform.parent.TransformPoint(target.transform.localPosition + offset);
    }

    public GameObject TargetGameObject()
    {
        return target;
    }

}

[CustomEditor(typeof(MoveToTargetAction))]
public class MoveToTargetActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MoveToTargetAction m = (MoveToTargetAction)target;
        if (GUILayout.Button("Execute"))
        {
            m.Execute();
        }
        
    }
}


