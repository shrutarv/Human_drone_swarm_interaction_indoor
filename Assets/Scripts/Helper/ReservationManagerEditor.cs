using System;
using Reservation;
using UnityEditor;
using UnityEngine;

namespace Helper
{
    [CustomEditor(typeof(ReservationManager))]
    public class ReservationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ReservationManager rm = (ReservationManager)target;
            if (GUILayout.Button("Log History"))
            {
                rm.LogHistory();
            }
            if (GUILayout.Button("Toggle Highlight"))
            {
                rm.ToggleHighlight();
            }
        }
    }
}
