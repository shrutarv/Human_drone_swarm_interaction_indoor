using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MLSwarmController : MonoBehaviour
{
    

    public float step = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunAllAutoPilots()
    {
        List<AutoPilot> drones = new List<AutoPilot>();
        for (int i = 0; i < transform.childCount; i++)
        {
            drones.Add(transform.GetChild(i).Find("Drone").GetComponentInChildren<AutoPilot>());
        }
        float delay = 0;
        foreach (var pilot in drones)
        {
            if (!pilot.running)
            {
                pilot.delayedStartDuration = delay;
            }
            delay = delay + step;
        }
        foreach (var pilot in drones)
        {
            pilot.ExecuteAllSteps();
        }
    }

    public void UseCrazyflieConnectionForAllDrones()
    {
        foreach (Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.SetToCrazyflieConnection();
        }
    }

    public void UseSimulatorForAllDrones()
    {
        foreach (Transform drone in transform)
        {
            var simulator = drone.GetComponentInChildren<DroneSimulator>();
            simulator.UseSimulator();
        }
    }

    public void StartAllDrones()
    {
        foreach(Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.DroneStart();
        }
    }

    public void LandAllDrones()
    {
        foreach (Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.DroneLand();
        }
    }

    public void LetThemWander()
    {
        foreach (Transform drone in transform)
        {
            var leader = drone.GetComponentInChildren<MLLeaderController>();
            leader.leadingDroneActive = true;
            leader.isWanderingActive = true;
            leader.directFlight = false;
        }
    }

    public void LetsMoveHome()
    {
        foreach (Transform drone in transform)
        {
            var leader = drone.GetComponentInChildren<MLLeaderController>();
            var controller = drone.GetComponentInChildren<DroneController>();
            leader.targetPosition = controller.homeHoverPosition;
            leader.leadingDroneActive = true;
            leader.isWanderingActive = false;
            leader.directFlight = true;
        }
    }

    public void StopWandering()
    {
        foreach (Transform drone in transform)
        {
            var leader = drone.GetComponentInChildren<MLLeaderController>();
            leader.leadingDroneActive = false;
            leader.isWanderingActive = false;
            leader.directFlight = false;
        }
    }

    public void LetsExactlyMoveHome()
    {
        foreach (Transform drone in transform)
        {
            var controller = drone.GetComponentInChildren<DroneController>();
            controller.DroneMoveHome();
        }
    }

    public void ToggleCohesion()
    {
        foreach (Transform drone in transform)
        {
            var leader = drone.GetComponentInChildren<MLLeaderController>();
            leader.isCohesionActive = !leader.isCohesionActive;

        }
    }

    public void ToggleAlignment()
    {
        foreach (Transform drone in transform)
        {
            var leader = drone.GetComponentInChildren<MLLeaderController>();
            leader.isAlignmentActive = !leader.isAlignmentActive;

        }
    }

}

[CustomEditor(typeof(MLSwarmController))]
public class MLSwarmControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MLSwarmController s = (MLSwarmController)target;

        if (GUILayout.Button("Use Crazyflie Connection"))
        {
            s.UseCrazyflieConnectionForAllDrones();
        }

        if (GUILayout.Button("Use Simulation"))
        {
            s.UseSimulatorForAllDrones();
        }

        if (GUILayout.Button("Start"))
        {
            s.StartAllDrones();
        }

        if (GUILayout.Button("Land"))
        {
            s.LandAllDrones();
        }

        if (GUILayout.Button("Wander"))
        {
            s.LetThemWander();
        }

        if (GUILayout.Button("Wander Home"))
        {
            s.LetsMoveHome();
        }

        if (GUILayout.Button("Stop Wandering"))
        {
            s.StopWandering();
        }

        if (GUILayout.Button("Toggle Cohesion"))
        {
            s.ToggleCohesion();
        }

        if (GUILayout.Button("Toggle Alignment"))
        {
            s.ToggleAlignment();
        }

        if (GUILayout.Button("Move Home Exactly"))
        {
            s.LetsExactlyMoveHome();
        }


    }
}
