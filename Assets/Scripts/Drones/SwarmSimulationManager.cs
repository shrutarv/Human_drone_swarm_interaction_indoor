using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwarmSimulationManager : MonoBehaviour
{

    public SwarmCreator SwarmCreator;
    public SwarmController SwarmController;

    public int numberOfDrones = 4;

    // Start is called before the first frame update
    void Start()
    {
        SwarmCreator = GetComponent<SwarmCreator>();
        SwarmController = GetComponent<SwarmController>();
    }

    public List<LeaderController> GetLeaderControllers()
    {
        List<LeaderController> list = new List<LeaderController>();
        foreach (Transform drone in transform)
        {
            list.Add(drone.GetComponentInChildren<LeaderController>());
        }
        return list;
    }

    public void StartSimulation()
    {
        SwarmCreator.CreateSimRow(numberOfDrones);
        SwarmController.UseSimulatorForAllDrones();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetSimulation()
    {
        StartCoroutine(ResetSimulationCoroutine());
        
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

    

    [CustomEditor(typeof(SwarmSimulationManager))]
    public class SwarmSimulationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SwarmSimulationManager s = (SwarmSimulationManager)target;
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
