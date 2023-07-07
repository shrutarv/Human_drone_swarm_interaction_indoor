using System;
using UnityEditor;
using UnityEngine;

namespace Helper
{
    [CustomEditor(typeof(AgentCreator))]
    public class AgentCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AgentCreator ac = (AgentCreator)target;
            if (GUILayout.Button("Create Matrix"))
            {
                ac.CreateMatrix();
            }
        }
    }
}
