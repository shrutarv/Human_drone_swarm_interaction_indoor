using System;
using UnityEditor;
using UnityEngine;

namespace Helper
{
    [CustomEditor(typeof(DeadlockCreator))]
    public class DeadlockCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DeadlockCreator dc = (DeadlockCreator)target;
            if (GUILayout.Button("Create Deadlock"))
            {
                dc.CreateDeadlock();
            }
        }
    }
}
