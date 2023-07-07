using System;
using UnityEditor;
using UnityEngine;

namespace Helper
{
    [CustomEditor(typeof(PlayerControllerSM))]
    public class PlayerControllerSMEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
            //UnityEditorInternal.ComponentUtility.

            DrawDefaultInspector();

            PlayerControllerSM pc = (PlayerControllerSM)target;
            if (GUILayout.Button("Log History"))
            {
                pc.LogHistory();
            }
            if (GUILayout.Button("Toggle Highlight"))
            {
                pc.ToggleHighlight();
            }
            if (GUILayout.Button("Reserve Path"))
            {
                pc.ReservePath();
            }
            if (GUILayout.Button("Log Dependencies"))
            {
                pc.LogReservationDependecies();
            }
            if (GUILayout.Button("Invalidate Reservation"))
            {
                pc.ButtonInvalidateReservation();
            }
            if (pc.stepMode && GUILayout.Button("Step To Next Section"))
            {
                pc.StepToNextSection();
            }
        }
    }
}
