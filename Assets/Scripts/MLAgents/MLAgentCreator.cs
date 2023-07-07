using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MLAgentCreator : MonoBehaviour
{
    public int nextId = 0;
    DroneAcademy droneAcademy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAgent()
    {
        droneAcademy = GameObject.Find("MLDroneAcademy").GetComponent<DroneAcademy>();
        GameObject AgentObj = new GameObject($"InferenceAgent{nextId++}");
        AgentObj.transform.parent = transform;
        InferenceAgent Agent = AgentObj.AddComponent<InferenceAgent>();
        Agent.GiveBrain(droneAcademy.broadcastHub.broadcastingBrains[0]);
        //Agent.AgentReset();
    }

    [CustomEditor(typeof(MLAgentCreator))]
    public class MLAgentCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MLAgentCreator c = (MLAgentCreator)target;
            if (GUILayout.Button("Create Agent"))
            {
                c.CreateAgent();
            }

        }
    }

}
