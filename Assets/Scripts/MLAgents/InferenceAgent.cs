using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InferenceAgent : Agent
{
    public Vector3 position;
    public Vector3 target;
    public Vector3 output;

    
    void Awake()
    {
        agentParameters = new AgentParameters();
    }


    public override void InitializeAgent()
    {
        agentParameters.resetOnDone = false;
        agentParameters.onDemandDecision = true;
    }

    public override void AgentReset()
    {

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        output = new Vector3(vectorAction[0], 0, vectorAction[1]);
    }

    public override void CollectObservations()
    {
        AddVectorObs(position);
        AddVectorObs(target);
    }

    public override void AgentOnDone()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [CustomEditor(typeof(InferenceAgent))]
    public class InferenceAgentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InferenceAgent i = (InferenceAgent)target;
            if (GUILayout.Button("Done"))
            {
                i.Done();
            }
            if (GUILayout.Button("Request Decision"))
            {
                i.RequestDecision();
            }

        }
    }


}
