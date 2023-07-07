using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MLSwarmSimulationManager : MonoBehaviour
{

    public MLSwarmCreator SwarmCreator;
    public MLSwarmController SwarmController;

    public int numberOfDrones = 4;

    // Start is called before the first frame update
    void Start()
    {
        SwarmCreator = GetComponent<MLSwarmCreator>();
        SwarmController = GetComponent<MLSwarmController>();
    }

    public List<MLLeaderController> GetMLLeaderControllers()
    {
        List<MLLeaderController> list = new List<MLLeaderController>();
        foreach (Transform drone in transform)
        {
            list.Add(drone.GetComponentInChildren<MLLeaderController>());
        }
        return list;
    }

    public void StartSimulation()
    {
        SwarmCreator.CreateRow(numberOfDrones);
        //StartCoroutine(StartSimulationCoroutine());
    }

    IEnumerator StartSimulationCoroutine()
    {
        SwarmCreator.CreateRow(numberOfDrones);
        //SwarmController.UseSimulatorForAllDrones();
        //SwarmController.StartAllDrones();
        yield return new WaitForSeconds(1f);
        //SwarmController.ToggleCohesion();
        //SwarmController.ToggleAlignment();
        //SwarmController.LetThemWander();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetSimulation()
    {
        SwarmCreator.DeleteAllDrones();
        StartSimulation();
        //StartCoroutine(ResetSimulationCoroutine());
        
    }

    public void StopSimulation()
    {
        SwarmCreator.DeleteAllDrones();
    }

    IEnumerator ResetSimulationCoroutine()
    {
        SwarmCreator.DeleteAllDrones();
        yield return new WaitForSeconds(0.01f);
        StartSimulation();
    }

    [CustomEditor(typeof(MLSwarmSimulationManager))]
    public class MLSwarmSimulationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MLSwarmSimulationManager s = (MLSwarmSimulationManager)target;
            if (GUILayout.Button("Start Simulation"))
            {
                s.StartSimulation();
            }

            if (GUILayout.Button("Reset Simulation"))
            {
                s.ResetSimulation();
            }

            if (GUILayout.Button("Stop Simulation"))
            {
                s.StopSimulation();
            }


        }
    }

}
