using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoPilot : MonoBehaviour, DroneActionListener
{
    public int id { get { return controller.id; } } 
    public DroneController controller;
    public GameObject drone;

    public bool running = false;

    public int index = 0;

    public float delayedStartDuration = 0;
    public float trajectoryUploadDuration = 5;

    public string currentActionName;
    public float currentActionTimeLeft;

    public string nextActionName;
    public float nextActionTimeLeft;

    DroneAction currentAction;
    DroneAction nextAction;

    public bool isHomePositionInitialized = false;
    public Vector3 homeLandPosition;
    public Vector3 homeHoverPosition;

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.parent.gameObject.GetComponent<DroneController>();
        drone = transform.parent.gameObject;
    }

    public GameObject GetDrone()
    {
        if(drone == null)
        {
            drone = transform.parent.gameObject;
        }
        return drone;
    }

    public DroneController GetController()
    {
        if (controller == null)
        {
            controller = transform.parent.gameObject.GetComponent<DroneController>();
        }
        return controller;
    }

    public IDroneConnection GetConnection()
    {
        return GetController().GetConnection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FinishedAction(Guid actionId)
    {
        ExecuteNextStep();
    }

    public void NearlyFinished(Guid actionId)
    {
        
    }

    public void TimeLeft(Guid actionId, float amount)
    {
        currentActionTimeLeft = amount;
    }

    public void ExecuteAllSteps()
    {
        if (!running)
        {
            running = true;
            ExecuteNextStep();
        }
    }

    internal void ExecuteNextStep()
    {
        if (running)
        {
            if (transform.childCount > index)
            {
                var child = transform.GetChild(index);
                if (child != null)
                {
                    currentActionName = transform.GetChild(index).gameObject.name;
                    currentAction = transform.GetChild(index).GetComponent<DroneAction>();
                    currentAction.Execute();
                    index++;
                }
            }
            else
            {
                Reset();
            }
        }
    }

    public void Reset()
    {
        running = false;
        index = 0;
        delayedStartDuration = 0;
    }

    public void TriggerEnter(Collider col)
    {
        if (currentAction != null)
        {
            currentAction.Trigger(col);
        }
    }

    public void CreatePathThroughWorkstations()
    {
        var droneCourse = GameObject.Find("DroneCourse");
        var firstGate = droneCourse.transform.Find("Gate03");
        var firstEntry = firstGate.Find("Entry");
        var firstMain = firstGate.Find("Main");
        var firstExit = firstGate.Find("Exit");
        var secondGate = droneCourse.transform.Find("Gate02");
        var secondEntry = secondGate.Find("Entry");
        var secondMain = secondGate.Find("Main");
        var secondExit = secondGate.Find("Exit");

        {
            var actionGO = new GameObject("DelayedStart");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<DelayedStartAction>();
        }
        {
            var actionGO = new GameObject("Start");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<StartAction>();
            action.startHeight = 1.5f;
            action.velocity = 1f;
        }
        {
            var actionGO = new GameObject("AS_3_Entry");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveToTargetAction>();
            action.target = firstEntry.gameObject;
            action.offset.x = -1.5f;
            action.velocity = 2f;
        }
        {
            var actionGO = new GameObject("AS_3_Main");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveToTargetAction>();
            action.target = firstMain.gameObject;
            action.offset.x = 0;
            action.velocity = 1f;
        }
        {
            var actionGO = new GameObject("AS_3_Exit");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveToTargetAction>();
            action.target = firstExit.gameObject;
            action.offset.x = 0;
            action.velocity = 1.5f;
        }
        {
            var actionGO = new GameObject("AS_2_Entry");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveToTargetAction>();
            action.target = secondEntry.gameObject;
            action.offset.x = -1.5f;
            action.velocity = 2f;
        }
        {
            var actionGO = new GameObject("AS_2_Main");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveToTargetAction>();
            action.target = secondMain.gameObject;
            action.offset.x = 0;
            action.velocity = 1f;
        }
        {
            var actionGO = new GameObject("AS_2_Exit");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveToTargetAction>();
            action.target = secondExit.gameObject;
            action.offset.x = 0;
            action.velocity = 1.5f;
        }
        {
            var actionGO = new GameObject("MoveHome");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveHomeAction>();
            action.velocity = 1f;
        }
        {
            var actionGO = new GameObject("MoveHomeWait");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<MoveHomeAction>();
            action.velocity = 1f;
        }
        {
            var actionGO = new GameObject("Land");
            actionGO.transform.parent = transform;
            var action = actionGO.AddComponent<LandAction>();
            action.velocity = 1f;
        }
    }

}


[CustomEditor(typeof(AutoPilot))]
public class AutoPilotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AutoPilot a = (AutoPilot)target;
        if (GUILayout.Button("Create Path Through Workstations"))
        {
            a.CreatePathThroughWorkstations();
        }

        if (GUILayout.Button("Execute All Steps"))
        {
            a.ExecuteAllSteps();
        }
        if (GUILayout.Button("Reset"))
        {
            a.Reset();
        }



    }
}
